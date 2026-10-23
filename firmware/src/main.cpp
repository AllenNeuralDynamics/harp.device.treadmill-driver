#include <pico/stdlib.h>
#include <cstring>
#include <pio_encoder.h>
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

// PIO will periodically write raw values to this location.
volatile uint32_t __not_in_flash("encoder_raw") encoder_raw;

// offset --> measurement taken at requested time.
int32_t __not_in_flash("encoder_offset") encoder_offset;

inline uint32_t get_tared_encoder_ticks()
{ return encoder_raw - encoder_offset;}

// Note: the brake and torque sensor hardware is not present in this build.
// The corresponding registers are retained (and read as 0 or store written
// values) to keep the register map compatible with the original
// treadmill-driver device.
#pragma pack(push, 1)
struct app_regs_t
{
    int32_t encoder_ticks;   // 32.
    int16_t reaction_torque;  // 33. Unused. Always reads 0.
    int16_t brake_current;    // 34. Unused. Always reads 0.
    int32_t sensors[3]; // aggregate vector of the above 3 three registers:
                         // [position, uint32(torque), uint32(current)]
                         // torque and current are always 0.
    uint16_t sensor_dispatch_frequency_hz;  // 36
    uint16_t brake_current_setpoint;    // 37. Unused. Stores written value.
                                        // Writes return a WRITE_ERROR if
                                        // torque_limiting_triggered is set.
    uint8_t tare;       // {unused[7:3], brake_current[2], torque[1], encoder[0]}
                        // Only the encoder bit has an effect.
    uint8_t reset_tare; // {unused[7:3], brake_current[2], torque[1], encoder[0]}
                        // Only the encoder bit has an effect.
    uint8_t torque_limiting;    // Unused. Stores written value.
    uint8_t torque_limiting_triggered; // Unused. Never set by the device.
                                       // Stores written value.
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
    // No brake hardware. Store the value so it reads back, but preserve the
    // original WRITE_ERROR behavior if the torque limit state is set.
    if (app_regs.torque_limiting_triggered)
    {
        HarpCore::send_harp_reply(WRITE_ERROR, msg.header.address);
        return;
    }
    HarpCore::copy_msg_payload_to_register(msg);
    HarpCore::send_harp_reply(WRITE, msg.header.address);
}

void write_tare(msg_t& msg)
{
    HarpCore::copy_msg_payload_to_register(msg);
    // Handle bits to apply tare value. Torque and brake current bits are
    // accepted but have no effect.
    if (1u << 0 & app_regs.tare) // Zero encoder
        encoder_offset = encoder_raw;
    HarpCore::send_harp_reply(WRITE, msg.header.address);
}

void write_reset_tare(msg_t& msg)
{
    HarpCore::copy_msg_payload_to_register(msg);
    // Handle bits to clear tare value. Torque and brake current bits are
    // accepted but have no effect.
    if (1u << 0 & app_regs.tare) // Reset encoder to native value.
    {
        encoder_offset = 0;
        app_regs.tare &= ~(1u << 0); // Also clear tare setting in tare register.
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
    app_regs.reaction_torque = 0;
    HarpCore::send_harp_reply(READ, reg_name);
}

void read_reg_brake_current(uint8_t reg_name)
{
    app_regs.brake_current = 0;
    HarpCore::send_harp_reply(READ, reg_name);
}

void update_sensor_register()
{
    app_regs.sensors[0] = get_tared_encoder_ticks();
    app_regs.sensors[1] = 0; // No torque sensor.
    app_regs.sensors[2] = 0; // No brake current sensor.
}

void read_reg_sensors(uint8_t reg_name)
{
    update_sensor_register();
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
    {&HarpCore::read_reg_generic, &write_tare},
    {&HarpCore::read_reg_generic, &write_reset_tare},
    {&HarpCore::read_reg_generic, &HarpCore::write_reg_generic},
    {&HarpCore::read_reg_generic, &HarpCore::write_reg_generic}
    // More handler function pairs here if we add additional registers.
};

void update_app_state()
{
    uint32_t curr_time_us = time_us_32();
    // Update encoder count.
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
    // Zero encoder by saving current position as offset.
    encoder_offset = encoder.get_count();
    encoder.request_count(); // Enter update loop by first requesting encoder count.
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
    reset_app(); // Apply app register starting values.
    while(true)
        app.run();
}
