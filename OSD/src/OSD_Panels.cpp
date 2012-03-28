#include <avr/pgmspace.h>
#include <math.h>
#include <stdlib.h>
//#include <wiring.h>
#include "Config.h"
//#include "OSD_Config.h"
#include "time.h"
#include "uart.h"
#include <util/delay.h>
#include "ArduOSD.h"
#include "MemoryFree.h"

#ifdef MAVLINK10
# include "GCS_MAVLink/include_v1.0/mavlink_types.h"
#else
# include "GCS_MAVLink/include/mavlink_types.h"
#endif

typedef unsigned char byte;


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

char buf_show[12] = {0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0};

const char buf_Rule[36] = {0xc2,0xc0,0xc0,0xc1,0xc0,0xc0,0xc1,0xc0,0xc0,
                           0xc4,0xc0,0xc0,0xc1,0xc0,0xc0,0xc1,0xc0,0xc0,
                           0xc3,0xc0,0xc0,0xc1,0xc0,0xc0,0xc1,0xc0,0xc0,
                           0xc5,0xc0,0xc0,0xc1,0xc0,0xc0,0xc1,0xc0,0xc0
                          };

//------------------ Panel: Startup ArduCam OSD LOGO -------------------------------

void ArduOSD::panLogo(/*int first_col, int first_line*/)
{
    setPanel(10, 5);
    openPanel();
    print_P(PSTR("\x20\x20\x20\x20\xba\xbb\xbc\xbd\xbe|\x20\x20\x20\x20\xca\xcb\xcc\xcd\xce|ArduCam OSD"));
    closePanel();
}


/* **************************************************************** */
// Panel  : panFlightMode
// Needs  : X, Y locations
// Output : 2 symbols, one static name symbol and another that changes by flight modes
// Size   : 1 x 2  (rows x chars)
// Status : done


inline const char* getFlightModeString(uint8_t apm_mav_type, uint8_t osd_nav_mode, uint16_t osd_mode)
{
    const char* str = NULL;
#ifndef MAVLINK10
    if(apm_mav_type == 2) //ArduCopter MultiRotor or ArduCopter Heli
    {
        switch(osd_mode)
        {
        case 100:
            str= FM_STAB;
            break;
        case 101:
            str = FM_ACRO;
            break;
        case 102:
            str = FM_ALTH;
            break;
        case MAV_MODE_AUTO:
            switch (osd_nav_mode)
            {
            case MAV_NAV_WAYPOINT:
                str = FM_AUTO;
                break;
            case MAV_NAV_HOLD:
                str = FM_LOIT;
                break;
            case MAV_NAV_RETURNING:
                str = FM_RETL;
                break;
            }
            break;
        case MAV_MODE_GUIDED:
            if (osd_nav_mode == MAV_NAV_WAYPOINT) str = FM_GUID;
            break;
        case 107:
            str=FM_CIRC;
            break;
        case 108:
            str = FM_POSI;
            break;
        case 109:
            str = FM_LAND;
            break;
        case 110:
            str = FM_OFLO;
            break;
        }
    }
    else if(apm_mav_type == 1) //ArduPlane
    {

        if(osd_mode == MAV_MODE_TEST1 && osd_nav_mode == MAV_NAV_VECTOR) str = FM_STAB;//Stabilize
        else if(osd_mode == MAV_MODE_TEST2)
        {
            if (osd_nav_mode == 1) str = FM_FBWA;//FLY_BY_WIRE_A
            else if(osd_nav_mode == 2) str = FM_FBWB;//FLY_BY_WIRE_B
        }
        else if(osd_mode == MAV_MODE_MANUAL && osd_nav_mode == MAV_NAV_VECTOR) str = FM_MANU;//Manual
        else if(osd_mode == MAV_MODE_AUTO)
        {
            if (osd_nav_mode == MAV_NAV_RETURNING)        			str = FM_RETL;
            else if (osd_nav_mode== MAV_NAV_WAYPOINT)        				str = FM_AUTO;
            else if (osd_nav_mode == MAV_NAV_LOITER) str = FM_LOIT;//Loiter
        }
        else  if(osd_mode == MAV_MODE_TEST3) str = FM_CIRC;//CIRCLE
    }
#else
    if(apm_mav_type == 2) //ArduCopter MultiRotor or ArduCopter Heli
    {
        switch (osd_mode)
        {
        case 0:
            str = (FM_STAB);//Stabilize
            break;
        case 1:
            str = (FM_STAB);//Stabilize
            break;
        case 2:
            str = (FM_ALTH);//Alt Hold;
            break;
        case 3:
            str = (FM_AUTO);//Auto
            break;
        case 4:
            str = (FM_GUID);//Guided
            break;
        case 5:
            str = (FM_LOIT);//Loiter
            break;
        case 6:
            str = (FM_RETL);//Return to Launch
            break;
        case 7:
            str = (FM_CIRC); // Circle
            break;
        case 8:
            str = (FM_POSI); // Position
            break;
        case 9:
            str = (FM_LAND); // Land
            break;
        case 10:
            str = (FM_OFLO); // OF_Loiter
            break;
        }
    }
    else if(apm_mav_type == 1) //ArduPlane
    {
        switch(osd_mode)
        {
        case 0:
            str = FM_MANU;
            break;
        case 1:
            str = FM_CIRC;
            break;
        case 2:
            str = FM_STAB;
            break;
        case 5:
            str = FM_FBWA;
            break;
        case 6:
            str = FM_FBWB;
            break;
        case 10:
            str = FM_AUTO;
            break;
        case 11:
            str = FM_RETL;
            break;
        case 12:
            str = FM_LOIT;
            break;
        }
    }
#endif

    return str;
}


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
void ArduOSD::showArrow()
{
    uint8_t c = '\x90';
    if (osd_home_direction > 1)
        c += ((osd_home_direction-1) << 1);

    write(c);
    write(++c);
}


// Calculate and shows Artificial Horizon
inline void ArduOSD::showHorizon(uint8_t start_col, uint8_t start_row)
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

void ArduOSD::writePanels()
{
    unsigned long timeout = millis() - lastMAVBeat;

    if(timeout < 2000)
    {
        if (waitingMAVBeats)
        {
            clear();
        }

        for (uint8_t i = 0; i < 24; i++)
        {
            if (setting.enable & (1UL << i))
            {
                setPanel(setting.coord[i][0], setting.coord[i][1]);
                openPanel();

                switch (i)
                {
                case OSD_ITEM_Cen:
                    //panCenter();
                    print_P(PSTR("\x05\x03\x04\x05|\x15\x13\x14\x15"));
                    break;
                case OSD_ITEM_Pit:
                    //panPitch();
                    printf_P(PSTR("%4i\xb0\xb1"),osd_pitch);
                    break;
                case OSD_ITEM_Rol:
                    //panRoll();
                    printf_P(PSTR("%4i\xb0\xb2"),osd_roll);
                    break;
                case OSD_ITEM_BatA:
                    //panBatt_A();
                    printf_P(PSTR(" \xE2%5.2f\x8E"), (double)osd_vbat_A);
                    break;
                case OSD_ITEM_GPSats:
                    //panGPSats();
                    printf_P(PSTR("\x0f%2i"), osd_satellites_visible);
                    break;
                case OSD_ITEM_GPL:
                    //panGPL();
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
                    //panGPS();
                    printf_P(PSTR("\x83%11.6f|\x84%11.6f"), (double)osd_lat, (double)osd_lon);
                    break ;
                case OSD_ITEM_Rose:
                    //panRose();
                    printf_P(PSTR("\x20\xc0\xc0\xc0\xc0\xc0\xc7\xc0\xc0\xc0\xc0\xc0\x20|\xd0%s\xd1"), getHeadingPatern(osd_heading));
                    break;
                case OSD_ITEM_Head:
                    //panHeading();
                    printf_P(PSTR("%4.0f\xb0"), (double)osd_heading);
                    break;//13x3
                case OSD_ITEM_MavB:
                    //panMavBeat();
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
                    if (osd_got_home==1) showArrow();//panHomeDir(); //13x3
                    break;
                case OSD_ITEM_Alt:
                    //panAlt(); //
                    printf_P(PSTR("\x85%5.0f\x8D"),(double)(osd_alt));
                    break;
                case OSD_ITEM_Vel:
                    //panVel(); //
                    printf_P(PSTR("\x86%3.0f\x88"),(double)osd_groundspeed);
                    break;
                case OSD_ITEM_Thr:
                    //panThr(); //
                    printf_P(PSTR("\x87%3.0i\x25"),osd_throttle);
                    break;
                case OSD_ITEM_FMod:
                {
                    const char*mode = getFlightModeString(apm_mav_type, osd_nav_mode, osd_mode);//getFlightModeString();
                    if (mode != NULL)
                    {
                        write('\xE0');
                        print_P(mode);
                    }
                    //panFlightMode();  //
                }
                break;
                case OSD_ITEM_Hor:
                    //panHorizon(panel_setting.coord[Hor_BIT][0], panel_setting.coord[Hor_BIT][1]); //14x5
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
                    showHorizon(setting.coord[OSD_ITEM_Hor][0]+1, setting.coord[OSD_ITEM_Hor][1]);
                    break;
                case OSD_ITEM_SYS:
                		if (osd_sys_status != MAV_STATE_ACTIVE)
                				printf_P(PSTR("Disarmed"));
                		else
                				printf_P(PSTR("        "));
                		break;
                }
								
                if (i != OSD_ITEM_Hor)
                    closePanel();
            }
        }
    }
    else if (!waitingMAVBeats)
    {
        waitingMAVBeats = true;
        clear();
        panLogo();
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

void ArduOSD::refresh()
{
    //setHeadingPatern();  	// generate the heading patern

    //osd_battery_pic_B = setBatteryPic(osd_battery_remaining_B);     // battery B remmaning picture
    setHomeVars();   			// calculate and set Distance from home and Direction to home
    writePanels();   			// writing enabled panels (check OSD_Panels Tab)
}

///////////////////////////////////////////////////////
// Function: loadBar(void)
//
// On bootup time we will show loading bar for defined BOOTTIME seconds
// This is interesting to avoid writing to APM during bootup if OSD's TX is connected
// After that, it continue in normal mode eg starting to listen MAVLink commands

#define barX 5
#define barY 12

void ArduOSD::loadBar()   //change name due we don't have CLI anymore
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

        _delay_ms(1);       // Minor delay to make sure that we stay here long enough
    }
}

//------------------ Home Distance and Direction Calculation ----------------------------------

inline void ArduOSD::setHomeVars()
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
