#include <avr/wdt.h>
#include <avr/eeprom.h>
#include <avr/interrupt.h>
#include <math.h>
#include <string.h>
#include <stdlib.h>

#include "ArduOSD.h"
#include "Config.h"
#include "time.h"
#include "uart.h"
#include "spi.h"
#include "analog.h"
#include "MemoryFree.h"


#define ToDeg(x) 	(x*57.2957795131)  // *180/pi
#define _bv(x) 		(1UL << (x))

volatile uint8_t 	ArduOSD::crlf_count;


osd_setting_t ArduOSD::setting =
{
    _bv(OSD_ITEM_Pit) | _bv(OSD_ITEM_Rol) | _bv(OSD_ITEM_VBatA) | _bv(OSD_ITEM_GPSats) | _bv(OSD_ITEM_GPL) | _bv(OSD_ITEM_GPS)
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
        {22, 5},	// panRSSI_y_ADDR

        {21, 2},  // panCur_A_y_ADDR
        {21, 4},  // panCur_B_R
        {2, 2},   // panAlt_y_ADDR
        {2, 3},   // panVel_y_ADDR
        {2, 4},   // panThr_y_ADDR
        {17, 13}, // panFMod_y_ADDR
        {8, 7},   // panHorizon_y_ADDR}
        {11, 4},  // sys status
    }
};

uint8_t mark_EE EEMEM;
uint8_t verison_EE EEMEM;
osd_setting_t setting_EE EEMEM;

void ArduOSD::Init()
{
    cli();

    //PORTD = 0xFF;

    DDRD = _BV(DDD1);	//Tx Out, Rx In

    timer_init();
    uart_init(UART_BAUD_SELECT_DOUBLE_SPEED(TELEMETRY_SPEED, F_CPU));
    spi_init();
    analog_init();

    sei();

    OSD::init();
    DrawLogo();
    LoadSetting();
    DrawLoadBar();

    delay(500);
    clear();

    wdt_enable(WDTO_2S);
}

void ArduOSD::LoadSetting()
{
    if (eeprom_read_byte(&mark_EE) != 'O' || eeprom_read_byte(&verison_EE) != DATA_VER)
    {
        setPanel(6,9);
        openPanel();
        print_P(PSTR("Missing/Old Config"));
        closePanel();

        //loadBar();
        delay(500);

        eeprom_write_byte(&mark_EE, 'O');
        eeprom_write_byte(&verison_EE, DATA_VER);
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

    if (getMode() == MAX7456_MODE_NTCS)
        for (int j = 0; j < 24; j++)
        {
            if(setting.coord[j][1] >= getCenter())
                setting.coord[j][1] -= 3;//Cutting lines offset after center if NTSC
        }
}

void ArduOSD::UploadFont()
{
    uint8_t character_bitmap[0x40];
    uint16_t font_count = 0;
    uint8_t checksum = 0;
    bool error = false;

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

void ArduOSD::GetSetting()
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

void ArduOSD::GetAnalog()
{
    uart_putc('a');
    uint8_t channel = uart_wait_getc();
    wdt_reset();
    int16_t read = ::analog_read(channel);
    uint8_t checksum = (read >> 8) + (read & 0xff);
    uart_putc(read >> 8);
    uart_putc(read);
    uart_putc(checksum);
}

void ArduOSD::UploadSetting()
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

void ArduOSD::Reboot()
{
    uart_putc('R');
    clear();
    DrawLogo();
    setPanel(6,9);
    openPanel();
    print_P(PSTR("Rebooting..."));
    closePanel();
    /*for (uint8_t i = 0; i < 10; i++)
    {
        delay(100);
        wdt_reset();
    }*/

    while(1);
}

/******* STARTUP PANEL *******/

const char FM_STAB[] PROGMEM = "stab";
const char FM_ACRO[] PROGMEM = "acro";
const char FM_ALTH[] PROGMEM = "alth";
const char FM_AUTO[] PROGMEM = "auto";
const char FM_GUID[] PROGMEM = "guid";
const char FM_LOIT[] PROGMEM = "loit";
const char FM_RETL[] PROGMEM = "retl";
const char FM_CIRC[] PROGMEM = "circ";
const char FM_POSI[] PROGMEM = "posi";
const char FM_OFLO[] PROGMEM = "oflo";
const char FM_MANU[] PROGMEM = "manu";
const char FM_FBWA[] PROGMEM = "fbwa";
const char FM_FBWB[] PROGMEM = "fbwb";
const char FM_LAND[] PROGMEM = "land";

const char* FM_ACM[] = {FM_STAB, FM_STAB, FM_ALTH, FM_AUTO, FM_GUID, FM_LOIT, FM_RETL, FM_CIRC, FM_POSI, FM_LAND, FM_OFLO};
const char* FM_APM[] = {FM_MANU, FM_CIRC, FM_STAB, FM_STAB, FM_STAB, FM_FBWA, FM_FBWB, FM_STAB, FM_STAB, FM_STAB, FM_AUTO, FM_RETL, FM_LOIT};

char buf_show[12] = {0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0};

const char buf_Rule[36] = {0xc2,0xc0,0xc0,0xc1,0xc0,0xc0,0xc1,0xc0,0xc0,
                           0xc4,0xc0,0xc0,0xc1,0xc0,0xc0,0xc1,0xc0,0xc0,
                           0xc3,0xc0,0xc0,0xc1,0xc0,0xc0,0xc1,0xc0,0xc0,
                           0xc5,0xc0,0xc0,0xc1,0xc0,0xc0,0xc1,0xc0,0xc0
                          };

//------------------ Panel: Startup ArduCam OSD LOGO -------------------------------

void ArduOSD::DrawLogo(/*int first_col, int first_line*/)
{
    setPanel(7, 5);
    openPanel();
    //print_P(PSTR("\x20\x20\x20\xba\xbb\xbc\xbd\xbe|\x20\x20\x20\xca\xcb\xcc\xcd\xce|Hazys OSD"));
    print_P(PSTR("Hazys OSD v" OSD_VER "| for MAVLink 1.0"));
    closePanel();
}


/* **************************************************************** */
// Panel  : panFlightMode
// Needs  : X, Y locations
// Output : 2 symbols, one static name symbol and another that changes by flight modes
// Size   : 1 x 2  (rows x chars)
// Status : done

#ifndef MAVLINK10
inline const char* getFlightModeString(uint8_t apm_mav_type, uint8_t osd_nav_mode, uint16_t osd_mode)
{
    const char* str = NULL;

    if(apm_mav_type == 2) //ArduCopter MultiRotor or ArduCopter Heli
    {
        switch(osd_mode)
        {
        case 100:
            //str= FM_STAB;
            //break;
            return FM_STAB;
        case 101:
            return FM_ACRO;
            //break;
        case 102:
            return FM_ALTH;
            //break;
        case MAV_MODE_AUTO:
            switch (osd_nav_mode)
            {
            case MAV_NAV_WAYPOINT:
                return FM_AUTO;
                //break;
            case MAV_NAV_HOLD:
                return FM_LOIT;
                //break;
            case MAV_NAV_RETURNING:
                return FM_RETL;
                //break;
            }
            break;
        case MAV_MODE_GUIDED:
            if (osd_nav_mode == MAV_NAV_WAYPOINT) return FM_GUID;
        case 107:
            return FM_CIRC;

        case 108:
            return FM_POSI;

        case 109:
            return FM_LAND;

        case 110:
            return FM_OFLO;

        }
    }
    else if(apm_mav_type == 1) //ArduPlane
    {

        if(osd_mode == MAV_MODE_TEST1 && osd_nav_mode == MAV_NAV_VECTOR) return FM_STAB;//Stabilize
        else if(osd_mode == MAV_MODE_TEST2)
        {
            if (osd_nav_mode == 1) return FM_FBWA;//FLY_BY_WIRE_A
            else if(osd_nav_mode == 2) return FM_FBWB;//FLY_BY_WIRE_B
        }
        else if(osd_mode == MAV_MODE_MANUAL && osd_nav_mode == MAV_NAV_VECTOR) return FM_MANU;//Manual
        else if(osd_mode == MAV_MODE_AUTO)
        {
            if (osd_nav_mode == MAV_NAV_RETURNING)        			return FM_RETL;
            else if (osd_nav_mode== MAV_NAV_WAYPOINT)        				return FM_AUTO;
            else if (osd_nav_mode == MAV_NAV_LOITER) return FM_LOIT;//Loiter
        }
        else  if(osd_mode == MAV_MODE_TEST3) return FM_CIRC;//CIRCLE
    }

    return NULL;
}
#endif


inline char getBatteryPic(uint16_t bat_level)
{
    uint16_t up = 1000;
    char pic = 0xba;
    while (bat_level < up)
    {
        up -= 100;
        pic--;
    }
    return pic;
}

inline const char* getHeadingPatern(float heading)
{
    int start;
    start = round((heading * 36)/360);
    start -= 5;
    if(start < 0) start += 36;
    for(int x=0; x <= 10; x++)
    {
        buf_show[x] = buf_Rule[start];
        if(++start > 35) start = 0;
    }
    //buf_show[11] = '\0';
    return buf_show;
}

// ---------------- EXTRA FUNCTIONS ----------------------
// Show those fancy 2 char arrows
void ArduOSD::DrawArrow()
{
    uint8_t c = '\x90';
    if (osd_home_direction > 1)
        c += ((osd_home_direction-1) << 1);

    write(c);
    write(++c);
}


// Calculate and shows Artificial Horizon
inline void ArduOSD::DrawHorizon(uint8_t start_col, uint8_t start_row)
{

    int x, nose, row, minval, hit, subval = 0;
    int cols = 12;
    int rows = 5;
    int col_hit[cols];
    float  pitch, roll;

    pitch = (abs(osd_pitch) == 90)? 89.99 * (90/osd_pitch) * -0.017453293: osd_pitch * -0.017453293;
    roll = (abs(osd_roll) == 90)? 89.99 * (90/osd_roll) * 0.017453293: osd_roll * 0.017453293;

    nose = round(tan(pitch) * (rows*9));
    for(int col=1; col <= cols; col++)
    {
        x = (col * 12) - (cols * 6) - 6;//center X point at middle of each col
        col_hit[col-1] = (tan(roll) * x) + nose + (rows*9) - 1;//calculating hit point on Y plus offset to eliminate negative values
        //col_hit[(col-1)] = nose + (rows * 9);
    }

    for(int col=0; col < cols; col++)
    {
        hit = col_hit[col];
        if(hit > 0 && hit < (rows * 18))
        {
            row = rows - ((hit-1)/18);
            minval = rows*18 - row*18 + 1;
            subval = hit - minval;
            subval = round((subval*9)/18);
            if(subval == 0) subval = 1;
            openSingle(start_col + col, start_row + row - 1);
            write('\x05' + subval);
            //write('0' + start_row + row - 1);
        }
    }
}


/******* PANELS - POSITION *******/

void ArduOSD::Draw()
{
    unsigned long timeout = millis() - lastMAVBeat;

    if(timeout < 2000)
    {
        if (mavlink_status == (uint8_t)MAVLINK_STATUS_GET_DATA)
        {
            clear();
            mavlink_status = (uint8_t)MAVLINK_STATUS_UPDATE_DATA;
        }

        if (mavlink_status >= (uint8_t)MAVLINK_STATUS_GET_DATA)
        {
            for (uint8_t i = 0; i < 24; i++)
            {
                if (setting.enable & (1UL << i))
                {
                    setPanel(setting.coord[i][0], setting.coord[i][1]);
                    openPanel();

                    switch (i)
                    {
                    case OSD_ITEM_Cen:
                        print_P(PSTR("\x05\x03\x04\x05|\x15\x13\x14\x15"));
                        break;
                    case OSD_ITEM_Pit:
                        printf_P(PSTR("%4i\xb0\xb1"),osd_pitch);
                        break;
                    case OSD_ITEM_Rol:
                        printf_P(PSTR("%4i\xb0\xb2"),osd_roll);
                        break;
                    case OSD_ITEM_VBatA:
                        printf_P(PSTR(" \xE2%5.2f\x8E"), (double)osd_vbat_A);
                        break;
                    case OSD_ITEM_VBatB:
                        printf_P(PSTR(" \xE3%5.2f\x8E"), (double)osd_vbat_B);
                        break;
                    case OSD_ITEM_GPSats:
                        printf_P(PSTR("\x0f%2i"), osd_satellites_visible);
                        break;
                    case OSD_ITEM_GPL:
                        switch(osd_fix_type)
                        {
                        case 0:
                        case 1:
                            print_P(PSTR("\x10\x20"));
                            break;
                        case 2: //If not APM, x01 would show 2D fix
                        case 3:
                            print_P(PSTR("\x11\x20"));//If not APM, x02 would show 3D fix
                            break;
                        }
                        break;
                    case OSD_ITEM_GPS:
                        printf_P(PSTR("\x83%11.6f|\x84%11.6f"), (double)osd_lat, (double)osd_lon);
                        break ;
                    case OSD_ITEM_Rose:
                        printf_P(PSTR("\x20\xc0\xc0\xc0\xc0\xc0\xc7\xc0\xc0\xc0\xc0\xc0\x20|\xd0%s\xd1"), getHeadingPatern(osd_heading));
                        break;
                    case OSD_ITEM_Head:
                        printf_P(PSTR("%4.0f\xb0"), (double)osd_heading);
                        break;//13x3
                    case OSD_ITEM_MavB:
                        if(mavbeat == 1)
                        {
                            print_P(PSTR("\xEA\xEC"));
                            mavbeat = 0;
                        }
                        else
                        {
                            print_P(PSTR("\xEA\xEB"));
                        }
                        break;//13x3
                    case OSD_ITEM_HDis:
                        if (osd_got_home==1) printf_P(PSTR("\x1F%5.0f\x8D"), (double)osd_home_distance);
                        break;//13x3
                    case OSD_ITEM_HDir:
                        if (osd_got_home==1) DrawArrow();//panHomeDir(); //13x3
                        break;
                    case OSD_ITEM_RSSI:
                        printf_P(PSTR("\xE1%3i%%"), osd_rssi);
                        break;
                    case OSD_ITEM_CurrA:
                        printf_P(PSTR(" \xE4%5.2f\x8F"), osd_curr_A);
                        break;
                    case OSD_ITEM_CurrB:
                        printf_P(PSTR(" \xE5%5.2f\x8F"), osd_curr_B);
                        break;
                    case OSD_ITEM_Alt:
                        printf_P(PSTR("\x85%5.0f\x8D"),(double)(osd_alt));
                        break;
                    case OSD_ITEM_Vel:
                        printf_P(PSTR("\x86%3.0f\x88"),(double)osd_groundspeed);
                        break;
                    case OSD_ITEM_Thr:
                        printf_P(PSTR("\x87%3i%%"), osd_throttle);
                        break;
                    case OSD_ITEM_FMod:
                    {
                        if (apm_mav_type == 1 && osd_mode <= 12 || osd_mode <= 10)
                        {
                            write('\xE0');
                            print_P(apm_mav_type == 1? FM_APM[osd_mode]: FM_ACM[osd_mode]);
                        }
                    }
                    break;
                    case OSD_ITEM_Hor:
                        for (uint8_t j = 0; j < 5; j++)
                        {
                            uint8_t c = (j == 2? '\xd8':'\xc8');
                            write(c);
                            for (uint8_t k = 0; k < 12; k++)
                                write('\x20');
                            write(++c);
                            write('|');
                        }
                        closePanel();
                        DrawHorizon(setting.coord[OSD_ITEM_Hor][0]+1, setting.coord[OSD_ITEM_Hor][1]);
                        break;
                    case OSD_ITEM_SYS:
                        if (osd_sys_status != 4)
                            print_P(PSTR("Disarmed"));
                        else
                            print_P(PSTR("        "));
                        break;
                    }

                    if (i != OSD_ITEM_Hor)
                        closePanel();
                }
            }
        }
    }
    else if (mavlink_status != MAVLINK_STATUS_WAIT_HEARTBEAT)
    {
        //waitingMAVBeats = true;
        if (mavlink_status != (uint8_t)MAVLINK_STATUS_INACTIVE)
            mavlink_status = (uint8_t)MAVLINK_STATUS_WAIT_HEARTBEAT;
        clear();
        DrawLogo();
        setPanel(5, 10);
        openPanel();
        print_P(PSTR("Waiting for|MAVLink heartbeats..."));
        closePanel();
    }
// OSD debug for development (Shown on top-middle panels)
#ifdef membug
    setPanel(13,4);
    openPanel();
    printf("%i",freeMem());
    printf(" %i", crlf_count);
    closePanel();
#endif
}

///////////////////////////////////////////////////////
// Function: loadBar(void)
//
// On bootup time we will show loading bar for defined BOOTTIME seconds
// This is interesting to avoid writing to APM during bootup if OSD's TX is connected
// After that, it continue in normal mode eg starting to listen MAVLink commands

#define barX 5
#define barY 12

void ArduOSD::DrawLoadBar()   //change name due we don't have CLI anymore
{
    int waitTimer;
    uint8_t barStep = 0;

    // Write plain panel to let users know what to do
    //panBoot(barX,barY);
    setPanel(barX, barY);
    openPanel();
    print_P(PSTR("Booting up:\xed\xf2\xf2\xf2\xf2\xf2\xf2\xf2\xf2\xf2\xf3"));
    closePanel();

    delay(500);    // To give small extra waittime to users
    uart_flush();

    //delay(BOOTTIME);

    // Our main loop to wait input from user.

    for(waitTimer = 0; waitTimer <= BOOTTIME; waitTimer++)
    {

        // How often we update our progress bar is depending on modulo
        if(waitTimer % (BOOTTIME / 8) == 0)
        {
            // Update bar it self
            setPanel(barX + 12 + barStep, barY);
            openPanel();
            write('\xee');
            closePanel();
            barStep++;
        }

        delay(1);       // Minor delay to make sure that we stay here long enough
    }
}

//------------------ Home Distance and Direction Calculation ----------------------------------

inline void ArduOSD::SetHomeVars()
{
    float dstlon, dstlat;
    long bearing;

    if(osd_got_home == 0 && osd_fix_type > 1)
    {
        osd_home_lat = osd_lat;
        osd_home_lon = osd_lon;
        osd_home_alt = osd_alt;
        osd_got_home = 1;
    }
    else if(osd_got_home == 1)
    {
        // shrinking factor for longitude going to poles direction
        float rads = fabs(osd_home_lat) * 0.0174532925;
        double scaleLongDown = cos(rads);
        double scaleLongUp   = 1.0f/cos(rads);

        //DST to Home
        dstlat = fabs(osd_home_lat - osd_lat) * 111319.5;
        dstlon = fabs(osd_home_lon - osd_lon) * 111319.5 * scaleLongDown;
        osd_home_distance = sqrt(dstlat*dstlat + dstlon*dstlon);

        //DIR to Home
        dstlon = (osd_home_lon - osd_lon); //OffSet_X
        dstlat = (osd_home_lat - osd_lat) * scaleLongUp; //OffSet Y
        bearing = 90 + (atan2(dstlat, -dstlon) * 57.295775); //absolut home direction
        if(bearing < 0) bearing += 360;//normalization
        bearing = bearing - 180;//absolut return direction
        if(bearing < 0) bearing += 360;//normalization
        bearing = bearing - osd_heading;//relative home direction
        if(bearing < 0) bearing += 360; //normalization
        osd_home_direction = round((float)(bearing/360.0f) * 16.0f) + 1;//array of arrows =)
        if(osd_home_direction > 16) osd_home_direction = 0;

    }
}

void ArduOSD::ReadMessage()
{
    while(uart_available() > 0)
    {
        uint8_t c = uart_getc();//serial_getc();

        //uart_putc(mavlink_status + '0');
        /* allow CLI to be started by hitting enter 3 times, if no
        heartbeat packets have been received */
        if (mavlink_status == (uint8_t)MAVLINK_STATUS_INACTIVE /*&& millis() < 20000*/)
        {
            switch (c)
            {
            case '\r':
                //crlf_count++;
                break;
            case '\n':
                crlf_count++;
                break;
            default:
                if (crlf_count >= 3)
                {
                    switch (c)
                    {
                    case 'F':
                        UploadFont();
                        continue;
                    case 'S':
                        UploadSetting();
                        continue;
                    case 's':
                        GetSetting();
                        continue;
                    case 'R':
                        Reboot();
                        continue;
                    case 'a':
                        GetAnalog();
                        continue;
                    }
                }
                crlf_count=0;
                break;
            }
        }

        ParseMavlink(c);
        delayMicroseconds(138);
        wdt_reset();
    }
    //trying to grab msg
}

void ArduOSD::Run()
{
    static unsigned long lasttime  = 0;

    while (1)
    {
        wdt_reset();

        if(mavlink_status == MAVLINK_STATUS_REQUIRE_DATA) //Request rate control
        {
            clear();
            DrawLogo();
            setPanel(3,10);
            openPanel();
            print_P(PSTR("Requesting DataStreams..."));
            closePanel();
            for(int n = 0; n < 3; n++)
            {
                RequestMavlinkRates();//Three times to certify it will be readed
                delay(50);
            }
            /*enable_mav_request = 0;
            for (uint8_t i = 0; i < 10; i++)
            {
                delay(200);
                wdt_reset();
            }
            clear();
            waitingMAVBeats = 0;
            lastMAVBeat = millis();//Preventing error from delay sensing*/
            mavlink_status = MAVLINK_STATUS_WAIT_DATA;
        }

        //ReadMavlink();
        ReadMessage();

        unsigned long now = millis();
        if (now - lasttime > 120)
        {
            //ArduOSD::refresh();
            SetHomeVars();

            if (setting.enable & _bv(OSD_ITEM_VBatA_ADC))
                osd_vbat_A = analog_read(setting.ad_setting[AD_VBatA]);
            if (setting.enable & _bv(OSD_ITEM_VBatB_ADC))
                osd_vbat_B = analog_read(setting.ad_setting[AD_VBatB]);
            if (setting.enable & _bv(OSD_ITEM_CurrA_ADC))
                osd_curr_A = analog_read(setting.ad_setting[AD_CurrA]);
            if (setting.enable & _bv(OSD_ITEM_CurrB_ADC))
                osd_curr_B = analog_read(setting.ad_setting[AD_CurrB]);
            if (setting.enable & _bv(OSD_ITEM_RSSI_ADC))
                osd_rssi = (uint8_t)analog_read(setting.ad_setting[AD_RSSI]);
            if (setting.enable & _bv(OSD_ITEM_Alt_R))
                osd_alt -= osd_home_lat;

            Draw();
            lasttime = now;

            //uart_puts("draw\n");
        }
    }
    //uart_puts("
}

float ArduOSD::analog_read(ad_setting_t& ad_setting)
{
    return ::analog_read(ad_setting.channel) * ad_setting.k + ad_setting.b;
}

int main()
{
    ArduOSD::Init();
    ArduOSD::Run();
    return 0;
}