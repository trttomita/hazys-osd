#ifndef OSD_Config_Func_h
#define OSD_Config_Func_h

#include "Max7456.h"
#include "MavlinkClient.h"

typedef unsigned char byte;

enum OSD_ITEM
{
    OSD_ITEM_Cen, OSD_ITEM_Pit, OSD_ITEM_Rol, OSD_ITEM_VBatA, OSD_ITEM_VBatB /*(!Not implemented)*/, OSD_ITEM_GPSats, OSD_ITEM_GPL, OSD_ITEM_GPS,
    // panB_REG Byte has:
    OSD_ITEM_Rose, OSD_ITEM_Head, OSD_ITEM_MavB, OSD_ITEM_HDir, OSD_ITEM_HDis, OSD_ITEM_WDir /*(!Not implemented)*/, OSD_ITEM_WDis /*(!Not implemented)*/, OSD_ITEM_RSSI /*(!Not implemented)*/,
    // panC_REG Byte has:
    OSD_ITEM_CurrA /*(!Not implemented)*/, OSD_ITEM_CurrB /*(!Not implemented)*/, OSD_ITEM_Alt, OSD_ITEM_Vel, OSD_ITEM_Thr, OSD_ITEM_FMod, OSD_ITEM_Hor, OSD_ITEM_SYS,

    OSD_ITEM_VBatA_ADC, OSD_ITEM_VBatB_ADC, OSD_ITEM_CurrA_ADC, OSD_ITEM_CurrB_ADC, OSD_ITEM_RSSI_ADC, OSD_ITEM_Alt_R
};

enum AD_ITEM
{
	  AD_VBatA,
	  AD_VBatB,
	  AD_CurrA,
	  AD_CurrB,
	  AD_RSSI,
	  AD_COUNT
};
/*
enum OSD_ALT_CONF
{
    OSD_ALT_BAT_A_ADC = (1UL << 24),
    OSD_ALT_BAT_B_ADC = (1UL << 25),
    OSD_ALT_CUR_A_ADC = (1UL << 26),
    OSD_ALT_CUR_B_ADC = (1UL << 27),
    OSD_ALT_RSSI_ADC = (1UL << 28),
    OSD_ALT_REL_ALT = (1UL << 31)
}*/

struct ad_setting_t
{
    uint8_t channel;
    float	k;
    float b;
};

struct osd_setting_t
{
	  uint8_t ver;
    uint32_t enable;
    uint8_t coord[24][2];
    ad_setting_t ad_setting[(int)AD_COUNT];
};



class ArduOSD : public OSD, MavlinkClient
{
public:
    static void Init();
    static void Run();

private:
    // config functions
    static void LoadSetting();
    static void UploadFont();
    static void UploadSetting();
    static void GetSetting();
    static void GetAnalog();
    static void Reboot();

    // data functions
    //static void RequestMavlinkRates();
    //static void ReadMavlink();
    static void ReadMessage();
    static inline void SetHomeVars();

    // draw functions
    static void Draw();
    static void DrawLoadBar();
    static void DrawLogo();
    static void DrawArrow();
    static inline void DrawHorizon(uint8_t start_col, uint8_t start_row);
    static float analog_read(ad_setting_t& ad_setting);
private:
	/*
    static volatile float    	osd_vbat_A;                 // Battery A voltage in milivolt
    static volatile float    	osd_curr_A;                 // Battery A current
    static volatile uint16_t 	osd_battery_remaining_A;    // 0 to 100 <=> 0 to 1000
    static volatile uint8_t  	osd_battery_pic_A;       		// picture to show battery remaining
    static volatile float    	osd_vbat_B;               	// voltage in milivolt
    static volatile float    	osd_curr_B;                 // Battery B current
//  static volatile uint16_t 	osd_battery_remaining_B;  	// 0 to 100 <=> 0 to 1000
//  static volatile uint8_t  	osd_battery_pic_B;     			// picture to show battery remaining

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
    static volatile uint8_t  	osd_home_direction;         // Arrow direction pointing to home (1-16 to CW loop)

    static volatile int8_t	 	osd_pitch;                  // pitch form DCM
    static volatile int8_t   	osd_roll;                   // roll form DCM
    static volatile int8_t   	osd_yaw;                    // relative heading form DCM
    static volatile float    	osd_heading;                // ground course heading from GPS
    static volatile float    	osd_alt;                    // altitude
    static volatile float    	osd_groundspeed;            // ground speed
    static volatile uint16_t 	osd_throttle;               // throtle

    static volatile uint8_t		osd_sys_status;

    static volatile uint8_t		osd_rssi;

//MAVLink session control
    static volatile bool  	 	mavbeat;
    static volatile long     	lastMAVBeat;
    //static volatile bool		 	waitingMAVBeats;
    static volatile uint8_t  	apm_mav_type;
    static volatile uint8_t  	apm_mav_system;
    static volatile uint8_t  	apm_mav_component;
    //static volatile bool  	 	enable_mav_request;
    static volatile uint8_t   mavlink_status;

    //static volatile uint8_t 	modeScreen; //NTSC:0, PAL:1

    //static volatile bool 			mavlink_active;*/
    static volatile uint8_t 	crlf_count;


    static osd_setting_t 			setting;
};

#endif // OSD_Config_Func_h

