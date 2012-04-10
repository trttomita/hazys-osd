#ifndef ANALOG_H
#define ANALOG_H

#include <avr/io.h>
#include <stdint.h>


#ifdef __cplusplus
extern "C" {
#endif

inline void analog_init()
{
		ADMUX = 0;
		ADCSRA = (1 << ADEN) | (1 << ADPS2) | (1 << ADPS1) | (1 << ADPS0);
}

int16_t analog_read(uint8_t channel);


#ifdef __cplusplus
}
#endif

#endif