#include <pio_encoder.h>


PIOEncoder::PIOEncoder(uint8_t pio_index, uint32_t state_machine_id,
                       uint8_t ab_base_pin)
:sm_{state_machine_id}, pin_a_{ab_base_pin}, pin_b_{ab_base_pin + 1}
{
    // TODO: ensure state of PIO hardware is compatible with this program.
    // i.e: FIFO has not been joined, state machine is unused, etc.
    pio_ = (pio_index == 0)? pio0: pio1;
    offset_ = pio_add_program(pio_, &quadrature_encoder_program);
    quadrature_encoder_program_init(pio_, sm_, offset_, ab_base_pin, 0);
}

PIOEncoder::~PIOEncoder()
{
    pio_remove_program(pio_, &quadrature_encoder_program, offset_);
    // TODO: unit GPIO pins?
}


uint32_t PIOEncoder::get_count()
{
    return quadrature_encoder_get_count(pio_, sm_);
}

