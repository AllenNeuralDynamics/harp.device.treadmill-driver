#include <pico/stdlib.h>
#include <cstring>
#include <pio_encoder.h>
#include <pio_ads7049.h>
#include <pio_ltc264x.h>
#include <config.h>
#include <harp_message.h>
#include <harp_core.h>
#include <harp_c_app.h>
#include <harp_synchronizer.h>
#ifdef DEBUG
    #include <cstdio> // for printf
#endif

// encoder program is 29 instructions, so it needs to go on its own PIO slice.
PIOEncoder encoder(pio1, 0, ENCODER_BASE_PIN);
// Create PIO SPI ADC instances for current and torque transducer sensing.
// Both PIO_ADS7049 instances can use the same PIO program.
PIO_ADS7049 current_sensor(pio0,
                           BRAKE_CURRENT_CS_PIN,
                           BRAKE_CURRENT_SCK_PIN, BRAKE_CURRENT_POCI_PIN);
// Use PIO_ADS7049 program already-loaded by current sensor.
PIO_ADS7049 reaction_torque_sensor(pio0, TORQUE_TRANSDUCER_CS_PIN,
                                   TORQUE_TRANSDUCER_SCK_PIN,
                                   TORQUE_TRANSDUCER_POCI_PIN,
                                   current_sensor.get_program_address());
// Create PIO SPI DAC instance for driving the brake current setpoint.
PIO_LTC264x brake_setpoint(pio0, // CS pin is SCK pin + 1
                           BRAKE_SETPOINT_SCK_PIN,
                           BRAKE_SETPOINT_PICO_PIN);



// Create device name array.
const uint16_t who_am_i = TREADMILL_HARP_DEVICE_ID;
const uint8_t assembly_version = 0;
const uint8_t harp_version_major = 0;
const uint8_t harp_version_minor = 0;
const uint16_t serial_number = 0;

// Setup for Harp App
const size_t reg_count = 10;

uint32_t __not_in_flash("dispatch_interval_us") dispatch_interval_us;
uint32_t __not_in_flash("next_msg_dispatch_time_us") next_msg_dispatch_time_us;

// PIO and DMA will periodically write raw values to these locations.
volatile uint32_t __not_in_flash("encoder_raw") encoder_raw;
volatile int16_t __not_in_flash("torque_raw") torque_raw;
volatile int16_t __not_in_flash("brake_current_raw") brake_current_raw;

// Filtered value for torque limit.
volatile int32_t __not_in_flash("filtered_torque") filtered_torque;
uint32_t __not_in_flash("torque_limit_interval_us") torque_limit_interval_us;
uint32_t __not_in_flash("next_torque_check_time_us") next_torque_check_time_us;

// offset --> measurement taken at requested time.
int32_t __not_in_flash("encoder_offset") encoder_offset;
int16_t __not_in_flash("torque_offset") torque_offset;
int16_t __not_in_flash("brake_current_offset") brake_current_offset;

inline uint32_t get_tared_encoder_ticks()
{ return encoder_raw - encoder_offset;}

// DMA writes the data to torque_raw and brake_current_raw word-by-word
// (2 bytes at a time in this case).
// Subtlety: these "getter functions" make an atomic copy of the data at the
// location that DMA is writing to into the CPU registers before doing the
// subtraction, hence preventing part of the word (1 of 2 bytes) from being
// overwritten by DMA while this function executes.
inline int16_t get_tared_reaction_torque()
{ return torque_raw - torque_offset;}
inline int16_t get_tared_brake_current()
{ return brake_current_raw - brake_current_offset;}

#pragma pack(push, 1)
struct app_regs_t
{
    int32_t encoder_ticks;   // 32.
    int16_t reaction_torque;  // 33. 12-bit. underlying measurement is signed.
    int16_t brake_current;    // 34. 12-bit. underlying measurement is unsigned
                              //   but can go negative because of tare value.
    int32_t sensors[3]; // aggregate vector of the above 3 three registers:
                         // [position, uint32(torque), uint32(current)]
    uint16_t sensor_dispatch_frequency_hz;  // 36
    uint16_t brake_current_setpoint;    // 37. 16-bit full-scale range,
                                        // but 12-bit resolution. Unsigned.
                                        // This value is cleared to 0 if
                                        // torque_limiting is enabled and
                                        // triggered. Further writes in this
                                        // condition return a WRITE_ERROR.
    uint8_t tare;       // {unused[7:3], brake_current[2], torque[1], encoder[0]}
    uint8_t reset_tare; // {unused[7:3], brake_current[2], torque[1], encoder[0]}
    uint8_t torque_limiting;    // 1 --> Disable of the brake if the
                                //       maximum torque sensor value is detected.
                                //       This feature prevents the reaction
                                //       torque sensor from being damaged.
                                //       Resets to this state.
                                // 0 --> Do not disable the brake if the
                                //       maximum torque sensor is detected.
    uint8_t torque_limiting_triggered; // 1 --> torque limit triggered. Brake
                                       //       is disabled and
                                       //       brake_current_setpoint is
                                       //       set to 0.
                                       //       An EVENT msg is sent when this
                                       //       value occurs.
                                       // Write 0 to clear the torque-limit
                                       // condition and re-enable the brake.
    // More app "registers" here.
};
#pragma pack(pop)
app_regs_t __not_in_flash("app_regs") app_regs;

// Define "specs" per-register
RegSpecs app_reg_specs[reg_count]
{
    {(uint8_t*)&app_regs.encoder_ticks, sizeof(app_regs.encoder_ticks), S32},
    {(uint8_t*)&app_regs.reaction_torque, sizeof(app_regs.reaction_torque), S16},
    {(uint8_t*)&app_regs.brake_current, sizeof(app_regs.brake_current), S16},
    {(uint8_t*)&app_regs.sensors, sizeof(app_regs.sensors), S32},
    {(uint8_t*)&app_regs.sensor_dispatch_frequency_hz, sizeof(app_regs.sensor_dispatch_frequency_hz), U16},
    {(uint8_t*)&app_regs.brake_current_setpoint, sizeof(app_regs.brake_current_setpoint), U16},
    {(uint8_t*)&app_regs.tare, sizeof(app_regs.tare), U8},
    {(uint8_t*)&app_regs.reset_tare, sizeof(app_regs.reset_tare), U8},
    {(uint8_t*)&app_regs.torque_limiting, sizeof(app_regs.torque_limiting), U8},
    {(uint8_t*)&app_regs.torque_limiting_triggered, sizeof(app_regs.torque_limiting_triggered), U8}
    // More specs here if we add additional registers.
};

void write_sensor_dispatch_frequency_hz(msg_t& msg)
{
    msg_type_t msg_reply_type = WRITE;
    HarpCore::copy_msg_payload_to_register(msg);
    // Clamp maximum value.
    if (app_regs.sensor_dispatch_frequency_hz > MAX_EVENT_FREQUENCY_HZ)
    {
        // Update register and dependedent values.
        app_regs.sensor_dispatch_frequency_hz = MAX_EVENT_FREQUENCY_HZ;
        msg_reply_type = WRITE_ERROR;
    }
    if (app_regs.sensor_dispatch_frequency_hz > 0)
    {
        dispatch_interval_us = div_u32u32(1'000'000,
                                          uint32_t(app_regs.sensor_dispatch_frequency_hz));
    }
    // Update next time to dispatch.
    next_msg_dispatch_time_us = time_us_32();
    HarpCore::send_harp_reply(msg_reply_type, msg.header.address);
}

void write_brake_current_setpoint(msg_t& msg)
{
    // Note: LTC2641 driver clamps the resolution to 12-bit even though the
    // full-scale range is 16 bit.
    // Note: offset is not applied to desired current setpoint because it is
    //  distinct from measured current.
    if (app_regs.torque_limiting_triggered) // i.e: brake should be disabled.
    {
        HarpCore::send_harp_reply(WRITE_ERROR, msg.header.address);
        return;
    }
    HarpCore::copy_msg_payload_to_register(msg);
    brake_setpoint.write_value(app_regs.brake_current_setpoint);
    HarpCore::send_harp_reply(WRITE, msg.header.address);
}

void write_tare(msg_t& msg)
{
    HarpCore::copy_msg_payload_to_register(msg);
    // Handle bits to apply tare value.
    if (1u << 0 & app_regs.tare) // Zero encoder
        encoder_offset = encoder_raw;
    if (1u << 1 & app_regs.tare) // Zero reaction torque sensor
        torque_offset = torque_raw;
    if (1u << 2 & app_regs.tare) // Zero brake current sensor
        brake_current_offset = brake_current_raw;
    HarpCore::send_harp_reply(WRITE, msg.header.address);
}

void write_reset_tare(msg_t& msg)
{
    HarpCore::copy_msg_payload_to_register(msg);
    // Handle bits to clear tare value.
    if (1u << 0 & app_regs.tare) // Reset encoder to native value.
    {
        encoder_offset = 0;
        app_regs.tare &= ~(1u << 0); // Also clear tare setting in tare register.
    }
    if (1u << 1 & app_regs.tare) // Remove reaction torque sensor offset.
    {
        torque_offset = 0;
        app_regs.tare &= ~(1u << 1); // Also clear tare setting in tare register.
    }
    if (1u << 2 & app_regs.tare) // Remove brake current sensor offset.
    {
        brake_current_offset = 0;
        app_regs.tare &= ~(1u << 2); // Also clear tare setting in tare register.
    }
    // Clear register since it reads as 0.
    app_regs.reset_tare = 0;
    HarpCore::send_harp_reply(WRITE, msg.header.address);
}

void read_reg_encoder_ticks(uint8_t reg_name)
{
    app_regs.encoder_ticks = get_tared_encoder_ticks();
    HarpCore::send_harp_reply(READ, reg_name);
}

void read_reg_reaction_torque(uint8_t reg_name)
{
    app_regs.reaction_torque = get_tared_reaction_torque();
    HarpCore::send_harp_reply(READ, reg_name);
}

void read_reg_brake_current(uint8_t reg_name)
{
    app_regs.brake_current = get_tared_brake_current();
    HarpCore::send_harp_reply(READ, reg_name);
}

void update_sensor_register()
{
    app_regs.sensors[0] = get_tared_encoder_ticks();
    // Both torque sensor and brake current sensor are signed int16s, but
    // we promote to int32 for now to send an array of one type as a single msg.
    app_regs.sensors[1] = int32_t(get_tared_reaction_torque());
    app_regs.sensors[2] = int32_t(get_tared_brake_current());
}

void read_reg_sensors(uint8_t reg_name)
{
    update_sensor_register();
    HarpCore::send_harp_reply(READ, reg_name);
}

void update_torque_limit_monitor()
{
    // Check measured torque limit and disable the brake if we measure either
    // limit.
    // Bail early if torque limiting is unset or we already tripped it.
    if (!app_regs.torque_limiting || app_regs.torque_limiting_triggered)
        return;
    // y[n] = 15/16 * y[n-1] + 1/16 * x[n]
    filtered_torque = ((filtered_torque*15) >> 4) + (torque_raw >> 4);
    if (filtered_torque > RAW_TORQUE_SENSOR_MIN && filtered_torque < RAW_TORQUE_SENSOR_MAX)
        return;
    // Kill the brake; clear the current brake setpoint.
    app_regs.brake_current_setpoint = 0;
    brake_setpoint.write_value(0);
    app_regs.torque_limiting_triggered = 1; //i.e: brake disabled.
    if (HarpCore::is_muted())
        return;
    const uint8_t address_offset = 9; // torque_limiting_triggered reg.
    HarpCore::send_harp_reply(EVENT, (APP_REG_START_ADDRESS + address_offset));
}

RegFnPair reg_handler_fns[reg_count]
{
    {&read_reg_encoder_ticks, &HarpCore::write_to_read_only_reg_error},
    {&read_reg_reaction_torque, &HarpCore::write_to_read_only_reg_error},
    {&read_reg_brake_current, &HarpCore::write_to_read_only_reg_error},
    {&read_reg_sensors, &HarpCore::write_to_read_only_reg_error},
    {&HarpCore::read_reg_generic, &write_sensor_dispatch_frequency_hz},
    {&HarpCore::read_reg_generic, &write_brake_current_setpoint},
    {&HarpCore::read_reg_generic, &write_tare},
    {&HarpCore::read_reg_generic, &write_reset_tare},
    {&HarpCore::read_reg_generic, &HarpCore::write_reg_generic},
    {&HarpCore::read_reg_generic, &HarpCore::write_reg_generic}
    // More handler function pairs here if we add additional registers.
};

void update_app_state()
{
    uint32_t curr_time_us = time_us_32();
    // Handle periodic safety check.
    if (int32_t(curr_time_us - next_torque_check_time_us) >= torque_limit_interval_us)
    {
        next_torque_check_time_us += torque_limit_interval_us;
        update_torque_limit_monitor();
    }
    // Update encoder count.
    // (Brake current and Transducer Torque update automatically.)
    encoder_raw = encoder.fetch_count(); // Get previously-requested count.
    encoder.request_count(); // request count for the next iteration.
    if (HarpCore::is_muted() || (app_regs.sensor_dispatch_frequency_hz == 0))
        return;
    // Handle periodic sensor register dispatch.
    if (int32_t(curr_time_us - next_msg_dispatch_time_us) >= dispatch_interval_us)
    {
        next_msg_dispatch_time_us += dispatch_interval_us;
        update_sensor_register();
        const uint8_t address_offset = 3; // "sensors" register address.
        HarpCore::send_harp_reply(EVENT, APP_REG_START_ADDRESS + address_offset);
    }
}

void reset_app()
{
    app_regs.sensor_dispatch_frequency_hz = 0;
    app_regs.tare = 0b111 << 4; // All sensor "untare" bits are set.
    dispatch_interval_us = 0;
    app_regs.brake_current_setpoint = 0;
    app_regs.torque_limiting = 1;
    app_regs.torque_limiting_triggered = 0;
    brake_setpoint.write_value(app_regs.brake_current_setpoint);
    // Clear torque and brake current offsets.
    torque_offset = 0;
    brake_current_offset = 0;
    // Zero encoder by saving current position as offset.
    encoder_offset = encoder.get_count();
    encoder.request_count(); // Enter update loop by first requesting encoder count.
    // Clear internal filters
    filtered_torque = 0;
    torque_limit_interval_us = 1000; // 1[ms]
    next_torque_check_time_us = time_us_32();
}

// Create Core.
HarpCApp& app = HarpCApp::init(who_am_i, HW_VERSION_MAJOR, HW_VERSION_MINOR,
                               assembly_version,
                               harp_version_major, harp_version_minor,
                               FW_VERSION_MAJOR, FW_VERSION_MINOR,
                               serial_number, "Harp.Device.Treadmill",
                               (uint8_t*)GIT_HASH,
                               &app_regs, app_reg_specs,
                               reg_handler_fns, reg_count, update_app_state,
                               reset_app);

// Core0 main.
int main()
{
#ifdef DEBUG
    stdio_uart_init_full(uart1, 921600, UART_TX_PIN, -1); // use uart1 tx only.
    printf("Hello, from an RP2040!\r\n");
#endif
    // Init Synchronizer.
    HarpSynchronizer::init(uart1, HARP_SYNC_RX_PIN);
    app.set_synchronizer(&HarpSynchronizer::instance());
    //app.set_visual_indicators_fn(set_led_state);
    // Init PIO encoder on pins 0 and 1.
    // Init PIO-based ADC with continuous streaming to memory via DMA.
    current_sensor.setup_dma_stream_to_memory((uint16_t*)&brake_current_raw, 1);
    reaction_torque_sensor.setup_dma_stream_to_memory((uint16_t*)&torque_raw, 1);
    // Start PIO-connected hardware.
    current_sensor.start();
    reaction_torque_sensor.start();
    brake_setpoint.start();
    reset_app(); // Apply app register starting values.
    while(true)
        app.run();
}
