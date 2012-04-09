#include "ArduOSD.h"
#include <avr/io.h>

inline static void ArduOSD::init_analog()
{
}
	
static uint16_t ArduOSD::read_analog(uint8_t channel)
{
		static uint16_t analog_sum = 0;
	
		ADMUX = (ADMUX & 0xE0) | channel;
		analog_sum = 0;
		for (uint8_t i = 0; i < 2; i++)
		{
				ADCSRA |= (1 << ADSC);
				loop_until_bit_is_set(ADCSRA, ADIF);
				analog_sum += ADC;
				ADCSRA &= ~(1 << ADIF);
		}
		return analog_sum >> 1;
}

