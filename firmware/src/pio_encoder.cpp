#include <pio_encoder.h>


PIOEncoder::PIOEncoder(PIO pio, uint32_t state_machine_id,
                       uint8_t ab_base_pin)
:pio_{pio}, sm_{state_machine_id}, pin_a_{ab_base_pin}, pin_b_{ab_base_pin + 1}
{
    // TODO: ensure state of PIO hardware is compatible with this program.
    // i.e: FIFO has not been joined, state machine is unused, etc.
    // This program must be loaded at offset 0.
    pio_add_program_at_offset(pio_, &quadrature_encoder_program, 0);
    quadrature_encoder_program_init(pio_, sm_, 0, ab_base_pin, 0);
}

PIOEncoder::~PIOEncoder()
{
    pio_remove_program(pio_, &quadrature_encoder_program, 0);
    // TODO: unit GPIO pins?
}

void PIOEncoder::request_count()
{
    return quadrature_encoder_request_count(pio_, sm_);
}

uint32_t PIOEncoder::fetch_count()
{
    return quadrature_encoder_fetch_count(pio_, sm_);
}

uint32_t PIOEncoder::get_count()
{
    return quadrature_encoder_get_count(pio_, sm_);
}

