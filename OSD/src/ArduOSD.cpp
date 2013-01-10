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
char ArduOSD::spd_sym;
char ArduOSD::dst_sym;
float ArduOSD::converts;
float ArduOSD::convertd;		


osd_setting_t ArduOSD::setting =
{
    DATA_VER,
    0,
    _bv(OSD_ITEM_Pit) | _bv(OSD_ITEM_Rol) | _bv(OSD_ITEM_VBatA) | _bv(OSD_ITEM_GPSats) | _bv(OSD_ITEM_GPL) | _bv(OSD_ITEM_GPS)
    | _bv(OSD_ITEM_Rose) | _bv(OSD_ITEM_Head) | _bv(OSD_ITEM_MavB)| _bv(OSD_ITEM_HDir) | _bv(OSD_ITEM_HDis)
    | _bv(OSD_ITEM_Alt) | _bv(OSD_ITEM_HAlt) | _bv(OSD_ITEM_Vel) | _bv(OSD_ITEM_AS) | _bv(OSD_ITEM_Thr) | _bv(OSD_ITEM_FMod) 
    | _bv(OSD_ITEM_Hor) | _bv(OSD_ITEM_SYS),


    {   
    	{13, 7}, //  panCenter_y_ADDR
      {22, 9}, //  panPitch_y_ADDR
            {11, 1}, //  panRoll_y_ADDR
            {1, 6}, //  panBatt_A_y_ADDR
            {1, 8}, //  panBatt_B_y_ADDR
            {1, 13}, // panGPSats_y_ADDR
            {4, 13}, // panGPL_y_ADDR
            {1, 14}, // panGPS_y_ADDR
            {16, 14}, // panRose_y_ADDR
            {24, 13}, // panHeading_y_ADDR
            {14, 15}, // panMavBeat_y_ADDR
            {14, 3}, //  panHomeDir_y_ADDR
            {22, 1}, //  panHomeDis_y_ADDR
            {17, 12}, //  panWPDir_y_ADDR
            {20, 12}, //  panWPDis_y_ADDR
            {22, 5}, ////  panRSSI_y_ADDR
            {1, 7}, //  panCur_A_y_ADDR
            {1, 9}, //  panCur_B_y_ADDR
            {22, 3}, //  panAlt_y_ADDR
            {22, 2}, //homeAlt
            {1, 2}, //  panVel_y_ADDR
            {1, 1}, // panAS
            {1, 4}, //  panThr_y_ADDR
            {17, 13}, // panFMod_y_ADDR
            {8, 6}, //  panHorizon_y_ADDR
            {11,4}
    }
};

uint8_t mark_EE EEMEM;
//uint8_t verison_EE EEMEM;
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
    
    PORTD |= _BV(PD6);	//PD6 key input, pull up

    sei();

    OSD::Init();


    wdt_enable(WDTO_2S);
}

void ArduOSD::LoadSetting()
{
    if (eeprom_read_byte(&mark_EE) != 'O' || eeprom_read_byte((const uint8_t*)&setting_EE) != DATA_VER)
    {
        SetPanel(6,9);
        OpenPanel();
        print_P(PSTR("Missing/Old Config"));
        ClosePanel();

        //loadBar();
        delay(500);

        eeprom_write_byte(&mark_EE, 'O');
        //eeprom_write_byte(&setting_EE, DATA_VER);
        eeprom_write_block((const void*) &setting, &setting_EE, sizeof(setting));

        SetPanel(6,9);
        OpenPanel();
        print_P(PSTR("OSD Initialized   "));
        ClosePanel();

        delay(500);
    }
    else
    {
        eeprom_read_block(&setting, &setting_EE, sizeof(setting));
    }

    if (GetMode() == MAX7456_MODE_NTCS)
        for (int j = 0; j < 24; j++)
        {
            if(setting.coord[j][1] >= GetCenter())
                setting.coord[j][1] -= 3;//Cutting lines offset after center if NTSC
        }
        
    if (setting.option & _BV(OSD_OPT_M_ISO))
    { 
    	converts = 3.6;
        convertd = 1.0;
        spd_sym = 0x81;
        dst_sym = 0x8D;
    } else 
    {
        converts = 2.23;
        convertd = 3.28;
        spd_sym = 0xfb;
        dst_sym = 0x66;
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

    Clear();
}

void ArduOSD::GetSetting()
{
    uint8_t checksum = 0;
    uart_putc('s');
    uart_putc(sizeof(setting));
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
        if (ck == checksum && buf[0] == DATA_VER)
        {
            memcpy(&setting, buf, sizeof(setting));
            wdt_reset();
            eeprom_write_block((const void*) &setting, &setting_EE, sizeof(setting));
            wdt_reset();
            ack = '!';
            Clear();
        }
    }
    uart_putc(ack);
}

void ArduOSD::Reboot()
{
    uart_putc('R');
    Clear();
    DrawLogo();
    SetPanel(6,9);
    OpenPanel();
    print_P(PSTR("Rebooting..."));
    ClosePanel();
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

/*
const char ST_UNINIT[] 	PROGMEM = "Unknown ";
const char ST_BOOT[]	 	PROGMEM = "Booting "
const char ST_CALI[]   	PROGMEM = "Calibrat";
const char ST_STANDBY[] PROGMEM = "Disarmed";
const char ST_ACTIVE[] 	PROGMEM = "        ";
const char ST_CRITICAL[] PROGMEM = "Critical";
const char ST_EMEGENCY[] PROGMEM = "Emegency";
const char ST_POWEROFF[] PROGMEM = "PowerOff";

const char* MAV_STATE[] = {ST_UNINIT, ST_BOOT, ST_CALI, ST_STANDBY, ST_ACTIVE, };
*/

char buf_show[12] = {0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0};

const char buf_Rule[36] = {0xc2,0xc0,0xc0,0xc1,0xc0,0xc0,0xc1,0xc0,0xc0,
                           0xc4,0xc0,0xc0,0xc1,0xc0,0xc0,0xc1,0xc0,0xc0,
                           0xc3,0xc0,0xc0,0xc1,0xc0,0xc0,0xc1,0xc0,0xc0,
                           0xc5,0xc0,0xc0,0xc1,0xc0,0xc0,0xc1,0xc0,0xc0
                          };

//------------------ Panel: Startup ArduCam OSD LOGO -------------------------------

void ArduOSD::DrawLogo(/*int first_col, int first_line*/)
{
    SetPanel(7, 5);
    OpenPanel();
    //print_P(PSTR("\x20\x20\x20\xba\xbb\xbc\xbd\xbe|\x20\x20\x20\xca\xcb\xcc\xcd\xce|Hazys OSD"));
    print_P(PSTR("Hazy's OSD v" OSD_VER "| for MAVLink 1.0"));
    ClosePanel();
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
void ArduOSD::DrawArrow(uint8_t dir)
{
    uint8_t c = '\x90';
    if (dir > 1)
        c += ((dir-1) << 1);

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
            OpenSingle(start_col + col, start_row + row - 1);
            write('\x05' + subval);
            //write('0' + start_row + row - 1);
        }
    }
}


/******* PANELS - POSITION *******/

void ArduOSD::Draw()
{

    for (uint8_t i = 0; i < sizeof(setting.coord)/sizeof(uint8_t*); i++)
    {
        if (setting.enable & (1UL << i))
        {
            SetPanel(setting.coord[i][0], setting.coord[i][1]);
            OpenPanel();

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
                printf_P(PSTR("\xE2%5.2f\x8E"), (double)osd_vbat_A);
                break;
            case OSD_ITEM_VBatB:
                printf_P(PSTR("\xE3%5.2f\x8E"), (double)osd_vbat_B);
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
                /*if (osd_got_home==1) */printf_P(PSTR("\x1F%5.0f%c"), (double)(osd_home_distance * convertd), dst_sym);
                break;//13x3
            case OSD_ITEM_HDir:
                /*if (osd_got_home==1) */DrawArrow(osd_home_direction);//panHomeDir(); //13x3
                break;
            case OSD_ITEM_WDir:
            		{
            				int8_t wp_target_bearing_rotate_int = round(((float)wp_target_bearing - osd_heading)/360.0 * 16.0) + 1; //Convert to int 0-16 
    								if(wp_target_bearing_rotate_int < 0 ) wp_target_bearing_rotate_int += 16; //normalize
    								DrawArrow((uint8_t)wp_target_bearing_rotate_int);
            		}
            		break;
            case OSD_ITEM_WDis:
            		printf_P(PSTR("\x57%2i\x00%4.0f%c"), wp_number, (double)(wp_dist * convertd), dst_sym);
            		break;
            case OSD_ITEM_RSSI:
                printf_P(PSTR("\xE1%3i%%"), osd_rssi);
                break;
            case OSD_ITEM_CurrA:
                printf_P(PSTR("\xE4%5.2f\x8F"), osd_curr_A);
                break;
            case OSD_ITEM_CurrB:
                printf_P(PSTR("\xE5%5.2f\x8F"), osd_curr_B);
                break;
            case OSD_ITEM_Alt:
                printf_P(PSTR("\xe6%5.0f%c"),
                	(double)(osd_alt*convertd), dst_sym);
                break;
            case OSD_ITEM_HAlt:
            		printf_P(PSTR("\xe7%5.0f%c"),
            			(double)((osd_alt - osd_home_alt) * convertd), dst_sym);
            		break;
            case OSD_ITEM_Vel:
                printf_P(PSTR("\xe9%3.0f%c"), (double)osd_groundspeed*converts, spd_sym);
                break;
            case OSD_ITEM_AS:
            		printf_P(PSTR("\xe8%3.0f%c"), (double)osd_airspeed*converts, spd_sym);
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
                ClosePanel();
                DrawHorizon(setting.coord[OSD_ITEM_Hor][0]+1, setting.coord[OSD_ITEM_Hor][1]);
                break;
            case OSD_ITEM_SYS:
                if (osd_sys_status < 3)
                    print_P(PSTR("APM Init"));
                else if (osd_sys_status != 4)
                    print_P(PSTR("Disarmed"));
                else
                    print_P(PSTR("        "));
                break;
            }

            if (i != OSD_ITEM_Hor)
                ClosePanel();
        }
    }


// OSD debug for development (Shown on top-middle panels)
#ifdef membug
    SetPanel(13,4);
    OpenPanel();
    printf("%i",freeMem());
    printf(" %i", crlf_count);
    ClosePanel();
#endif
}

void ArduOSD::DrawWaitingHB()
{
    Clear();
    DrawLogo();
    SetPanel(5, 10);
    OpenPanel();
    print_P(PSTR("Waiting for|MAVLink heartbeats..."));

    ClosePanel();
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
    SetPanel(barX, barY);
    OpenPanel();
    print_P(PSTR("Booting up:\xed\xf2\xf2\xf2\xf2\xf2\xf2\xf2\xf2\xf2\xf3"));
    ClosePanel();

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
            SetPanel(barX + 12 + barStep, barY);
            OpenPanel();
            write('\xee');
            ClosePanel();
            barStep++;
        }

        delay(1);       // Minor delay to make sure that we stay here long enough
        wdt_reset();
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
    unsigned long lasttime  = 0;
    //unsigned long key_time = 0;
    
    unsigned long timeout = 0;
    unsigned char key_state = 0;

    DrawLogo();
    LoadSetting();
    DrawLoadBar();

    //delay(500);
    //wdt_reset();

    DrawWaitingHB();

    while (1)
    {
        wdt_reset();

        if(mavlink_status == MAVLINK_STATUS_REQUIRE_DATA) //Request rate control
        {
            Clear();
            DrawLogo();
            SetPanel(3,10);
            OpenPanel();
            print_P(PSTR("Requesting DataStreams..."));
            ClosePanel();
            for(int n = 0; n < 3; n++)
            {
                RequestMavlinkRates();//Three times to certify it will be readed
                delay(50);
            }

            mavlink_status = MAVLINK_STATUS_WAIT_DATA;
        }

        //ReadMavlink();
        ReadMessage();

        unsigned long now = millis();
        timeout = now - lastMAVBeat;
        if (timeout < 2000)
        {
            if (now - lasttime > 120 && mavlink_status >= (uint8_t)MAVLINK_STATUS_GET_DATA)
            {
                SetHomeVars();

                if (setting.option & _bv(OSD_OPT_VBatA_ADC))
                    osd_vbat_A = analog_read(setting.ad_setting[AD_VBatA]);
                if (setting.option & _bv(OSD_OPT_VBatB_ADC))
                    osd_vbat_B = analog_read(setting.ad_setting[AD_VBatB]);
                if (setting.option & _bv(OSD_OPT_CurrA_ADC))
                    osd_curr_A = analog_read(setting.ad_setting[AD_CurrA]);
                if (setting.option & _bv(OSD_OPT_CurrB_ADC))
                    osd_curr_B = analog_read(setting.ad_setting[AD_CurrB]);
                if (setting.option & _bv(OSD_OPT_RSSI_ADC))
                    osd_rssi = (uint8_t)analog_read(setting.ad_setting[AD_RSSI]);
                //if (setting.enable & _bv(OSD_ITEM_Alt_R))
                //    osd_alt -= osd_home_lat;

                if (mavlink_status == (uint8_t)MAVLINK_STATUS_GET_DATA)
                {
                    Clear();
                    mavlink_status = (uint8_t)MAVLINK_STATUS_UPDATE_DATA;
                }

                Draw();
                lasttime = now;
            }
        }
        else if (mavlink_status != (uint8_t)MAVLINK_STATUS_WAIT_HEARTBEAT
                 && mavlink_status != (uint8_t)MAVLINK_STATUS_INACTIVE)
        {
            mavlink_status = MAVLINK_STATUS_WAIT_HEARTBEAT;
            DrawWaitingHB();
        }
        
        if (timer2_tick)
       	{
       		//key_time = now + 20;
       		timer2_tick = false;
       		char key_down = !(PIND & _BV(PD6));
       		
       		if (key_down)
       		{
       			if (key_state == 0)
       				key_state = 1;
       			else if (key_state = 1)
       				key_state = 2;	//confirm down
       			else if (key_state = 3)
       				key_state = 2;
       		}
       		else
       		{
       			if (key_state = 1)
       				key_state = 0;
       			else if (key_state = 2)
       				key_state = 3;
       			else if (key_state = 3)
       			{
       				key_state = 0;	//confirm up
       				//key pressed
       				osd_got_home = 0;
       				osd_home_distance = 0;
       				osd_home_direction = 0;
       			}
       		}
       	}
    }
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