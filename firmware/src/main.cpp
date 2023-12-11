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
// FIXME: Somehow we need to instantiate without loading the program, but
// pointing to an existing program
PIO_ADS7049 current_sensor(pio0,
                           BRAKE_CURRENT_CS_PIN,
                           BRAKE_CURRENT_SCK_PIN, BRAKE_CURRENT_POCI_PIN);
// Use PIO_ADS7049 program already-loaded by current sensor.
PIO_ADS7049 reaction_torque_sensor(pio0, TORQUE_TRANSDUCER_CS_PIN,
                                   TORQUE_TRANSDUCER_SCK_PIN,
                                   TORQUE_TRANSDUCER_POCI_PIN,
                                   current_sensor.get_program_address());
// Create PIO SPI DAC instance for driving the brake current setpoint.
// FIXME: maybe do this over vanilla SPI.
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
const size_t reg_count = 8;

#pragma pack(push, 1)
struct app_regs_t
{
    volatile uint32_t encoder_raw;
    volatile uint16_t brake_current_raw;
    volatile uint16_t brake_current_setpoint;
    volatile uint16_t torque_midpoint;
    volatile uint16_t torque_raw;
    volatile uint16_t torque_setpoint;
    volatile uint8_t brake_enabled;
    volatile uint8_t calibrate;
    // More app "registers" here.
} app_regs;

void write_brake_current_setpoint(msg_t& msg)
{
/*
    HarpCore::copy_msg_payload_to_register(msg);
    brake_setpoint.write_value(app_regs.brake_current_setpoint);
    if (!HarpCore::is_muted())
        HarpCore::send_harp_reply(WRITE, msg.header.address);
*/
}


// Define "specs" per-register
RegSpecs app_reg_specs[reg_count]
{
    {(uint8_t*)&app_regs.encoder_raw, sizeof(app_regs.encoder_raw), U32},
    {(uint8_t*)&app_regs.brake_current_raw, sizeof(app_regs.brake_current_raw), U16},
    {(uint8_t*)&app_regs.brake_current_setpoint, sizeof(app_regs.brake_current_setpoint), U16},
    {(uint8_t*)&app_regs.torque_midpoint, sizeof(app_regs.torque_midpoint), U16},
    {(uint8_t*)&app_regs.torque_raw, sizeof(app_regs.torque_raw), U16},
    {(uint8_t*)&app_regs.torque_setpoint, sizeof(app_regs.torque_setpoint), U16},
    {(uint8_t*)&app_regs.brake_enabled, sizeof(app_regs.brake_enabled), U8},
    {(uint8_t*)&app_regs.calibrate, sizeof(app_regs.calibrate), U8}
    // More specs here if we add additional registers.
};

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
};

void update_app_state()
{
    // Update encoder count.
    // (Brake current and Transducer Torque update automatically.)
    app_regs.encoder_raw = encoder.fetch_count();
    encoder.request_count(); // request count for the next iteration.
}

void reset_app()
{
    // Called when we write to the core reset "register."
    app_regs.torque_setpoint = 0;
    // TODO: actually apply the setpoint value.
    // TODO: technically, we should reset the encoder too.
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
    //app.set_visual_indicators_fn(set_led_state);
    app.set_synchronizer(&HarpSynchronizer::instance());
    // Init PIO encoder on pins 0 and 1.
    // Init PIO-based ADC with continuous streaming to memory via DMA.
    current_sensor.setup_dma_stream_to_memory(&app_regs.brake_current_raw, 1);
    reaction_torque_sensor.setup_dma_stream_to_memory(&app_regs.torque_raw, 1);
    // Start PIO-connected hardware.
    current_sensor.start();
    reaction_torque_sensor.start();
    brake_setpoint.start();

    encoder.request_count(); // Enter loop by first requesting encoder count.
    while(true)
        app.run();
}
