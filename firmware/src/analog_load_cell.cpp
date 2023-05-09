#include <analog_load_cell.h>

AnalogLoadCell::AnalogLoadCell(uint8_t adc_pin)
:adc_pin_{adc_pin}
{
    //static_assert(adc_pin >= ADC_BASE_PIN);
    adc_gpio_init(adc_pin_);
    adc_init();
    adc_select_input(adc_pin_ - ADC_BASE_PIN);
    adc_set_clkdiv(0); // run adc at full speed.
    adc_run(true); // init adc for continuous reading.
}

AnalogLoadCell::~AnalogLoadCell()
{
    // TODO: If no other resources are using the ADC (i.e: no other pins were
    // init), stop the adc. As-is: this is a bit heavy-handed.
    adc_run(false);
}

