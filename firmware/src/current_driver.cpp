#include <current_driver.h>


CurrentDriver::CurrentDriver(uint8_t* current_measurement,
                             uint8_t torque_pwm_pin)
:torque_pwm_pin_{torque_pwm_pin}, duty_cycle_{0}
{
// inpired by:
// https://github.com/raspberrypi/pico-examples/blob/master/pwm/hello_pwm/hello_pwm.c#L14-L29
    // Allocate torque_pwm_pin for pwm; allocate dir pin as output.
    gpio_set_function(torque_pwm_pin_, GPIO_FUNC_PWM);
    // Find out (and save) which hardware (PWM slice & channel) are connected
    // to this GPIO.
    slice_num_ = pwm_gpio_to_slice_num(torque_pwm_pin_);
    gpio_channel_ = pwm_gpio_to_channel(torque_pwm_pin_);
    // Set period of 1000 cycles (0 to 99 inclusive) (reg TOP value).
    pwm_set_wrap(slice_num_, 99);
    // Clear output duty cycle on startup.
    set_duty_cycle(0);
    set_pwm_frequency(CURRENT_DRIVER_PWM_FREQUENCY_HZ);
    // Enabling / Disabling PWM must be done by changing the duty cycle
    // and leaving the slice enabled bc disabling the slice leaves the GPIO
    // fixed in its current state.
    pwm_set_enabled(slice_num_, true);
}

CurrentDriver::~CurrentDriver()
{
    disable_output();
    pwm_set_enabled(slice_num_, false);
    // Set GPIOs to inputs.
    gpio_init_mask(1 << torque_pwm_pin_);
}

void CurrentDriver::set_duty_cycle(uint8_t duty_cycle_percentage)
{
    // Clamp output.
    if (duty_cycle_percentage > 100)
    {
        duty_cycle_percentage = 100;
    }
    // Save it for enabling/disabling.
    duty_cycle_ = duty_cycle_percentage;
}

void CurrentDriver::set_pwm_frequency(uint32_t freq_hz)
{
    // Configure for n[Hz] period broken down into PWM_STEP_INCREMEMENTS.
    // requested value must be within [0.0, 256.0]
    float new_freq_div = SYSTEM_CLOCK_HZ / freq_hz * PWM_STEP_INCREMENTS;

    // Default to 20[KHz].
    if (freq_hz < DIVIDER_MIN_FREQ_HZ || freq_hz > DRIVER_MAX_FREQ_HZ)
    {
        // Default is 62.5.
        new_freq_div = SYSTEM_CLOCK_HZ / DEFAULT_PWM_FREQUENCY_HZ * PWM_STEP_INCREMENTS;
    }
    pwm_set_clkdiv(slice_num_, new_freq_div);
}

