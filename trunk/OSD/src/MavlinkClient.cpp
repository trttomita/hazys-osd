#include "MavlinkClient.h"

#define MAVLINK_COMM_NUM_BUFFERS 1
#define MAVLINK_USE_CONVENIENCE_FUNCTIONS
#define MAVLINK_MAX_PAYLOAD_LEN 127

#ifdef MAVLINK10
#ifndef MAVLINK_CRC_EXTRA
#define MAVLINK_CRC_EXTRA 1
#endif
uint8_t get_mavlink_message_crc(uint8_t msgid);
#define MAVLINK_MESSAGE_CRC(msgid) get_mavlink_message_crc(msgid)
#include "../GCS_MAVLink/include/mavlink/v1.0/mavlink_types.h"
#include "../GCS_MAVLink/include/mavlink/v1.0/ardupilotmega/mavlink.h"
#else
#include "../GCS_MAVLink/include/mavlink/v0.9/mavlink_types.h"
#include "../GCS_MAVLink/include/mavlink/v0.9/ardupilotmega/mavlink.h"
#endif


#include "Mavlink_compat.h"
#include "time.h"

#define ToDeg(x) 	(x*57.2957795131)  // *180/pi

volatile float    MavlinkClient::osd_vbat_A;                 // Battery A voltage in milivolt
volatile float    MavlinkClient::osd_curr_A;                 // Battery A current
//volatile uint16_t MavlinkClient::osd_battery_remaining_A;    // 0 to 100 <=> 0 to 1000
volatile uint8_t  MavlinkClient::osd_battery_pic_A;       	 // picture to show battery remaining
volatile float    MavlinkClient::osd_vbat_B;               	 // voltage in milivolt
volatile float    MavlinkClient::osd_curr_B;                 // Battery B current
// uint16_t 			MavlinkClient::osd_battery_remaining_B;  	 // 0 to 100 <=> 0 to 1000
// uint8_t  			MavlinkClient::osd_battery_pic_B;     		 // picture to show battery remaining

volatile uint16_t MavlinkClient::osd_mode;                   // Navigation mode from RC AC2 = CH5, APM = CH8
volatile uint8_t  MavlinkClient::osd_nav_mode;               // Navigation mode from RC AC2 = CH5, APM = CH8

volatile float    MavlinkClient::osd_lat;                    // latidude
volatile float    MavlinkClient::osd_lon;                    // longitude
volatile uint8_t  MavlinkClient::osd_satellites_visible;     // number of satelites
volatile uint8_t  MavlinkClient::osd_fix_type;               // GPS lock 0-1=no fix, 2=2D, 3=3D

volatile uint8_t  MavlinkClient::osd_got_home;               // tels if got home position or not
volatile float    MavlinkClient::osd_home_lat;               // home latidude
volatile float    MavlinkClient::osd_home_lon;               // home longitude
volatile float    MavlinkClient::osd_home_alt;
volatile long     MavlinkClient::osd_home_distance;          // distance from home
volatile uint8_t  MavlinkClient::osd_home_direction;         // Arrow direction pointing to home (1-16 to CW loop)

volatile int8_t	 	MavlinkClient::osd_pitch;                  // pitch form DCM
volatile int8_t   MavlinkClient::osd_roll;                   // roll form DCM
volatile int8_t   MavlinkClient::osd_yaw;                    // relative heading form DCM
volatile float    MavlinkClient::osd_heading;                // ground course heading from GPS
volatile float    MavlinkClient::osd_alt;                    // altitude
volatile float    MavlinkClient::osd_groundspeed;            // ground speed
volatile uint16_t MavlinkClient::osd_throttle;               // throtle

volatile uint8_t	MavlinkClient::osd_sys_status;
volatile uint8_t	MavlinkClient::osd_rssi;

//MAVLink session control
volatile bool  	 	MavlinkClient::mavbeat;
volatile long    	MavlinkClient::lastMAVBeat;
//volatile bool		 	MavlinkClient::waitingMAVBeats;
volatile uint8_t  MavlinkClient::apm_mav_type;
volatile uint8_t  MavlinkClient::apm_mav_system;
volatile uint8_t  MavlinkClient::apm_mav_component;
//volatile bool  	 	MavlinkClient::enable_mav_request;
volatile uint8_t MavlinkClient::mavlink_status = MAVLINK_STATUS_INACTIVE;

mavlink_system_t mavlink_system = {12,1,0,0};

#ifdef MAVLINK10
uint8_t get_mavlink_message_crc(uint8_t msgid)
{
    switch (msgid)
    {
    case MAVLINK_MSG_ID_HEARTBEAT:    //id=0
        return 50;
    case MAVLINK_MSG_ID_SYS_STATUS:   //id=1
        return 124;
    case MAVLINK_MSG_ID_GPS_RAW_INT:  //id=24
        return 24;
    case MAVLINK_MSG_ID_GPS_STATUS:   //id=25
        return 23;
    case MAVLINK_MSG_ID_VFR_HUD:      //id=74
        return 20;
    case MAVLINK_MSG_ID_ATTITUDE:     //id=30
        return 39;
    default:  //ignore all others
        return 0;
    }
}
#endif

void MavlinkClient::RequestMavlinkRates()
{
    const int  maxStreams = 6;
    const uint8_t MAVStreams[maxStreams] = {MAV_DATA_STREAM_RAW_SENSORS,
                                            MAV_DATA_STREAM_EXTENDED_STATUS,
                                            MAV_DATA_STREAM_RC_CHANNELS,
                                            MAV_DATA_STREAM_POSITION,
                                            MAV_DATA_STREAM_EXTRA1,
                                            MAV_DATA_STREAM_EXTRA2
                                           };
    const uint16_t MAVRates[maxStreams] = {0x02, 0x02, 0x05, 0x02, 0x05, 0x02};
    for (int i=0; i < maxStreams; i++)
    {
        mavlink_msg_request_data_stream_send(MAVLINK_COMM_0,
                                             apm_mav_system, apm_mav_component,
                                             MAVStreams[i], MAVRates[i], 1);
    }
}

void MavlinkClient::ParseMavlink(uint8_t c)
{
    mavlink_message_t* msg;
    mavlink_status_t* status;

    //trying to grab msg
    if(_mavlink_parse_char(MAVLINK_COMM_0, c, &msg, &status))
    {
        //mavlink_active = 1;
        //handle msg
        switch(msg->msgid)
        {
        case MAVLINK_MSG_ID_HEARTBEAT:
        {
            mavbeat = 1;
            apm_mav_system    = msg->sysid;
            apm_mav_component = msg->compid;
            apm_mav_type      = mavlink_msg_heartbeat_get_type(msg);
#ifdef MAVLINK10
            osd_mode = mavlink_msg_heartbeat_get_custom_mode(msg);
            osd_nav_mode = 0;
            //osd_sys_status = mavlink_msg_heartbeat_get_system_status(msg);
            osd_sys_status =
                (mavlink_msg_heartbeat_get_base_mode(msg) & (uint8_t)MAV_MODE_FLAG_SAFETY_ARMED)
                == (uint8_t)MAV_MODE_FLAG_SAFETY_ARMED? MAV_STATE_ACTIVE: MAV_STATE_STANDBY;
#endif
            lastMAVBeat = millis();
            if (mavlink_status == (uint8_t)MAVLINK_STATUS_INACTIVE
                    || mavlink_status == (uint8_t)MAVLINK_STATUS_WAIT_HEARTBEAT)
                mavlink_status = (uint8_t)MAVLINK_STATUS_REQUIRE_DATA;
            /*if(waitingMAVBeats == 1)
            {
                enable_mav_request = 1;
            }*/
        }
        break;
        case MAVLINK_MSG_ID_SYS_STATUS:
        {
#ifndef MAVLINK10
            osd_vbat_A = (mavlink_msg_sys_status_get_vbat(msg) / 1000.0f);
            osd_mode = mavlink_msg_sys_status_get_mode(msg);
            osd_nav_mode = mavlink_msg_sys_status_get_nav_mode(msg);
            osd_sys_status = mavlink_msg_sys_status_get_status(msg);
#else
            osd_vbat_A = (mavlink_msg_sys_status_get_voltage_battery(msg) / 1000.0f);
            osd_curr_A = (mavlink_msg_sys_status_get_current_battery(msg) / 100.0f);
            if (osd_curr_A < 0.0f)
            	osd_curr_A = 0.0f;
#endif
            //osd_battery_remaining_A = mavlink_msg_sys_status_get_battery_remaining(msg);
            //osd_mode = apm_mav_component;//Debug
            //osd_nav_mode = apm_mav_system;//Debug
            if (mavlink_status == MAVLINK_STATUS_WAIT_DATA)
                mavlink_status = MAVLINK_STATUS_GET_DATA;
            //else
            //    mavlink_status = MAVLINK_STATUS_UPDATE_DATA;
        }
        break;
#ifndef MAVLINK10
        case MAVLINK_MSG_ID_GPS_RAW:
        {
            osd_lat = mavlink_msg_gps_raw_get_lat(msg);
            osd_lon = mavlink_msg_gps_raw_get_lon(msg);
            //osd_alt = mavlink_msg_gps_raw_get_alt(&msg);
            osd_fix_type = mavlink_msg_gps_raw_get_fix_type(msg);
        }
        break;
        case MAVLINK_MSG_ID_GPS_STATUS:
            osd_satellites_visible = mavlink_msg_gps_status_get_satellites_visible(msg);
            break;
#else
        case MAVLINK_MSG_ID_GPS_RAW_INT:
        {
            osd_lat = mavlink_msg_gps_raw_int_get_lat(msg) / 10000000;
            osd_lon = mavlink_msg_gps_raw_int_get_lon(msg) / 10000000;
            //osd_alt = mavlink_msg_gps_raw_get_alt(&msg);
            osd_fix_type = mavlink_msg_gps_raw_int_get_fix_type(msg);
        }
        break;
#endif
        break;
        case MAVLINK_MSG_ID_VFR_HUD:
        {
            osd_groundspeed = mavlink_msg_vfr_hud_get_groundspeed(msg);
            osd_heading = mavlink_msg_vfr_hud_get_heading(msg);// * 3.60f;//0-100% of 360
            osd_throttle = mavlink_msg_vfr_hud_get_throttle(msg);
            if(osd_throttle > 100 && osd_throttle < 150) osd_throttle = 100;//Temporary fix for ArduPlane 2.28
            if(osd_throttle < 0 || osd_throttle > 150) osd_throttle = 0;//Temporary fix for ArduPlane 2.28
            osd_alt = mavlink_msg_vfr_hud_get_alt(msg);
        }
        break;
        case MAVLINK_MSG_ID_ATTITUDE:
        {
            osd_pitch = ToDeg(mavlink_msg_attitude_get_pitch(msg));
            osd_roll = ToDeg(mavlink_msg_attitude_get_roll(msg));
            osd_yaw = ToDeg(mavlink_msg_attitude_get_yaw(msg));
        }
        break;
        default:
            //Do nothing
            break;
        }
    }


    //_delay_us(138);
    //delayMicroseconds(138);
    //wdt_reset();
    //next one

}