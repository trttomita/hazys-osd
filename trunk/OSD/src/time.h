#ifndef TIME_H
#define TIME_H

#include <avr/io.h>

#ifdef __cplusplus
extern "C" {
#endif

    void delay(unsigned long ms);

    void delayMicroseconds(unsigned int us);

    unsigned long millis();

    inline void timer_init()
    {
        // set timer 0 prescale factor to 64
        TCCR0 = _BV(CS01) | _BV(CS00);
        // enable timer 0 overflow interrupt
        TIMSK |= _BV(TOIE0);
    }

#ifdef __cplusplus
}
#endif

#endif //TIME_H