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
        HAlt,
        Vel,
        AS,
        Thr,
        FMod,
        Hor,
        //XXC, //Free
        SYS,
        //BatA_ADC,
        //BatB_ADC,
        //CurA_ADC,
        //CurB_ADC,
        //RSSI_ADC,
        //Alt_R,
        NULL
    }

    public static class OSDItemList
    {
        /*
        public static string[] Names
        {
            get
            {
                if (Thread.CurrentThread.CurrentUICulture.Name.StartsWith("zh-"))
                    return name_zh;
                else
                    return name_en;
            }
        }*/

        public static readonly KeyValuePair<OSDItem, OSDItem>[] Conflits = 
        {
            new KeyValuePair<OSDItem, OSDItem>(OSDItem.Cen, OSDItem.Hor)
        };

/*
        public static readonly KeyValuePair<OSDItem, OSDItem>[] Alternates = 
        {
            new KeyValuePair<OSDItem, OSDItem>(OSDItem.BatA, OSDItem.BatA_ADC),
            new KeyValuePair<OSDItem, OSDItem>(OSDItem.BatB, OSDItem.BatB_ADC),
            new KeyValuePair<OSDItem, OSDItem>(OSDItem.RSSI, OSDItem.RSSI_ADC),
            new KeyValuePair<OSDItem, OSDItem>(OSDItem.Alt, OSDItem.Alt_R),
            new KeyValuePair<OSDItem, OSDItem>(OSDItem.CurA, OSDItem.CurA_ADC),
            new KeyValuePair<OSDItem, OSDItem>(OSDItem.CurB, OSDItem.CurB_ADC)
        };
*/
        public static readonly OSDItem[] Avaliable = { 
        OSDItem.Cen,
        OSDItem.Pit,
        OSDItem.Rol,
        OSDItem.BatA,
        OSDItem.BatB,
        //OSDItem.BatB_ADC,
        OSDItem.CurA,
        //OSDItem.CurA_ADC,
        OSDItem.CurB,
        OSDItem.GPSats,
        OSDItem.GPL,
        OSDItem.GPS,

        
        OSDItem.Rose,
        OSDItem.Head,
        OSDItem.MavB,
        OSDItem.HDir,
        OSDItem.HDis,
        OSDItem.WDir,
        OSDItem.WDis,
        OSDItem.RSSI,
        //OSDItem.RSSI_ADC,

        OSDItem.Alt,
        OSDItem.HAlt,
        OSDItem.Vel,
        OSDItem.AS,
        OSDItem.Thr,
        OSDItem.FMod,
        OSDItem.Hor,

        OSDItem.SYS};
        /*

        static readonly string[] name_en = {
            "Center", 
            "Pitch", 
            "Roll", 
            "Voltage A",
            "Voltage B",
            "Visible Sats", 
            "GPS Lock", 
            "GPS Coord", 

            "Heading Rose", 
            "Heading", 
            "Heart Beat", 
            "Home Direction", 
            "Home Distance",
            "Waypoint Direction",
            "Waypoint Distance",
            "RSSI",

            "Current A",
            "Current B",
            "Altitude (Absolute)", 
            "Velocity", 
            "Throttle", 
            "Flight Mode", 
            "Horizon",
            "System Status",

            "Voltage A (AD)",
            "Voltage B (AD)",
            "Current A (AD)",
            "Current B (AD)",
            "RSSI (AD)",
            "Altitude (Relative)"
                                           };

        static readonly string[] name_zh = {
            "中心", 
            "俯仰", 
            "侧倾", 
            "电压",
            "电池B电压",
            "卫星数量", 
            "GPS锁定", 
            "GPS坐标", 

            "航向方位圈", 
            "航向", 
            "心跳", 
            "回家方向", 
            "回家距离", 
            "航点方向",
            "航点距离",
            "信号强度",

            "电池A电流",
            "电池B电流",
            
            "海拔高度", 
            "速度", 
            "油门", 
            "飞行模式", 
            "水平",
            "系统状态",
            "电池A电压 (AD)",
            "电池B电压 (AD)",
            "电池A电流 (AD)",
            "电池B电流 (AD)",
            "信号强度 (AD)",
            "相对高度",
            };*/
    }
}