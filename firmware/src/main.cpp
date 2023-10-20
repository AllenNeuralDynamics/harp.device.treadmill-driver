#include <pico/stdlib.h>
#include <cstring>
#include <cstdio> // for printf
#include <pio_encoder.h>
#include <analog_load_cell.h>
#include <continuous_adc_polling.h>
#include <config.h>
#include <harp_message.h>
#include <harp_core.h>
#include <harp_c_app.h>
#include <harp_synchronizer.h>

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
    volatile uint16_t torque_midpoint;
    volatile uint16_t torque_raw;
    volatile uint16_t torque_setpoint;
    volatile uint8_t brake_enabled;
    // More app "registers" here.
} app_regs;

// Define "specs" per-register
RegSpecs app_reg_specs[reg_count]
{
    {(uint8_t)&app_regs.encoder_raw, sizeof(app_regs.encoder_raw), U32},
    {(uint8_t)&app_regs.torque_midpoint, sizeof(app_regs.torque_midpoint), U16},
    {(uint8_t)&app_regs.torque_raw, sizeof(app_regs.torque_raw), U16},
    {(uint8_t)&app_regs.torque_setpoint, sizeof(app_regs.torque_setpoint), U16},
    {(uint8_t)&app_regs.brake_enabled, sizeof(app_regs.brake_enabled), U8}
    // More specs here if we add additional registers.
}

RegFnPair reg_handler_fns[reg_count]
{

    {&HarpCore::read_reg_generic, &HarpCore::write_to_read_only_reg_error},
    {&HarpCore::read_reg_generic, &HarpCore::write_to_read_only_reg_error},
    {&HarpCore::read_reg_generic, &HarpCore::write_to_read_only_reg_error},
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
                               serial_number, "Harp Treadmill",
                               &app_regs, app_reg_specs,
                               reg_handler_fns, reg_count, update_app_state,
                               reset_app);

// Create ADs70x9 instance for the ADS7049.
PIO_ADS70x9 ads7049(pio0, 0, 12, ADS7049_CS_PIN, ADS7049_SCK_PIN,
                    ADS7049_POCI_PIN);

// Core0 main.
int main()
{
#ifdef DEBUG
    stdio_uart_init_full(uart1, 921600, 4, -1); // use uart1 tx only.
    printf("Hello, from an RP2040!\r\n");
#endif
    // Init Synchronizer.
    HarpSynchronizer& sync = HarpSynchronizer::init(uart0, HARP_SYNC_RX_PIN);
    // Init PIO encoder on pins 0 and 1.
    // TODO: Setup DMA to write PIO to memory.
    PIOEncoder encoder(0, 0, ENCODER_BASE_PIN);
    //encoder.setup_dma_stream_to_memory(&encoder_raw, 1);
    //encoder.start();

    // Setup DMA channels to continuously stream ADC data to memory.
    //setup_continuous_adc_polling();
    // Init PIO-based ADC with continuous streaming to memory via DMA..
    ads7049.setup_dma_stream_to_memory(&torque_raw, 1);
    ads7049.start();

    // Init Torque Load Controller.
    //CurrentDriver(&adc_vals[ADC_INDEX], TORQUE_PWM_PIN);

    while(true)
    {
        app.run();
    }
}
