#include <pico/stdlib.h>
#include <cstring>
#include <pio_encoder.h>
#include <analog_load_cell.h>
#include <continuous_adc_polling.h>
#include <config.h>
#include <harp_message.h>
#include <harp_core.h>
#include <harp_c_app.h>
#include <harp_synchronizer.h>
#ifdef DEBUG
    #include <cstdio> // for printf
#endif

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
const size_t reg_count = 1;

#pragma pack(push, 1)
struct app_regs_t
{
    volatile uint32_t encoder_raw;
    volatile int16_t brake_current_raw;
    volatile int16_t brake_current_setpoint;
    volatile uint16_t torque_midpoint;
    volatile uint16_t torque_raw;
    volatile uint16_t torque_setpoint;
    volatile uint8_t brake_enabled;
    volatile uint8_t calibrate;
    // More app "registers" here.
} app_regs;

void write_brake_current_setpoint(msg_t& msg)
{
    HarpCore::copy_msg_payload_to_register(msg);
    BrakeSetpoint.write_value(app_regs.brake_current_setpoint);
    if (!HarpCore::is_muted())
        HarpCore::send_harp_reply(WRITE, msg.header.address);
}

// Define "specs" per-register
RegSpecs app_reg_specs[reg_count]
{
    {(uint8_t)&app_regs.encoder_raw, sizeof(app_regs.encoder_raw), U32},
    {(uint8_t)&app_regs.brake_current_raw, sizeof(app_regs.brake_current_raw), U16},
    {(uint8_t)&app_regs.brake_current_setpoint, sizeof(app_regs.brake_current_setpoint), U16},
    {(uint8_t)&app_regs.torque_midpoint, sizeof(app_regs.torque_midpoint), U16},
    {(uint8_t)&app_regs.torque_raw, sizeof(app_regs.torque_raw), U16},
    {(uint8_t)&app_regs.torque_setpoint, sizeof(app_regs.torque_setpoint), U16},
    {(uint8_t)&app_regs.brake_enabled, sizeof(app_regs.brake_enabled), U8},
    {(uint8_t)&app_regs.calibrate, sizeof(app_regs.calibrate), U8}
    // More specs here if we add additional registers.
}

RegFnPair reg_handler_fns[reg_count]
{
    {&HarpCore::read_reg_generic, &HarpCore::write_to_read_only_reg_error},
    {&HarpCore::read_reg_generic, &HarpCore::write_to_read_only_reg_error},
    {&HarpCore::read_reg_generic, &write_brake_current_setpoint},
    {&HarpCore::read_reg_generic, &HarpCore::write_to_read_only_reg_error},
    {&HarpCore::read_reg_generic, &HarpCore::write_to_read_only_reg_error},
    {&HarpCore::read_reg_generic, &HarpCore::write_reg_generic},
    {&HarpCore::read_reg_generic, &HarpCore::write_reg_generic},
    {&HarpCore::read_reg_generic, &HarpCore::write_reg_generic}
    // More handler function pairs here if we add additional registers.
}

void update_app_state
{
    // Called periodically inside of app.run()
    // Nothing to do!
}

void reset_app
{
    // Called when we write to the core reset "register."
    app_regs.torque_setpoint = app_regs.torque_midpoint;
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

PIOEncoder encoder(0, 0, ENCODER_BASE_PIN);
// Create PIO SPI ADC instances for current and torque transducer sensing.
PIO_ADS70x9 CurrentSensor(pio0, 0, 12, BRAKE_CURRENT_CS_PIN,
                          BRAKE_CURRENT_SCK_PIN, BRAKE_CURRENT_POCI_PIN);

PIO_ADS70x9 ReactionTorqueSensor(pio0, 1, 12, TORQUE_TRANSDUCER_CS_PIN,
                                 TORQUE_TRANSDUCER_SCK_PIN,
                                 TORQUE_TRANSDUCER_POCI_PIN);
PIO_LTC264x BrakeSetpoint(pio1, 0,
                          BRAKE_SETPOINT_CS_PIN,
                          BRAKE_SETPOINT_SCK_PIN,
                          BRAKE_SETPOINT_POCI_PIN);

// Core0 main.
int main()
{
#ifdef DEBUG
    stdio_uart_init_full(uart1, 921600, UART_TX_PIN, -1); // use uart1 tx only.
    printf("Hello, from an RP2040!\r\n");
#endif
    // Init Synchronizer.
    HarpSynchronizer& sync = HarpSynchronizer::init(uart0, HARP_SYNC_RX_PIN);
    //app.set_visual_indicators_fn(set_led_state);
    app.set_synchronizer(&sync);
    // Init PIO encoder on pins 0 and 1.
    // TODO: Setup DMA to write encoder values to memory.
    encoder.setup_dma_stream_to_memory(&encoder_raw);
    encoder.start();
    // Init PIO-based ADC with continuous streaming to memory via DMA.
    CurrentSensor.setup_dma_stream_to_memory(&brake_current_raw, 1);
    ReactionTorqueSensor.setup_dma_stream_to_memory(&torque_raw, 1);
    // Start PIO-connected hardware.
    CurrentSensor.start();
    ReactionTorqueSensor.start();
    BrakeSetpoint.start();

    while(true)
        app.run();
}
