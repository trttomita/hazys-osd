using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace OSDConfig
{
    public enum OSDItem
    {
        Cen,
        Pit,
        Rol,
        BatA,
        BatB,  //(!Not implemented)
        GPSats,
        GPL,
        GPS,

        // panB_REG Byte has:
        Rose,
        Head,
        MavB,
        HDir,
        HDis,
        WDir, //(!Not implemented)
        WDis, //(!Not implemented)
        RSSI, //(!Not implemented)

        // panC_REG Byte has:
        CurA, //(!Not implemented)
        CurB, //(!Not implemented)
        Alt,
        Vel,
        Thr,
        FMod,
        Hor,
        XXC, //Free
        NULL
    }

    public class OSDItemName
    {
        public static string[] Name
        {
            get
            {
                if (Thread.CurrentThread.CurrentUICulture.Name.StartsWith("zh-"))
                    return name_zh;
                else
                    return name_en;
            }
        }

        static readonly string[] name_en = {
            "Center", 
            "Pitch", 
            "Roll", 
            "Battery A", 
            null, 
            "Visible Sats", 
            "GPS Lock", 
            "GPS Coord", 

            "Heading Rose", 
            "Heading", 
            "Heart Beat", 
            "Home Direction", 
            "Home Distance", 
            null,
            null,
            null,

            null,
            null,
            "Altitude", 
            "Velocity", 
            "Throttle", 
            "Flight Mode", 
            "Horizon"};

        static readonly string[] name_zh = {
            "中心", 
            "俯仰", 
            "侧倾", 
            "电池A", 
            null, 
            "卫星数量", 
            "GPS锁定", 
            "GPS坐标", 

            "航向方位圈", 
            "航向", 
            "心跳", 
            "回家方向", 
            "回家距离", 
            null,
            null,
            null,

            null,
            null,
            "高度", 
            "速度", 
            "油门", 
            "飞行模式", 
            "水平"};
    }

    public class OSDSetting
    {
        public UInt32 enable = _BV(OSDItem.Pit) | _BV(OSDItem.Rol) | _BV(OSDItem.BatA) | _BV(OSDItem.GPSats) | _BV(OSDItem.GPL) | _BV(OSDItem.GPS)
        | _BV(OSDItem.Rose) | _BV(OSDItem.Head) | _BV(OSDItem.MavB) | _BV(OSDItem.HDir) | _BV(OSDItem.HDis)
        | _BV(OSDItem.Alt) | _BV(OSDItem.Vel) | _BV(OSDItem.Thr) | _BV(OSDItem.FMod) | _BV(OSDItem.Hor);

        public byte[,] coord = new byte[24, 2]
        {
            {13, 7}, //  panCenter_y_ADDR
            {22, 9}, //  panPitch_y_ADDR
            {11, 1}, //  panRoll_y_ADDR
            {20, 1}, //  panBatt_A_y_ADDR
            {21, 3}, //  panBatt_B_y_ADDR
            {2, 13}, // panGPSats_y_ADDR
            {5, 13}, // panGPL_y_ADDR
            {2, 14}, // panGPS_y_ADDR
            {16, 14}, // panRose_y_ADDR
            {24, 13}, // panHeading_y_ADDR
            {2, 9}, // panMavBeat_y_ADDR
            {14, 3}, //  panHomeDir_y_ADDR
            {2, 1}, //  panHomeDis_y_ADDR
            {0, 0}, //  panWPDir_y_ADDR
            {0, 0}, //  panWPDis_y_ADDR
            {21, 5}, ////  panRSSI_y_ADDR
            {21, 2}, //  panCur_A_y_ADDR
            {21, 4}, //  panCur_B_y_ADDR
            {2, 2}, //  panAlt_y_ADDR
            {2, 3}, //  panVel_y_ADDR
            {2, 4}, //  panThr_y_ADDR
            {17, 13}, // panFMod_y_ADDR
            {8, 7}, //  panHorizon_y_ADDR
            {0,0}
        };


        static UInt32 _BV(OSDItem bi)
        {
            return 1U << (int)bi;
            //_BV(Pit_BIT) | _BV(Rol_BIT) | _BV(BatA_BIT) | _BV(GPSats_BIT) | _BV(GPL_BIT) | _BV(GPS_BIT),
        }

        public bool IsEnabled(OSDItem info)
        {
            return (enable & (1U << (int)info)) != 0;
        }
    }
}
