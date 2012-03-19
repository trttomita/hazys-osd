/*

 Copyright (c) 2011.  All rights reserved.
 An Open Source Arduino based OSD and Camera Control project.

 Program  : ArduCAM-OSD (Supports the variant: minimOSD)
 Version  : V1.9, 14 February 2012
 Author(s): Sandro Benigno
 Coauthor(s):
   Jani Hirvinen   (All the EEPROM routines)
   Michael Oborne  (OSD Configutator)
   Mike Smith      (BetterStream and Fast Serial libraries)
 Special Contribuitor:
   Andrew Tridgell by all the support on MAVLink
   Doug Weibel by his great orientation since the start of this project
 Contributors: James Goppert, Max Levine
               and all other members of DIY Drones Dev team
 Thanks to: Chris Anderson, Jordi Munoz


 This program is free software: you can redistribute it and/or modify
 it under the terms of the GNU General Public License as published by
 the Free Software Foundation, either version 3 of the License, or
 (at your option) any later version.

 This program is distributed in the hope that it will be useful,
 but WITHOUT ANY WARRANTY; without even the implied warranty of
 MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
 GNU General Public License for more details.

 You should have received a copy of the GNU General Public License
 along with this program. If not, see <http://www.gnu.org/licenses/>

*/

/* ************************************************************ */
/* **************** MAIN PROGRAM - MODULES ******************** */
/* ************************************************************ */

/*
#undef PROGMEM
#define PROGMEM __attribute__(( section(".progmem.data") ))

#undef PSTR
#define PSTR(s) (__extension__({static prog_char __c[] PROGMEM = (s); &__c[0];}))
*/

//#define MAVLINK10

/* **********************************************/
/* ***************** INCLUDES *******************/

//#define membug
//#define FORCEINIT  // You should never use this unless you know what you are doing


// AVR Includes
#include <avr/io.h>
#include <inttypes.h>
#include <avr/pgmspace.h>
#include <avr/interrupt.h>
#include <avr/wdt.h>
#include <util/delay.h>
#include <math.h>
#include <stdlib.h>

//#ifdef membug
#include "MemoryFree.h"
//#endif

// Configurations
//#include "OSD_Config.h"
#include "Config.h"
#include "Max7456.h"
#include "ArduOSD.h"

#include "spi.h"
#include "uart.h"
#include "time.h"


/* **********************************************/
/* ***************** SETUP() *******************/

void setup()
{
    cli();

    //PORTD = 0xFF;

    DDRD = _BV(DDD1);	//Tx Out, Rx In

    timer_init();
    uart_init(UART_BAUD_SELECT_DOUBLE_SPEED(TELEMETRY_SPEED, F_CPU));
    spi_init();

    sei();

    ArduOSD::init();

    delay(500);

    wdt_enable(WDTO_500MS);


} // END of setup();



/* ***********************************************/
/* ***************** MAIN LOOP *******************/

// Mother of all happenings, The loop()
// As simple as possible.
void loop()
{
    static unsigned long lasttime  = 0;

    wdt_reset();
    ArduOSD::read_data();

    unsigned long now = millis();
    if (now - lasttime > 120)
    {
        ArduOSD::refresh();
        lasttime = now;
    }

}

int main()
{
    setup();
    while(1)
        loop();
    return 0;
}

