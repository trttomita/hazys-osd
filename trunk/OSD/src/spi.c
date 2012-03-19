#include "spi.h"

byte spi_transfer(byte value)
{
    SPDR = value;
    loop_until_bit_is_set(SPSR, SPIF);
    return SPDR;
}