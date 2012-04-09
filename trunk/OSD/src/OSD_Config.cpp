#include <avr/eeprom.h>
#include <util/delay.h>
#include <avr/wdt.h>
#include <string.h>

#include "Config.h"
#include "ArduOSD.h"
#include "time.h"
#include "uart.h"


/* ******************************************************************/
/* *********************** GENERAL FUNCTIONS ********************** */

//PanelConfig PanelConfig, PanelB, PanelC;
byte mark_EE EEMEM;
byte verison_EE EEMEM;
osd_setting_t setting_EE EEMEM;

#define _bv(x) (1UL << (x))

osd_setting_t ArduOSD::setting =
{
    _bv(OSD_ITEM_Pit) | _bv(OSD_ITEM_Rol) | _bv(OSD_ITEM_BatA) | _bv(OSD_ITEM_GPSats) | _bv(OSD_ITEM_GPL) | _bv(OSD_ITEM_GPS)
    | _bv(OSD_ITEM_Rose) | _bv(OSD_ITEM_Head) | _bv(OSD_ITEM_MavB)| _bv(OSD_ITEM_HDir) | _bv(OSD_ITEM_HDis)
    | _bv(OSD_ITEM_Alt) | _bv(OSD_ITEM_Vel) | _bv(OSD_ITEM_Thr) | _bv(OSD_ITEM_FMod) | _bv(OSD_ITEM_Hor) | _bv(OSD_ITEM_SYS),


    {   {13, 7}, 	// panCenter_y_ADDR
        {22, 9}, 	// panPitch_y_ADDR
        {11, 1}, 	// panRoll_y_ADDR
        {21, 1}, 	// panBatt_A_y_ADDR
        {21, 3}, 	// panBatt_B_y_ADDR
        {2, 13}, 	// panGPSats_y_ADDR
        {5, 13}, 	// panGPL_y_ADDR
        {2, 14}, 	// panGPS_y_ADDR

        {16, 14}, // panRose_y_ADDR
        {24, 13}, // panHeading_y_ADDR
        {2, 9}, 	// panMavBeat_y_ADDR
        {14, 3}, 	// panHomeDir_y_ADDR
        {2, 1}, 	// panHomeDis_y_ADDR
        {0, 0}, 	// panWPDir_y_ADDR
        {0, 0}, 	// panWPDis_y_ADDR
        {21, 5},	// panRSSI_y_ADDR

        {21, 2}, //  panCur_A_y_ADDR
        {21, 4}, //  panCur_B_y_ADDR
        {2, 2}, //  panAlt_y_ADDR
        {2, 3}, //  panVel_y_ADDR
        {2, 4}, //  panThr_y_ADDR
        {17, 13}, // panFMod_y_ADDR
        {8, 7}, //  panHorizon_y_ADDR}
        {11, 4}, // sys status
    }
};

void ArduOSD::init()
{
    OSD::init();
    	
    init_analog();
    	
    panLogo();

    if (eeprom_read_byte(&mark_EE) != 'O' || eeprom_read_byte(&verison_EE) != VER)
    {
        setPanel(6,9);
        openPanel();
        print_P(PSTR("Missing/Old Config"));
        closePanel();

        //loadBar();
        delay(500);

        eeprom_write_byte(&mark_EE, 'O');
        eeprom_write_byte(&verison_EE, VER);
        eeprom_write_block((const void*) &setting, &setting_EE, sizeof(setting));
        //writeSettings();

        setPanel(6,9);
        openPanel();
        print_P(PSTR("OSD Initialized   "));
        closePanel();

        delay(500);
    }
    else
    {
        eeprom_read_block(&setting, &setting_EE, sizeof(setting));
    }

    for (int j = 0; j < 24; j++)
    {
        if(setting.coord[j][1] >= getCenter() && getMode() == 0)
        {
            setting.coord[j][1] -= 3;//Cutting lines offset after center if NTSC
        }
    }

    loadBar();
    delay(500);
    clear();
}



void ArduOSD::uploadFont()
{
    //uint16_t byte_count = 0;
    //uint8_t bit_count;
    //uint8_t ascii_binary[0x08];

    // move these local to prevent ram usage
    uint8_t character_bitmap[0x40];
    uint16_t font_count = 0;
    uint8_t checksum = 0;
    bool error = false;

    clear();
    setPanel(6,9);
    openPanel();
    print_P(PSTR("Update CharSet"));
    closePanel();

    uart_putc('F');
    
    while(font_count < 256)
    {
        //ack = '@';
        //loop until uart available
        checksum = 0;
       
        for (uint8_t i = 0; i < 54; i++)
        {
        		while (!uart_available())
        			wdt_reset();
            uint8_t b = (uint8_t)uart_getc();
            character_bitmap[i] = b;
            checksum += b;
        }
        //wdt_reset();

				while (!uart_available())
					wdt_reset();
        uint8_t ck = (uint8_t)uart_getc();

        if (ck == checksum)
        {
            write_NVM(font_count, character_bitmap);
            uart_putc('!');
            font_count++;
        }
        else
        {
            uart_putc('@');
            error=true;
            break;
        }
    }

    clear();
}

void ArduOSD::getSetting()
{
    uart_putc('s');
    uart_putc((uint8_t)sizeof(setting));
    uint8_t checksum = 0;
    for (uint8_t* p = (uint8_t*)&setting; p < (uint8_t*)&setting + sizeof(setting); p++)
    {
        uint8_t c = *p;
        uart_putc(c);
        checksum += c;
    }
    uart_putc(checksum);
}

void ArduOSD::uploadSetting()
{
    uart_putc('S');
    uint8_t size = uart_wait_getc();
    wdt_reset();
    uint8_t ack = '@';
    if (size == sizeof(setting))
    {
        uint8_t buf[sizeof(setting)];
        uint8_t checksum = 0;

        for (uint8_t* p = buf; p < buf+sizeof(setting) ; p++)
        {
            uint8_t c = uart_wait_getc();
            *p = c;
            checksum += c;
            wdt_reset();
        }
        uint8_t ck = uart_wait_getc();

        wdt_reset();
        if (ck == checksum)
        {
            memcpy(&setting, buf, sizeof(setting));
            wdt_reset();
            eeprom_write_block((const void*) &setting, &setting_EE, sizeof(setting));
            wdt_reset();
            ack = '!';
            clear();
        }
    }
    uart_putc(ack);
}

void ArduOSD::reboot()
{
    clear();
    panLogo();
    setPanel(6,9);
    openPanel();
    print_P(PSTR("Rebooting..."));
    closePanel();
    for (uint8_t i = 0; i < 10; i++)
    {
        delay(100);
        wdt_reset();
    }

    while(1);
}