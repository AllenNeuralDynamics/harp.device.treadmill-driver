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
const uint8_t hw_version_major = 0;
const uint8_t hw_version_minor = 0;
const uint8_t assembly_version = 0;
const uint8_t harp_version_major = 0;
const uint8_t harp_version_minor = 0;
const uint8_t fw_version_major = 0;
const uint8_t fw_version_minor = 0;
const uint16_t serial_number = 0;

// Setup for Harp App
const size_t reg_count = 7;

uint32_t __not_in_flash("dispatch_interval_us") dispatch_interval_us;
uint32_t __not_in_flash("next_msg_dispatch_time_us") next_msg_dispatch_time_us;

// PIO and DMA will periodically write raw values to these locations.
volatile uint32_t __not_in_flash("encoder_raw") encoder_raw;
volatile int16_t __not_in_flash("torque_raw") torque_raw;
volatile int16_t __not_in_flash("brake_current_raw") brake_current_raw;

// offset --> measurement taken at requested time.
uint32_t __not_in_flash("encoder_offset") encoder_offset;
int16_t __not_in_flash("torque_offset") torque_offset;
int16_t __not_in_flash("brake_current_offset") brake_current_offset;

inline uint32_t get_tared_encoder_ticks(){ return encoder_raw - encoder_offset;}
inline int16_t get_tared_reaction_torque(){ return torque_raw - torque_offset;}
inline int16_t get_tared_brake_current(){ return brake_current_raw - brake_current_offset;}

#pragma pack(push, 1)
struct app_regs_t
{
    uint32_t encoder_ticks;   // 32.
    int16_t reaction_torque;  // 33. 12-bit. underlying measurement is signed.
    int16_t brake_current;    // 34. 12-bit. underlying measurement is unsigned
                              //   but can go negative because of tare value.
    uint32_t sensors[3]; // aggregate vector of the above 3 three registers:
                         // [position, uint32(torque), uint32(current)]
    uint16_t sensor_dispatch_frequency_hz;  // 36
    uint16_t brake_current_setpoint;   // 37. 12-bit. Unsigned.
    uint8_t tare;   // {unused[15:3], brake_current[2], torque[1], encoder[0]}
    //uint16_t errors;   // bitfields
    //uint8_t enable_events; // >0 = error_state changes will send EVENT msgs.
    // More app "registers" here.
};
#pragma pack(pop)
app_regs_t __not_in_flash("app_regs") app_regs;

// Define "specs" per-register
RegSpecs app_reg_specs[reg_count]
{
    {(uint8_t*)&app_regs.encoder_ticks, sizeof(app_regs.encoder_ticks), U32},
    {(uint8_t*)&app_regs.reaction_torque, sizeof(app_regs.reaction_torque), S16},
    {(uint8_t*)&app_regs.brake_current, sizeof(app_regs.brake_current), S16},
    {(uint8_t*)&app_regs.sensors, sizeof(app_regs.sensors), U32},
    {(uint8_t*)&app_regs.sensor_dispatch_frequency_hz, sizeof(app_regs.sensor_dispatch_frequency_hz), U16},
    {(uint8_t*)&app_regs.brake_current_setpoint, sizeof(app_regs.brake_current_setpoint), U16},
    {(uint8_t*)&app_regs.tare, sizeof(app_regs.tare), U8}
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
    msg_type_t msg_reply_type = WRITE;
    HarpCore::copy_msg_payload_to_register(msg);
    // Clamp to 12-bit.
    if (app_regs.brake_current_setpoint > 0x0FFF)
    {
        app_regs.brake_current_setpoint = 0x0FFF;
        msg_reply_type = WRITE_ERROR;
    }
    else if (app_regs.brake_current_setpoint < 0)
    {
        app_regs.brake_current_setpoint = 0;
        msg_reply_type = WRITE_ERROR;
    }
    // Note: offset is not applied to desired current setpoint because it is
    //  distinct from measured current.
    brake_setpoint.write_value(app_regs.brake_current_setpoint);
    HarpCore::send_harp_reply(msg_reply_type, msg.header.address);
}

void write_tare(msg_t& msg)
{
    HarpCore::copy_msg_payload_to_register(msg);
    // Handle bitfield-specific behavior.
    if (0x01 & app_regs.tare) // Zero encoder
        encoder_offset = encoder_raw;
    if (0x02 & app_regs.tare) // Zero reaction torque sensor
        torque_offset = torque_raw;
    if (0x04 & app_regs.tare) // Zero brake current sensor
        brake_current_offset = brake_current_raw;
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

void read_reg_sensors(uint8_t reg_name)
{
    app_regs.sensors[0] = get_tared_encoder_ticks();
    // Both torque sensor and brake current sensor are signed int16s, but
    // we need to stuff them into a u32 array for now.
    app_regs.sensors[1] = uint32_t(get_tared_reaction_torque());
    app_regs.sensors[2] = uint32_t(get_tared_brake_current());
    HarpCore::send_harp_reply(READ, reg_name);
}

RegFnPair reg_handler_fns[reg_count]
{
    {&read_reg_encoder_ticks, &HarpCore::write_to_read_only_reg_error},
    {&read_reg_reaction_torque, &HarpCore::write_to_read_only_reg_error},
    {&read_reg_brake_current, &HarpCore::write_to_read_only_reg_error},
    {&read_reg_sensors, &HarpCore::write_to_read_only_reg_error},
    {&HarpCore::read_reg_generic, &write_sensor_dispatch_frequency_hz},
    {&HarpCore::read_reg_generic, &write_brake_current_setpoint},
    {&HarpCore::read_reg_generic, &write_tare}
    // More handler function pairs here if we add additional registers.
};

void update_app_state()
{
    // Update encoder count.
    // (Brake current and Transducer Torque update automatically.)
    encoder_raw = encoder.fetch_count(); // Get previously-requested count.
    encoder.request_count(); // request count for the next iteration.
    if (HarpCore::is_muted() || (app_regs.sensor_dispatch_frequency_hz == 0))
        return;
    // Handle periodic sensor register dispatch.
    uint32_t curr_time_us = time_us_32();
    if (int32_t(curr_time_us - next_msg_dispatch_time_us) >= dispatch_interval_us)
    {
        next_msg_dispatch_time_us += dispatch_interval_us;
        const uint8_t address_offset = 3; // "sensors" register address.
        const RegSpecs& reg_specs = app_reg_specs[address_offset];
        HarpCore::send_harp_reply(EVENT, APP_REG_START_ADDRESS + address_offset,
                                  reg_specs.base_ptr, reg_specs.num_bytes,
                                  reg_specs.payload_type);
    }
}

void reset_app()
{
    app_regs.sensor_dispatch_frequency_hz = 0;
    dispatch_interval_us = 0;
    app_regs.brake_current_setpoint = 0;
    brake_setpoint.write_value(app_regs.brake_current_setpoint);
    // Clear torque and brake current offsets.
    torque_offset = 0;
    brake_current_offset = 0;
    // Zero encoder by saving current position as offset.
    encoder_offset = encoder.get_count();
    encoder.request_count(); // Enter update loop by first requesting encoder count.
}

// Create Core.
HarpCApp& app = HarpCApp::init(who_am_i, hw_version_major, hw_version_minor,
                               assembly_version,
                               harp_version_major, harp_version_minor,
                               fw_version_major, fw_version_minor,
                               serial_number, "Harp.Device.Treadmill",
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
    HarpSynchronizer::init(uart0, HARP_SYNC_RX_PIN);
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
    reset_app();
    while(true)
        app.run();
}
