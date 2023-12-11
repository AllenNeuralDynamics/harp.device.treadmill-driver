#ifndef PIO_LTC264X_H
#define PIO_LTC264X_H

#include <pico/stdlib.h>
#include <hardware/dma.h>
#include <hardware/pio.h>
#include <hardware/regs/dreq.h>
#include <stdio.h>
#include <stdint.h>
#include <pio_ltc264x.pio.h> // auto-generated upon compilation.



/**
 * \brief class for the LTC264x family of SPI-based DACs using the RP2040's PIO.
 * \note this class is compatible 12, 14, and 16-bit device variants.
 * \note underlying PIO implementation uses a 31.25[MHz] SPI frequency assuming
 *  a 125[MHz] system clock.
 * \note The LTC2641's power-on reset output value is 0. The LTC2642's is
 *  midscale.
 */
class PIO_LTC264x
{
public:
/**
 * \brief constructor. Setup gpio pins, state machine.
 * \note CS pin is \p sck_pin + 1.
 */
    PIO_LTC264x(PIO pio, uint8_t sck_pin, uint8_t pico_pin);

    ~PIO_LTC264x();

/**
 * \brief Write a single value to output to the DAC.
 * \param value value to write to the DAC. If using the 12 or 14-bit device
 *  variants, value must be left-shifted such that the MSbit is bit[15].
 *  The unused LSbits can be any value and will be ignored by 12 or 14-bit
 *  device variants.
 * \warning only works if the device was not setup with either
 *  setup_dma_stream_from_memory or setup_dma_stream_from_memory_with_interrupt.
*/
    void write_value(uint16_t value);

/**
 * \brief Configure single-shot or continuous streaming of a specified number
    of values to a specified memory location at the specified interval.
*/
/*
    void setup_dma_stream_from_memory(uint16_t* starting_address,
                                      size_t sample_count,
                                      uint16_t interval,
                                      bool loop);
*/

/**
 * \brief single-shot or continuous streaming of a specified number of values
 *  to a specified memory location at 2MHz. Upon writing the specified number of
 *  values, trigger an interrupt to call the specified callback function.
*/
    // FIXME: make inline.
    // FIXME: implement pacing via pacing timer.
    // see: https://forums.raspberrypi.com/viewtopic.php?t=329200
/*
    void setup_dma_stream_from_memory_with_interrupt(uint16_t* starting_address,
                                                     size_t sample_count,
                                                     uint16_t interval,
                                                     bool loop,
                                                     int dma_irq_source,
                                                     irq_handler_t handler_func);
*/

/**
 * \brief Configure continuous streaming of a specified number of values to a
 *  specified memory location at 2MHz. Upon writing the specified number of
 *  values optionally trigger an interrupt.
 * \details Streaming occurs at the maximum data rate of the sensor (2MHz)
 *  and requires 2 DMA channels.
 * \param starting_address the starting address of the memory location to write
 *  new data to.
 * \param sample_count the number of samples to write to memory before looping
 *  back to the starting address.
 * \param interval the time between DMA transfers.
 * \param loop boolean indicating whether to restart the sequence (forever)
 *  after outputting sample_count values.
 * \param trigger_interrupt if true, fire an interrupt upon writing
 *  `sample_count` samples to memory.
 * \param dma_irq_source DMA_IRQ_0 or DMA_IRQ_1
 * \param handler_func callback function. (This function must clear the
 *  corresponding interrupt source.
 * \note: shortest interval is FIXME clock cycles.
 */
/*
    void _setup_dma_stream_from_memory(uint16_t* starting_address,
                                       size_t sample_count,
                                       uint16_t interval,
                                       bool loop,
                                       bool trigger_interrupt,
                                       int dma_irq_source,
                                       irq_handler_t handler_func);
*/

/**
 * \brief launch the pio program
 */
    void start();

    int samp_chan_; // DMA channel used to collect samples and fire an interrupt
                    // if configured to do so. If it fires an interrupt,
                    // a DMA handler function needs to clear it.
private:
    PIO pio_;
    uint sm_;
    uint16_t* data_ptr_[1];   // Data that the reconfiguration channel will write back
                            // to the sample channel. In this case, just the
                            // address of the location of the adc samples. This
                            // value must exist with global scope since the DMA
                            // reconfiguration channel will need to writes its value
                            // back to the sample channel on regular intervals.


    uint32_t dma_data_chan_;
    uint32_t dma_ctrl_chan_;
};
#endif // PIO_LTC2642_H
