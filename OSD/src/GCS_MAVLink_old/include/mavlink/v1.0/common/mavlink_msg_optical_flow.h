// MESSAGE OPTICAL_FLOW PACKING

#define MAVLINK_MSG_ID_OPTICAL_FLOW 100

typedef struct __mavlink_optical_flow_t
{
    uint64_t time_usec; ///< Timestamp (UNIX)
    float flow_comp_m_x; ///< Flow in meters in x-sensor direction, angular-speed compensated
    float flow_comp_m_y; ///< Flow in meters in y-sensor direction, angular-speed compensated
    float ground_distance; ///< Ground distance in meters. Positive value: distance known. Negative value: Unknown distance
    int16_t flow_x; ///< Flow in pixels in x-sensor direction
    int16_t flow_y; ///< Flow in pixels in y-sensor direction
    uint8_t sensor_id; ///< Sensor ID
    uint8_t quality; ///< Optical flow quality / confidence. 0: bad, 255: maximum quality
} mavlink_optical_flow_t;

#define MAVLINK_MSG_ID_OPTICAL_FLOW_LEN 26
#define MAVLINK_MSG_ID_100_LEN 26



#define MAVLINK_MESSAGE_INFO_OPTICAL_FLOW { \
	"OPTICAL_FLOW", \
	8, \
	{  { "time_usec", NULL, MAVLINK_TYPE_UINT64_T, 0, 0, offsetof(mavlink_optical_flow_t, time_usec) }, \
         { "flow_comp_m_x", NULL, MAVLINK_TYPE_FLOAT, 0, 8, offsetof(mavlink_optical_flow_t, flow_comp_m_x) }, \
         { "flow_comp_m_y", NULL, MAVLINK_TYPE_FLOAT, 0, 12, offsetof(mavlink_optical_flow_t, flow_comp_m_y) }, \
         { "ground_distance", NULL, MAVLINK_TYPE_FLOAT, 0, 16, offsetof(mavlink_optical_flow_t, ground_distance) }, \
         { "flow_x", NULL, MAVLINK_TYPE_INT16_T, 0, 20, offsetof(mavlink_optical_flow_t, flow_x) }, \
         { "flow_y", NULL, MAVLINK_TYPE_INT16_T, 0, 22, offsetof(mavlink_optical_flow_t, flow_y) }, \
         { "sensor_id", NULL, MAVLINK_TYPE_UINT8_T, 0, 24, offsetof(mavlink_optical_flow_t, sensor_id) }, \
         { "quality", NULL, MAVLINK_TYPE_UINT8_T, 0, 25, offsetof(mavlink_optical_flow_t, quality) }, \
         } \
}


/**
 * @brief Pack a optical_flow message
 * @param system_id ID of this system
 * @param component_id ID of this component (e.g. 200 for IMU)
 * @param msg The MAVLink message to compress the data into
 *
 * @param time_usec Timestamp (UNIX)
 * @param sensor_id Sensor ID
 * @param flow_x Flow in pixels in x-sensor direction
 * @param flow_y Flow in pixels in y-sensor direction
 * @param flow_comp_m_x Flow in meters in x-sensor direction, angular-speed compensated
 * @param flow_comp_m_y Flow in meters in y-sensor direction, angular-speed compensated
 * @param quality Optical flow quality / confidence. 0: bad, 255: maximum quality
 * @param ground_distance Ground distance in meters. Positive value: distance known. Negative value: Unknown distance
 * @return length of the message in bytes (excluding serial stream start sign)
 */
static inline uint16_t mavlink_msg_optical_flow_pack(uint8_t system_id, uint8_t component_id, mavlink_message_t* msg,
        uint64_t time_usec, uint8_t sensor_id, int16_t flow_x, int16_t flow_y, float flow_comp_m_x, float flow_comp_m_y, uint8_t quality, float ground_distance)
{
#if MAVLINK_NEED_BYTE_SWAP || !MAVLINK_ALIGNED_FIELDS
    char buf[26];
    _mav_put_uint64_t(buf, 0, time_usec);
    _mav_put_float(buf, 8, flow_comp_m_x);
    _mav_put_float(buf, 12, flow_comp_m_y);
    _mav_put_float(buf, 16, ground_distance);
    _mav_put_int16_t(buf, 20, flow_x);
    _mav_put_int16_t(buf, 22, flow_y);
    _mav_put_uint8_t(buf, 24, sensor_id);
    _mav_put_uint8_t(buf, 25, quality);

    memcpy(_MAV_PAYLOAD_NON_CONST(msg), buf, 26);
#else
    mavlink_optical_flow_t packet;
    packet.time_usec = time_usec;
    packet.flow_comp_m_x = flow_comp_m_x;
    packet.flow_comp_m_y = flow_comp_m_y;
    packet.ground_distance = ground_distance;
    packet.flow_x = flow_x;
    packet.flow_y = flow_y;
    packet.sensor_id = sensor_id;
    packet.quality = quality;

    memcpy(_MAV_PAYLOAD_NON_CONST(msg), &packet, 26);
#endif

    msg->msgid = MAVLINK_MSG_ID_OPTICAL_FLOW;
    return mavlink_finalize_message(msg, system_id, component_id, 26, 175);
}

/**
 * @brief Pack a optical_flow message on a channel
 * @param system_id ID of this system
 * @param component_id ID of this component (e.g. 200 for IMU)
 * @param chan The MAVLink channel this message was sent over
 * @param msg The MAVLink message to compress the data into
 * @param time_usec Timestamp (UNIX)
 * @param sensor_id Sensor ID
 * @param flow_x Flow in pixels in x-sensor direction
 * @param flow_y Flow in pixels in y-sensor direction
 * @param flow_comp_m_x Flow in meters in x-sensor direction, angular-speed compensated
 * @param flow_comp_m_y Flow in meters in y-sensor direction, angular-speed compensated
 * @param quality Optical flow quality / confidence. 0: bad, 255: maximum quality
 * @param ground_distance Ground distance in meters. Positive value: distance known. Negative value: Unknown distance
 * @return length of the message in bytes (excluding serial stream start sign)
 */
static inline uint16_t mavlink_msg_optical_flow_pack_chan(uint8_t system_id, uint8_t component_id, uint8_t chan,
        mavlink_message_t* msg,
        uint64_t time_usec,uint8_t sensor_id,int16_t flow_x,int16_t flow_y,float flow_comp_m_x,float flow_comp_m_y,uint8_t quality,float ground_distance)
{
#if MAVLINK_NEED_BYTE_SWAP || !MAVLINK_ALIGNED_FIELDS
    char buf[26];
    _mav_put_uint64_t(buf, 0, time_usec);
    _mav_put_float(buf, 8, flow_comp_m_x);
    _mav_put_float(buf, 12, flow_comp_m_y);
    _mav_put_float(buf, 16, ground_distance);
    _mav_put_int16_t(buf, 20, flow_x);
    _mav_put_int16_t(buf, 22, flow_y);
    _mav_put_uint8_t(buf, 24, sensor_id);
    _mav_put_uint8_t(buf, 25, quality);

    memcpy(_MAV_PAYLOAD_NON_CONST(msg), buf, 26);
#else
    mavlink_optical_flow_t packet;
    packet.time_usec = time_usec;
    packet.flow_comp_m_x = flow_comp_m_x;
    packet.flow_comp_m_y = flow_comp_m_y;
    packet.ground_distance = ground_distance;
    packet.flow_x = flow_x;
    packet.flow_y = flow_y;
    packet.sensor_id = sensor_id;
    packet.quality = quality;

    memcpy(_MAV_PAYLOAD_NON_CONST(msg), &packet, 26);
#endif

    msg->msgid = MAVLINK_MSG_ID_OPTICAL_FLOW;
    return mavlink_finalize_message_chan(msg, system_id, component_id, chan, 26, 175);
}

/**
 * @brief Encode a optical_flow struct into a message
 *
 * @param system_id ID of this system
 * @param component_id ID of this component (e.g. 200 for IMU)
 * @param msg The MAVLink message to compress the data into
 * @param optical_flow C-struct to read the message contents from
 */
static inline uint16_t mavlink_msg_optical_flow_encode(uint8_t system_id, uint8_t component_id, mavlink_message_t* msg, const mavlink_optical_flow_t* optical_flow)
{
    return mavlink_msg_optical_flow_pack(system_id, component_id, msg, optical_flow->time_usec, optical_flow->sensor_id, optical_flow->flow_x, optical_flow->flow_y, optical_flow->flow_comp_m_x, optical_flow->flow_comp_m_y, optical_flow->quality, optical_flow->ground_distance);
}

/**
 * @brief Send a optical_flow message
 * @param chan MAVLink channel to send the message
 *
 * @param time_usec Timestamp (UNIX)
 * @param sensor_id Sensor ID
 * @param flow_x Flow in pixels in x-sensor direction
 * @param flow_y Flow in pixels in y-sensor direction
 * @param flow_comp_m_x Flow in meters in x-sensor direction, angular-speed compensated
 * @param flow_comp_m_y Flow in meters in y-sensor direction, angular-speed compensated
 * @param quality Optical flow quality / confidence. 0: bad, 255: maximum quality
 * @param ground_distance Ground distance in meters. Positive value: distance known. Negative value: Unknown distance
 */
#ifdef MAVLINK_USE_CONVENIENCE_FUNCTIONS

static inline void mavlink_msg_optical_flow_send(mavlink_channel_t chan, uint64_t time_usec, uint8_t sensor_id, int16_t flow_x, int16_t flow_y, float flow_comp_m_x, float flow_comp_m_y, uint8_t quality, float ground_distance)
{
#if MAVLINK_NEED_BYTE_SWAP || !MAVLINK_ALIGNED_FIELDS
    char buf[26];
    _mav_put_uint64_t(buf, 0, time_usec);
    _mav_put_float(buf, 8, flow_comp_m_x);
    _mav_put_float(buf, 12, flow_comp_m_y);
    _mav_put_float(buf, 16, ground_distance);
    _mav_put_int16_t(buf, 20, flow_x);
    _mav_put_int16_t(buf, 22, flow_y);
    _mav_put_uint8_t(buf, 24, sensor_id);
    _mav_put_uint8_t(buf, 25, quality);

    _mav_finalize_message_chan_send(chan, MAVLINK_MSG_ID_OPTICAL_FLOW, buf, 26, 175);
#else
    mavlink_optical_flow_t packet;
    packet.time_usec = time_usec;
    packet.flow_comp_m_x = flow_comp_m_x;
    packet.flow_comp_m_y = flow_comp_m_y;
    packet.ground_distance = ground_distance;
    packet.flow_x = flow_x;
    packet.flow_y = flow_y;
    packet.sensor_id = sensor_id;
    packet.quality = quality;

    _mav_finalize_message_chan_send(chan, MAVLINK_MSG_ID_OPTICAL_FLOW, (const char *)&packet, 26, 175);
#endif
}

#endif

// MESSAGE OPTICAL_FLOW UNPACKING


/**
 * @brief Get field time_usec from optical_flow message
 *
 * @return Timestamp (UNIX)
 */
static inline uint64_t mavlink_msg_optical_flow_get_time_usec(const mavlink_message_t* msg)
{
    return _MAV_RETURN_uint64_t(msg,  0);
}

/**
 * @brief Get field sensor_id from optical_flow message
 *
 * @return Sensor ID
 */
static inline uint8_t mavlink_msg_optical_flow_get_sensor_id(const mavlink_message_t* msg)
{
    return _MAV_RETURN_uint8_t(msg,  24);
}

/**
 * @brief Get field flow_x from optical_flow message
 *
 * @return Flow in pixels in x-sensor direction
 */
static inline int16_t mavlink_msg_optical_flow_get_flow_x(const mavlink_message_t* msg)
{
    return _MAV_RETURN_int16_t(msg,  20);
}

/**
 * @brief Get field flow_y from optical_flow message
 *
 * @return Flow in pixels in y-sensor direction
 */
static inline int16_t mavlink_msg_optical_flow_get_flow_y(const mavlink_message_t* msg)
{
    return _MAV_RETURN_int16_t(msg,  22);
}

/**
 * @brief Get field flow_comp_m_x from optical_flow message
 *
 * @return Flow in meters in x-sensor direction, angular-speed compensated
 */
static inline float mavlink_msg_optical_flow_get_flow_comp_m_x(const mavlink_message_t* msg)
{
    return _MAV_RETURN_float(msg,  8);
}

/**
 * @brief Get field flow_comp_m_y from optical_flow message
 *
 * @return Flow in meters in y-sensor direction, angular-speed compensated
 */
static inline float mavlink_msg_optical_flow_get_flow_comp_m_y(const mavlink_message_t* msg)
{
    return _MAV_RETURN_float(msg,  12);
}

/**
 * @brief Get field quality from optical_flow message
 *
 * @return Optical flow quality / confidence. 0: bad, 255: maximum quality
 */
static inline uint8_t mavlink_msg_optical_flow_get_quality(const mavlink_message_t* msg)
{
    return _MAV_RETURN_uint8_t(msg,  25);
}

/**
 * @brief Get field ground_distance from optical_flow message
 *
 * @return Ground distance in meters. Positive value: distance known. Negative value: Unknown distance
 */
static inline float mavlink_msg_optical_flow_get_ground_distance(const mavlink_message_t* msg)
{
    return _MAV_RETURN_float(msg,  16);
}

/**
 * @brief Decode a optical_flow message into a struct
 *
 * @param msg The message to decode
 * @param optical_flow C-struct to decode the message contents into
 */
static inline void mavlink_msg_optical_flow_decode(const mavlink_message_t* msg, mavlink_optical_flow_t* optical_flow)
{
#if MAVLINK_NEED_BYTE_SWAP
    optical_flow->time_usec = mavlink_msg_optical_flow_get_time_usec(msg);
    optical_flow->flow_comp_m_x = mavlink_msg_optical_flow_get_flow_comp_m_x(msg);
    optical_flow->flow_comp_m_y = mavlink_msg_optical_flow_get_flow_comp_m_y(msg);
    optical_flow->ground_distance = mavlink_msg_optical_flow_get_ground_distance(msg);
    optical_flow->flow_x = mavlink_msg_optical_flow_get_flow_x(msg);
    optical_flow->flow_y = mavlink_msg_optical_flow_get_flow_y(msg);
    optical_flow->sensor_id = mavlink_msg_optical_flow_get_sensor_id(msg);
    optical_flow->quality = mavlink_msg_optical_flow_get_quality(msg);
#else
    memcpy(optical_flow, _MAV_PAYLOAD(msg), 26);
#endif
}
