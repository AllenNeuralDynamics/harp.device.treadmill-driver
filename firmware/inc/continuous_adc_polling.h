#ifdef __cplusplus
extern "C" {
#endif

#include <pico/stdlib.h>
#include <hardware/adc.h>
#include <hardware/dma.h>
#include <hardware/regs/dreq.h>
#include <stdio.h>
#include <stdint.h>

// Demo to continuously sample all ADC inputs and write them to a location in
// memory where they can be read. Datasheet calls this data "scattering."

// Note: According to the datasheet sec 2.5.1, DMA read and write addresses must
//  be pointers to an address.
// Note: According to the datasheet sec 2.5.1.1, the way to reinitialize a
//  channel with an incrementing (read or write) address would be to rewrite the
//  starting address before (or upon) restart.
//  Otherwise, "If READ_ADDR and WRITE_ADDR are not reprogrammed, the DMA will
//  use the current values as start addresses for the next transfer."

extern uint8_t adc_vals[5]; // Array containing the latest ADC channel data.
extern uint8_t* data_ptr[1]; // Pointer to an address is required for
                      // the reinitialization DMA channel.
                      // Recall that DMA channels are basically
                      // operating on arrays of data moving their
                      // contents between locations.

void setup_continuous_adc_polling();

#ifdef __cplusplus
}
#endif
