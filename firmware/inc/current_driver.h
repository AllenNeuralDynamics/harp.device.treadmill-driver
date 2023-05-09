#ifndef CURRENT_DRIVER_H
#define CURRENT_DRIVER_H
#include <stdint.h>
#include <pico/stdlib.h>
#include <hardware/pwm.h>

#define CURRENT_DRIVER_PWM_FREQUENCY_HZ 30000

#define SYSTEM_CLOCK_HZ 125000000UL

class CurrentDriver
{
public:
/**
 \brief Constructor where only pins are specified.
 \note This constructor will setup 2 DMA channels to poll the current sensing
    pin such that analog measurements can me made without blocking.
 */
//    CurrentDriver(uint8_t current_sensing_pin, uint8_t torque_pwm_pin);

/**
 \brief Constructor where a pointer to the latest current measurement is
    specified.
 \note Setup such that this pointer contains the latest analog measurement must
    be done elsewhere.
FIXME: make the current measurement address 16 bit and indicate the full scale range value.
FIXME: make the DMA data 16 bit in size.
 */
    CurrentDriver(uint8_t* current_measurement, uint8_t torque_pwm_pin);

/**
 \brief destructor
 */
    ~CurrentDriver();


    /**
     * \brief write the latest duty cycle to the pwm output
     * \note inline
     */
    void update_output(void)
    {pwm_set_chan_level(slice_num_, gpio_channel_, duty_cycle_);}

    /**
     * \brief enable the pwm output
     * \note inline
     */
    void disable_output(void)
        {pwm_set_chan_level(slice_num_, gpio_channel_, 0);}

    void set_duty_cycle(uint8_t duty_cycle_percentage);

    void set_pwm_frequency(uint32_t freq_hz);

private:
    uint slice_num_;
    uint torque_pwm_pin_;

    uint gpio_channel_; // The pwm gpio channel for the torque_pwm_pin_
    uint duty_cycle_; // The current (i.e: active) duty cycle setting.

    // Constants
    static const uint SYSTEM_CLOCK = 125000000;

    static const uint PWM_STEP_INCREMENTS = 100;
    static const uint DEFAULT_PWM_FREQUENCY_HZ = 20000; // Just beyond human hearing.

    // PWM Frequency range bounds.
    static const uint DIVIDER_MIN_FREQ_HZ = 5000;
    static const uint DRIVER_MAX_FREQ_HZ = 500000;
};
#endif // CURRENT_DRIVER_H
