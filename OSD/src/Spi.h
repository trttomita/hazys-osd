#ifndef Spi_h
#define Spi_h

#include <avr/io.h>

#define DDR_SPI DDRB

#define DD_SS		DDB4
#define DD_MOSI DDB5
#define DD_MISO	DDB6
#define DD_SCK	DDB7


#ifdef __cplusplus
extern "C" {
#endif

    typedef unsigned char byte;


    inline void spi_init()
    {
        DDR_SPI = _BV(DD_MOSI) | _BV(DD_SCK) | _BV(DD_SS);

        SPCR = _BV(SPE) | _BV(MSTR) | _BV(SPR0);	// enable SPI Master, MSB, SPI mode 0, FOSC/4

        //DDRB = (1<<PB4)|(1<<PB5) | (1<<PB7);	 // Set MOSI , SCK , and SS output
        //SPCR = ( (1<<SPE)|(1<<MSTR) | (1<<SPR1) |(1<<SPR0));	// Enable SPI, Master, set clock rate fck/128

    }

    byte spi_transfer(byte value);


#ifdef __cplusplus
}
#endif
#endif
