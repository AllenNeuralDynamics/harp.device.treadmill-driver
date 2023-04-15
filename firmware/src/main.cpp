#include <pico/stdlib.h>
#include <cstring>
#include <cstdio> // for printf
#include <pio_encoder.h>
#include <analog_load_cell.h>
#include <config.h>
/*
#include <harp_core.h>
#include <harp_synchronizer.h>

// Create device name array.
const uint16_t who_am_i = 1234;
const uint8_t hw_version_major = 1;
const uint8_t hw_version_minor = 0;
const uint8_t assembly_version = 2;
const uint8_t harp_version_major = 2;
const uint8_t harp_version_minor = 0;
const uint8_t fw_version_major = 3;
const uint8_t fw_version_minor = 0;

// Create Core and synchronizer.
HarpCore& core = HarpCore::init(who_am_i, hw_version_major, hw_version_minor,
                                assembly_version,
                                harp_version_major, harp_version_minor,
                                fw_version_major, fw_version_minor,
                                "Harp Treadmill");
*/

// Core0 main.
int main()
{
    uint32_t count, torque;
    stdio_usb_init(); // TODO: remove this when we add back harp core.

    // Init PIO encoder on pins 0 and 1.
    PIOEncoder encoder(0, 0, ENCODER_BASE_PIN);
    AnalogLoadCell torque_load(TORQUE_LOAD_ADC_PIN);

    while(true)
    {
        //core.run();
        // Update State. Handle events.
        // Read encoder inputs.
        count = encoder.get_count();
        torque = torque_load.read_raw();
        printf("encoder: %09lu | torque: %09lu\r", count, torque);
        sleep_ms(17); // ~60[Hz] refresh rate.
    }
    return 0;
}
