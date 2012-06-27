#ifndef MAVLINK_CLIENT_H
#define MAVLINK_CLIENT_H

#include <stdint.h>

enum MAVLINK_STATUS
{
    MAVLINK_STATUS_INACTIVE,
    MAVLINK_STATUS_WAIT_HEARTBEAT,
    MAVLINK_STATUS_REQUIRE_DATA,
    MAVLINK_STATUS_WAIT_DATA,
    MAVLINK_STATUS_GET_DATA,
    MAVLINK_STATUS_UPDATE_DATA
};


class MavlinkClient
{
protected:
    static void RequestMavlinkRates();
    static void ParseMavlink(uint8_t c);

    static volatile float    	osd_vbat_A;                 // Battery A voltage in milivolt
    static volatile float    	osd_curr_A;                 // Battery A current
//  static volatile uint16_t 	osd_battery_remaining_A;    // 0 to 100 <=> 0 to 1000
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
};

#endif

