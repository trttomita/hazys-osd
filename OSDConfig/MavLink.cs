using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace OSDConfig
{
    class MavLink
    {
        static int mavlinkversion=2;
        public static byte packetcount = 0;

        /// <summary>
        /// Generate a Mavlink Packet and write to serial
        /// </summary>
        /// <param name="messageType">type number</param>
        /// <param name="indata">struct of data</param>
        public static byte[] generatePacket(byte messageType, object indata)
        {
            byte[] data;

            /*if (mavlinkversion == 3)
            {
                data = MavlinkUtil.StructureToByteArray(indata);
            }
            else*/
            {
                data = StructureToByteArrayBigEndian(indata);
            }

            //Console.WriteLine(DateTime.Now + " PC Doing req "+ messageType + " " + this.BytesToRead);
            byte[] packet = new byte[data.Length + 6 + 2];

            if (mavlinkversion == 3)
            {
                packet[0] = 254;
            }
            else if (mavlinkversion == 2)
            {
                packet[0] = (byte)'U';
            }
            packet[1] = (byte)data.Length;
            packet[2] = packetcount;
            packet[3] = 255; // this is always 255 - MYGCS
//#if MAVLINK10
//            packet[4] = (byte)MAV_COMPONENT.MAV_COMP_ID_MISSIONPLANNER;
//#else
//            packet[4] = (byte)MAV_COMPONENT.MAV_COMP_ID_WAYPOINTPLANNER;
//#endif
            packet[4] = 0;
            packet[5] = messageType;

            int i = 6;
            foreach (byte b in data)
            {
                packet[i] = b;
                i++;
            }

            ushort checksum = crc_calculate(packet, packet[1] + 6);

            /*
            if (mavlinkversion == 3)
            {
                checksum = MavlinkCRC.crc_accumulate(MAVLINK_MESSAGE_CRCS[messageType], checksum);
            }*/

            byte ck_a = (byte)(checksum & 0xFF); ///< High byte
            byte ck_b = (byte)(checksum >> 8); ///< Low byte

            packet[i] = ck_a;
            i += 1;
            packet[i] = ck_b;
            i += 1;

            

            packetcount++;


            return packet;
            //System.Threading.Thread.Sleep(1);
        }

        /// <summary>
        /// Convert a struct to an array of bytes, struct fields being reperesented in 
        /// big endian (MSB first)
        /// </summary>
        public static byte[] StructureToByteArrayBigEndian(params object[] list)
        {
            // The copy is made becuase SetValue won't work on a struct.
            // Boxing was used because SetValue works on classes/objects.
            // Unfortunately, it results in 2 copy operations.
            object thisBoxed = list[0]; // Why make a copy?
            Type test = thisBoxed.GetType();

            int offset = 0;
            byte[] data = new byte[Marshal.SizeOf(thisBoxed)];

            object fieldValue;
            TypeCode typeCode;

            byte[] temp;

            // Enumerate each structure field using reflection.
            foreach (var field in test.GetFields())
            {
                // field.Name has the field's name.

                fieldValue = field.GetValue(thisBoxed); // Get value

                // Get the TypeCode enumeration. Multiple types get mapped to a common typecode.
                typeCode = Type.GetTypeCode(fieldValue.GetType());

                switch (typeCode)
                {
                    case TypeCode.Single: // float
                        {
                            temp = BitConverter.GetBytes((Single)fieldValue);
                            Array.Reverse(temp);
                            Array.Copy(temp, 0, data, offset, sizeof(Single));
                            break;
                        }
                    case TypeCode.Int32:
                        {
                            temp = BitConverter.GetBytes((Int32)fieldValue);
                            Array.Reverse(temp);
                            Array.Copy(temp, 0, data, offset, sizeof(Int32));
                            break;
                        }
                    case TypeCode.UInt32:
                        {
                            temp = BitConverter.GetBytes((UInt32)fieldValue);
                            Array.Reverse(temp);
                            Array.Copy(temp, 0, data, offset, sizeof(UInt32));
                            break;
                        }
                    case TypeCode.Int16:
                        {
                            temp = BitConverter.GetBytes((Int16)fieldValue);
                            Array.Reverse(temp);
                            Array.Copy(temp, 0, data, offset, sizeof(Int16));
                            break;
                        }
                    case TypeCode.UInt16:
                        {
                            temp = BitConverter.GetBytes((UInt16)fieldValue);
                            Array.Reverse(temp);
                            Array.Copy(temp, 0, data, offset, sizeof(UInt16));
                            break;
                        }
                    case TypeCode.Int64:
                        {
                            temp = BitConverter.GetBytes((Int64)fieldValue);
                            Array.Reverse(temp);
                            Array.Copy(temp, 0, data, offset, sizeof(Int64));
                            break;
                        }
                    case TypeCode.UInt64:
                        {
                            temp = BitConverter.GetBytes((UInt64)fieldValue);
                            Array.Reverse(temp);
                            Array.Copy(temp, 0, data, offset, sizeof(UInt64));
                            break;
                        }
                    case TypeCode.Double:
                        {
                            temp = BitConverter.GetBytes((Double)fieldValue);
                            Array.Reverse(temp);
                            Array.Copy(temp, 0, data, offset, sizeof(Double));
                            break;
                        }
                    case TypeCode.Byte:
                        {
                            data[offset] = (Byte)fieldValue;
                            break;
                        }
                    default:
                        {
                            //System.Diagnostics.Debug.Fail("No conversion provided for this type : " + typeCode.ToString());
                            break;
                        }
                }; // switch
                if (typeCode == TypeCode.Object)
                {
                    int length = ((byte[])fieldValue).Length;
                    Array.Copy(((byte[])fieldValue), 0, data, offset, length);
                    offset += length;
                }
                else
                {
                    offset += Marshal.SizeOf(fieldValue);
                }
            } // foreach

            return data;
        } // Swap

        const int X25_INIT_CRC = 0xffff;
        const int X25_VALIDATE_CRC = 0xf0b8;

        public static ushort crc_accumulate(byte b, ushort crc)
        {
            unchecked
            {
                byte ch = (byte)(b ^ (byte)(crc & 0x00ff));
                ch = (byte)(ch ^ (ch << 4));
                return (ushort)((crc >> 8) ^ (ch << 8) ^ (ch << 3) ^ (ch >> 4));
            }
        }

        public static ushort crc_calculate(byte[] pBuffer, int length)
        {
            if (length < 1)
            {
                return 0xffff;
            }
            // For a "message" of length bytes contained in the unsigned char array
            // pointed to by pBuffer, calculate the CRC
            // crcCalculate(unsigned char* pBuffer, int length, unsigned short* checkConst) < not needed

            ushort crcTmp;
            int i;

            crcTmp = X25_INIT_CRC;

            for (i = 1; i < length; i++) // skips header U
            {
                crcTmp = crc_accumulate(pBuffer[i], crcTmp);
                //Console.WriteLine(crcTmp + " " + pBuffer[i] + " " + length);
            }

            return (crcTmp);
        }

    }


}
