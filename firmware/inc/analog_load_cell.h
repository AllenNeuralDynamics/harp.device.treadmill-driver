#ifndef ANALOG_LOAD_CELL_H
#define ANALOG_LOAD_CELL_H
#include <hardware/gpio.h>
#include <hardware/adc.h>

#define ADC_BASE_PIN 26


class AnalogLoadCell
{
public:
    AnalogLoadCell(uint8_t adc_pin);
    // TODO: another constructor, where the adc data simply comes from
    //  a memory address that DMA is continuously updating.
    ~AnalogLoadCell();

/**
 * \brief read the latest data. Nonblocking. Inline.
 */
    uint16_t read_raw()
    {
        //adc_select_input(adc_pin_ - ADC_BASE_PIN);
        return (uint16_t) adc_hw->result;
    }

private:
    uint8_t adc_pin_;

};
#endif //ANALOG_LOAD_CELL_H
