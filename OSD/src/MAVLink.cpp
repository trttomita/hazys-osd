#define MAVLINK_COMM_NUM_BUFFERS 1
#define MAVLINK_USE_CONVENIENCE_FUNCTIONS
#define MAVLINK_MAX_PAYLOAD_LEN 127



#ifdef MAVLINK10
#ifndef MAVLINK_CRC_EXTRA
#define MAVLINK_CRC_EXTRA 1
#endif
#include "../GCS_MAVLink/include_v1.0/mavlink_types.h"
#include "../GCS_MAVLink/include_v1.0/ardupilotmega/mavlink.h"
#else
#include "../GCS_MAVLink/include/mavlink_types.h"
#include "../GCS_MAVLink/include/ardupilotmega/mavlink.h"
#endif

#include "Mavlink_compat.h"

#include <avr/wdt.h>
#include "ArduOSD.h"
#include "time.h"
#include "uart.h"


// true when we have received at least 1 MAVLink packet



static int packet_drops = 0;
static int parse_error = 0;

mavlink_system_t mavlink_system = {12,1,0,0};

#define ToDeg(x) (x*57.2957795131)  // *180/pi

void ArduOSD::request_mavlink_rates()
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

void ArduOSD::read_mavlink()
{

    mavlink_message_t* msg;
    mavlink_status_t* status;

    // this might need to move to the flight software

    //grabing data

    while(uart_available() > 0)
    {
        uint8_t c = uart_getc();//serial_getc();
        /* allow CLI to be started by hitting enter 3 times, if no
        heartbeat packets have been received */
        if (mavlink_active == 0 /*&& millis() < 20000*/)
        {
            switch (c)
            {
            case '\r':
                crlf_count++;
                break;
            case '\n':
                break;
            default:
                if (crlf_count >= 3)
                {
                    switch (c)
                    {
                    case 'F':
                        uploadFont();
                        continue;
                    case 'S':
                        uploadSetting();
                        continue;
                    case 's':
                        getSetting();
                        continue;
                    case 'R':	//reboot
                        reboot();
                        continue;
                    }
                }
                crlf_count=0;
                break;
            }
        }

        //trying to grab msg
        if(mavlink_parse_char(MAVLINK_COMM_0, c, &msg, &status))
        {
            mavlink_active = 1;
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
#endif
                lastMAVBeat = millis();
                if(waitingMAVBeats == 1)
                {
                    enable_mav_request = 1;
                }
            }
            break;
            case MAVLINK_MSG_ID_SYS_STATUS:
            {
#ifndef MAVLINK10
                osd_vbat_A = (mavlink_msg_sys_status_get_vbat(msg) / 1000.0f);
                osd_mode = mavlink_msg_sys_status_get_mode(msg);
                osd_nav_mode = mavlink_msg_sys_status_get_nav_mode(msg);
#else
                osd_vbat_A = (mavlink_msg_sys_status_get_voltage_battery(msg) / 1000.0f);
#endif
                osd_battery_remaining_A = mavlink_msg_sys_status_get_battery_remaining(msg);
                //osd_mode = apm_mav_component;//Debug
                //osd_nav_mode = apm_mav_system;//Debug
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
            case MAVLINK_MSG_ID_GPS_STATUS:
            {
                //	uart_putc('8');
                osd_satellites_visible = mavlink_msg_gps_status_get_satellites_visible(msg);
            }
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
        delayMicroseconds(138);
        //wdt_reset();
        //next one
    }
    // Update global packet drops counter
    packet_drops += status->packet_rx_drop_count;
    parse_error += status->parse_error;

    //uart_putc('6');
}

void ArduOSD::read_data()
{
    if(enable_mav_request == 1) //Request rate control
    {
        clear();
        setPanel(3,10);
        openPanel();
        print_P(PSTR("Requesting DataStreams..."));
        closePanel();
        for(int n = 0; n < 3; n++)
        {
            request_mavlink_rates();//Three times to certify it will be readed
            delay(50);
        }
        enable_mav_request = 0;
        for (uint8_t i = 0; i < 10; i++)
        {
            delay(200);
            wdt_reset();
        }
        clear();
        waitingMAVBeats = 0;
        lastMAVBeat = millis();//Preventing error from delay sensing
    }
    read_mavlink();
}
