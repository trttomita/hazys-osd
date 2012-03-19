#include "ArduOSD.h"

volatile float    ArduOSD::osd_vbat_A;                 // Battery A voltage in milivolt
// float    ArduOSD::osd_curr_A;                 // Battery A current
volatile uint16_t ArduOSD::osd_battery_remaining_A;    // 0 to 100 <=> 0 to 1000
volatile uint8_t  ArduOSD::osd_battery_pic_A;       // picture to show battery remaining
// float    ArduOSD::osd_vbat_B;               // voltage in milivolt
// float    ArduOSD::osd_curr_B;                 // Battery B current
// uint16_t ArduOSD::osd_battery_remaining_B;  // 0 to 100 <=> 0 to 1000
// uint8_t  ArduOSD::osd_battery_pic_B;     // picture to show battery remaining

volatile uint16_t ArduOSD::osd_mode;                   // Navigation mode from RC AC2 = CH5, APM = CH8
volatile uint8_t  ArduOSD::osd_nav_mode;               // Navigation mode from RC AC2 = CH5, APM = CH8

volatile float    ArduOSD::osd_lat;                    // latidude
volatile float    ArduOSD::osd_lon;                    // longitude
volatile uint8_t  ArduOSD::osd_satellites_visible;     // number of satelites
volatile uint8_t  ArduOSD::osd_fix_type;               // GPS lock 0-1=no fix, 2=2D, 3=3D

volatile uint8_t  ArduOSD::osd_got_home;               // tels if got home position or not
volatile float    ArduOSD::osd_home_lat;               // home latidude
volatile float    ArduOSD::osd_home_lon;               // home longitude
volatile float    ArduOSD::osd_home_alt;
volatile long     ArduOSD::osd_home_distance;          // distance from home
volatile uint8_t  ArduOSD::osd_home_direction;             // Arrow direction pointing to home (1-16 to CW loop)

volatile int8_t	 ArduOSD::osd_pitch;                  // pitch form DCM
volatile int8_t   ArduOSD::osd_roll;                   // roll form DCM
volatile int8_t   ArduOSD::osd_yaw;                    // relative heading form DCM
volatile float    ArduOSD::osd_heading;                // ground course heading from GPS
volatile float    ArduOSD::osd_alt;                    // altitude
volatile float    ArduOSD::osd_groundspeed;            // ground speed
volatile uint16_t ArduOSD::osd_throttle;               // throtle

//MAVLink session control
volatile bool  	 ArduOSD::mavbeat;
volatile long    ArduOSD::lastMAVBeat;
volatile bool		 ArduOSD::waitingMAVBeats;
volatile uint8_t  ArduOSD::apm_mav_type;
volatile uint8_t  ArduOSD::apm_mav_system;
volatile uint8_t  ArduOSD::apm_mav_component;
volatile bool  	 ArduOSD::enable_mav_request;

//volatile uint8_t ArduOSD::modeScreen; //NTSC:0, PAL:1

volatile bool ArduOSD::mavlink_active;
volatile uint8_t ArduOSD::crlf_count;