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
        //XXC, //Free
        SYS,
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
            "Battery B (ADC)",
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
            "RSSI (ADC)",

            null,
            null,
            "Altitude", 
            "Velocity", 
            "Throttle", 
            "Flight Mode", 
            "Horizon",
            "System Status"};

        static readonly string[] name_zh = {
            "中心", 
            "俯仰", 
            "侧倾", 
            "电池A", 
            "电池B (ADC)", 
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
            "RSSI (ADC)",

            null,
            null,
            "高度", 
            "速度", 
            "油门", 
            "飞行模式", 
            "水平",
            "系统状态"};
    }

    public class ADSetting
    {
        public byte channel;
        public float k;
        public float b;

        public byte[] ToBytes()
        {
            byte[] buf = new byte[sizeof(byte) + sizeof(float) * 2];
            buf[0] = channel;
            Array.Copy(BitConverter.GetBytes(k), 0, buf, 1, sizeof(float));
            Array.Copy(BitConverter.GetBytes(k), 0, buf, 1 + sizeof(float), sizeof(float));
            return buf;
        }

        public void FromBytes(byte[] data, int offset)
        {
            channel = data[offset];
            k = BitConverter.ToSingle(data, offset + 1);
            b = BitConverter.ToSingle(data, offset + 1 + sizeof(float));
        }

        public const int Size = sizeof(byte) + sizeof(float) * 2;
    }

    public class OSDSetting
    {
        public UInt32 enable = _BV(OSDItem.Pit) | _BV(OSDItem.Rol) | _BV(OSDItem.BatA) | _BV(OSDItem.GPSats) | _BV(OSDItem.GPL) | _BV(OSDItem.GPS)
        | _BV(OSDItem.Rose) | _BV(OSDItem.Head) | _BV(OSDItem.MavB) | _BV(OSDItem.HDir) | _BV(OSDItem.HDis)
        | _BV(OSDItem.Alt) | _BV(OSDItem.Vel) | _BV(OSDItem.Thr) | _BV(OSDItem.FMod) | _BV(OSDItem.Hor) | _BV(OSDItem.SYS);

        public byte[,] coord = new byte[24, 2]
        {
            {13, 7}, //  panCenter_y_ADDR
            {22, 9}, //  panPitch_y_ADDR
            {11, 1}, //  panRoll_y_ADDR
            {21, 1}, //  panBatt_A_y_ADDR
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
            {22, 5}, ////  panRSSI_y_ADDR
            {21, 2}, //  panCur_A_y_ADDR
            {21, 4}, //  panCur_B_y_ADDR
            {2, 2}, //  panAlt_y_ADDR
            {2, 3}, //  panVel_y_ADDR
            {2, 4}, //  panThr_y_ADDR
            {17, 13}, // panFMod_y_ADDR
            {8, 7}, //  panHorizon_y_ADDR
            {11,4}
        };
        public ADSetting vbat_b;
        public ADSetting rssi;

        static UInt32 _BV(OSDItem bi)
        {
            return 1U << (int)bi;
            //_BV(Pit_BIT) | _BV(Rol_BIT) | _BV(BatA_BIT) | _BV(GPSats_BIT) | _BV(GPL_BIT) | _BV(GPS_BIT),
        }

        public bool IsEnabled(OSDItem info)
        {
            return (enable & (1U << (int)info)) != 0;
        }

        public byte[] ToBytes()
        {
            byte[] v = vbat_b.ToBytes();
            byte[] r = rssi.ToBytes();
            int size = sizeof(uint) + coord.Length + v.Length + r.Length;
            byte[] buf = new byte[size];
            Array.Copy(BitConverter.GetBytes(enable), 0, buf, 0, sizeof(uint));
            Buffer.BlockCopy(coord, 0, buf, sizeof(uint), coord.Length);

            Buffer.BlockCopy(v, 0, buf, sizeof(uint) + coord.Length, v.Length);
            Buffer.BlockCopy(r, 0, buf, sizeof(uint) + coord.Length + v.Length, r.Length);
            return buf;
        }

        public void FromBytes(byte[] data, int offset)
        {
            enable = BitConverter.ToUInt32(data, offset);
            Buffer.BlockCopy(data, offset + sizeof(uint), coord, 0, coord.Length);
            vbat_b.FromBytes(data, offset + sizeof(uint) + coord.Length);
            rssi.FromBytes(data, offset + sizeof(uint) + coord.Length + ADSetting.Size);
        }
    }
}
