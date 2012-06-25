#include "analog.h"

int16_t analog_read(uint8_t channel)
{
		static int16_t analog_sum = 0;
	
		//ADMUX = (ADMUX & 0xE0) | channel;
		ADMUX = channel;
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

