#ifndef OSD_Config_Func_h
#define OSD_Config_Func_h

#include "Max7456.h"

typedef unsigned char byte;

enum OSD_ITEM
{
		OSD_ITEM_Cen, OSD_ITEM_Pit, OSD_ITEM_Rol, OSD_ITEM_BatA, OSD_ITEM_BatB /*(!Not implemented)*/, OSD_ITEM_GPSats, OSD_ITEM_GPL, OSD_ITEM_GPS,
    // panB_REG Byte has:
    OSD_ITEM_Rose, OSD_ITEM_Head, OSD_ITEM_MavB, OSD_ITEM_HDir, OSD_ITEM_HDis, OSD_ITEM_WDir /*(!Not implemented)*/, OSD_ITEM_WDis /*(!Not implemented)*/, OSD_ITEM_RSSI /*(!Not implemented)*/,
    // panC_REG Byte has:
    OSD_ITEM_CurA /*(!Not implemented)*/, OSD_ITEM_CurB /*(!Not implemented)*/, OSD_ITEM_Alt, OSD_ITEM_Vel, OSD_ITEM_Thr, OSD_ITEM_FMod, OSD_ITEM_Hor, OSD_ITEM_SYS,
    
};

struct osd_setting_t 
{
    volatile uint32_t enable;
    volatile uint8_t coord[24][2];
};

class ArduOSD : public OSD
{
public:
    //ArduOSD(osd_setting_t& set);
    static void init();
    static void writeSettings();
    static void readSettings();
    static void uploadFont();
    static void uploadSetting();
    static void getSetting();
//void updateSettings(byte panel, byte panel_x, byte panel_y, byte panel_s );

    //void startPanels();
    static void writePanels();
    static void refresh();

    static void reboot();
//------------------ Heading and Compass ----------------------------------------
    //void setHeadingPatern();
    //char setBatteryPic(uint16_t bat_level);
//------------------ Home Distance and Direction Calculation ----------------------------------
    //void setHomeVars();

    static void loadBar();

		static void read_data();
		
private:
	  static void request_mavlink_rates();
    static void read_mavlink();
    static void panLogo();
    static void showArrow();
    static inline void showHorizon(uint8_t start_col, uint8_t start_row);
    static inline void setHomeVars();
    //void clearRow(int row);
		static inline void init_analog();
		static uint16_t read_analog(uint8_t channel);
		
private:
    //panel_setting_t setting;


    static volatile float    	osd_vbat_A;                 // Battery A voltage in milivolt
// float    osd_curr_A;                 // Battery A current
    static volatile uint16_t 	osd_battery_remaining_A;    // 0 to 100 <=> 0 to 1000
    static volatile uint8_t  	osd_battery_pic_A;       // picture to show battery remaining
// float    osd_vbat_B;               // voltage in milivolt
// float    osd_curr_B;                 // Battery B current
// uint16_t osd_battery_remaining_B;  // 0 to 100 <=> 0 to 1000
// uint8_t  osd_battery_pic_B;     // picture to show battery remaining

    static volatile uint16_t 	osd_mode;                   // Navigation mode from RC AC2 = CH5, APM = CH8
    static volatile uint8_t  	osd_nav_mode;               // Navigation mode from RC AC2 = CH5, APM = CH8

    static volatile float    	osd_lat;                    // latidude
    static volatile float    	osd_lon;                    // longitude
    static volatile uint8_t  	osd_satellites_visible;     // number of satelites
    static volatile uint8_t  	osd_fix_type;               // GPS lock 0-1=no fix, 2=2D, 3=3D

    static volatile uint8_t  	osd_got_home;               // tels if got home position or not
    static volatile float    	osd_home_lat;               // home latidude
    static volatile float    	osd_home_lon;               // home longitude
    static volatile float    	osd_home_alt;
    static volatile long     	osd_home_distance;          // distance from home
    static volatile uint8_t  	osd_home_direction;             // Arrow direction pointing to home (1-16 to CW loop)

    static volatile int8_t	 	osd_pitch;                  // pitch form DCM
    static volatile int8_t   	osd_roll;                   // roll form DCM
    static volatile int8_t   	osd_yaw;                    // relative heading form DCM
    static volatile float    	osd_heading;                // ground course heading from GPS
    static volatile float    	osd_alt;                    // altitude
    static volatile float    	osd_groundspeed;            // ground speed
    static volatile uint16_t 	osd_throttle;               // throtle

		static volatile uint8_t		osd_sys_status;
		
//MAVLink session control
    static volatile bool  	 	mavbeat;
    static volatile long     	lastMAVBeat;
    static volatile bool		 	waitingMAVBeats;
    static volatile uint8_t  	apm_mav_type;
    static volatile uint8_t  	apm_mav_system;
    static volatile uint8_t  	apm_mav_component;
    static volatile bool  	 	enable_mav_request;

    static volatile uint8_t 	modeScreen; //NTSC:0, PAL:1

		static volatile bool 			mavlink_active;
		static volatile uint8_t 	crlf_count;


    static osd_setting_t 			setting;
};

#endif // OSD_Config_Func_h

