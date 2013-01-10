using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace OSDConfig
{
    public class ADSetting
    {
        public byte channel = 0;
        public float k = 1;
        public float b = 0;

        public byte[] ToBytes()
        {
            byte[] buf = new byte[sizeof(byte) + sizeof(float) * 2];
            buf[0] = channel;
            Array.Copy(BitConverter.GetBytes(k), 0, buf, 1, sizeof(float));
            Array.Copy(BitConverter.GetBytes(b), 0, buf, 1 + sizeof(float), sizeof(float));
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

    public enum OSDOption
    {
        VBatA_ADC, VBatB_ADC, CurrA_ADC, CurrB_ADC, RSSI_ADC, M_ISO = 6, Alt_R,
    };

    public class OSDSetting
    {
        public const byte DataVersion = 6;

        public byte ver = DataVersion;
        public byte option = 0;

        public UInt32 enable = _BV(OSDItem.Pit) | _BV(OSDItem.Rol) | _BV(OSDItem.BatA) | _BV(OSDItem.GPSats) | _BV(OSDItem.GPL) | _BV(OSDItem.GPS)
        | _BV(OSDItem.Rose) | _BV(OSDItem.Head) | _BV(OSDItem.MavB) | _BV(OSDItem.HDir) | _BV(OSDItem.HDis)
        | _BV(OSDItem.Alt) | _BV(OSDItem.HAlt) | _BV(OSDItem.Vel) | _BV(OSDItem.AS) | _BV(OSDItem.Thr) | _BV(OSDItem.FMod) 
        | _BV(OSDItem.Hor) | _BV(OSDItem.SYS);


        public byte[,] coord = new byte[26, 2]
        {
            {13, 7}, //  panCenter_y_ADDR
            {22, 9}, //  panPitch_y_ADDR
            {11, 1}, //  panRoll_y_ADDR
            {1, 6}, //  panBatt_A_y_ADDR
            {1, 8}, //  panBatt_B_y_ADDR
            {1, 13}, // panGPSats_y_ADDR
            {4, 13}, // panGPL_y_ADDR
            {1, 14}, // panGPS_y_ADDR
            {16, 14}, // panRose_y_ADDR
            {24, 13}, // panHeading_y_ADDR
            {14, 15}, // panMavBeat_y_ADDR
            {14, 3}, //  panHomeDir_y_ADDR
            {22, 1}, //  panHomeDis_y_ADDR
            {17, 12}, //  panWPDir_y_ADDR
            {20, 12}, //  panWPDis_y_ADDR
            {22, 5}, ////  panRSSI_y_ADDR
            {1, 7}, //  panCur_A_y_ADDR
            {1, 9}, //  panCur_B_y_ADDR
            {22, 3}, //  panAlt_y_ADDR
            {22, 2}, //homeAlt
            {1, 2}, //  panVel_y_ADDR
            {1, 1}, // panAS
            {1, 4}, //  panThr_y_ADDR
            {17, 13}, // panFMod_y_ADDR
            {8, 6}, //  panHorizon_y_ADDR
            {11,4}
        };
        public ADSetting[] ad_setting = new ADSetting[] { 
            new ADSetting(), new ADSetting(), new ADSetting(),new ADSetting(), new ADSetting() };

        static UInt32 _BV(OSDItem bi)
        {
            return 1U << (int)bi;
            //_BV(Pit_BIT) | _BV(Rol_BIT) | _BV(BatA_BIT) | _BV(GPSats_BIT) | _BV(GPL_BIT) | _BV(GPS_BIT),
        }

        static UInt32 _BV(int i)
        {
            return 1U << i;
        }

        public bool IsEnabled(OSDItem info)
        {
            return (enable & (1U << (int)info)) != 0;
        }

        public void SetOption(OSDOption opt, bool on)
        {
            if (on)
                option = (byte)(option | _BV((int)opt));
            else
                option = (byte)(option & ~_BV((int)opt));
        }

        public bool GetOption(OSDOption opt)
        {
            return (option & _BV((int)opt)) != 0;
        }

        public byte[] ToBytes()
        {
            int size = 2 * sizeof(byte) + sizeof(uint) + coord.Length + ad_setting.Length * ADSetting.Size;

            byte[] buf = new byte[size];
            buf[0] = ver;
            buf[1] = option;
            Array.Copy(BitConverter.GetBytes(enable), 0, buf, 2 * sizeof(byte), sizeof(uint));
            Buffer.BlockCopy(coord, 0, buf, sizeof(uint) + 2 * sizeof(byte), coord.Length);
            for (int i = 0; i < ad_setting.Length; i++)
                Buffer.BlockCopy(ad_setting[i].ToBytes(), 0,
                    buf, 2 * sizeof(byte) + sizeof(uint) + coord.Length + i * ADSetting.Size, ADSetting.Size);


            return buf;
        }

        public bool FromBytes(byte[] data, int offset)
        {
            if (data[0] != DataVersion)
                return false;
            ver = data[0];
            option = data[1];
            enable = BitConverter.ToUInt32(data, offset + 2 * sizeof(byte));
            Buffer.BlockCopy(data, offset + 2 * sizeof(byte) + sizeof(uint), coord, 0, coord.Length);
            for (int i = 0; i < ad_setting.Length; i++)
                ad_setting[i].FromBytes(data, offset + 2 * sizeof(byte) + sizeof(uint) + coord.Length + i * ADSetting.Size);
            return true;
        }
    }
}
