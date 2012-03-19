using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO.Ports;
using System.Threading;
using System.Windows.Forms;

namespace OSDConfig
{
    class MegaLoad : SerialPort
    {
        public delegate void ProgressEventHandler(int progress);

        public event ProgressEventHandler Progress;
        public event EventHandler Connected;

        int progress = 0;
        int totalPage = 0;

        int flashSize;
        int pageSize;
        int bootSize;
        int eepromSize;

        public int GetFlashSize(int s)
        {
            if (s <= 'i')
                return 1024 << (s - 'g');
            else
                return 1024 << (3 + s - 'l');
        }

        public int GetPageSize(int s)
        {
            return 32 << (s - 'Q');
        }

        public int GetBootSize(int s)
        {
            return 128 << (s - 'a');
        }

        public int GetEEpromSize(int s)
        {
            return 64 << (s - '.');
        }


        public bool WriteFlash(byte[] flash)
        {
            byte[] buffer = new byte[1024];
            int error = 0;

            for (int i = 0; i < (flash.Length + pageSize - 1) / pageSize; )
            {
                Array.Clear(buffer, 0, pageSize);
                buffer[0] = (byte)((i >> 8) & 0xFF);
                buffer[1] = (byte)(i & 0xff);
                Write(buffer, 0, 2);

                Array.Copy(flash, i * pageSize, buffer, 0, Math.Min(pageSize, flash.Length - i * pageSize));
                for (int j = 0; j < pageSize / 32; j++)
                {
                    Write(buffer, j * 32, 32);
                    Thread.Sleep(5);
                }
                //Write(buffer, 0, pageSize);

                byte checksum = 0;
                //buffer[0] = 0;
                for (int j = 0; j < pageSize; j++)
                    checksum += buffer[j];

                buffer[0] = checksum;

                Write(buffer, 0, 1);
                int ack = ReadByte();
                if (ack == '!')
                {
                    Console.WriteLine("flash page {0} done", i);
                    i++;
                    error = 0;
                    progress = i * 100 / totalPage;
                    Console.WriteLine("progress {0}", progress);
                    if (Progress != null)
                        Progress(progress);
                }
                else
                {
                    Console.WriteLine("{0} flash error, retry page {1}", (char)ack, i);
                    if (++error >= 5)
                    {
                        Console.WriteLine("fail too many times");
                        //return false;
                        break;
                    }
                }
                Application.DoEvents();
            }

            buffer[0] = buffer[1] = 0xff;
            Write(buffer, 0, 2);
            return error == 0;
        }

        public bool WriteEEprom(byte[] eeprom)
        {
            byte[] buffer = new byte[10];
            for (int i = 0; i < eeprom.Length; )
            {
                buffer[0] = (byte)((i >> 8) & 0xff);
                buffer[1] = (byte)(i & 0xff);
                Write(buffer, 0, 2);
                Write(eeprom, i, 1);
                //checksum += eeprom[i];
                Write(eeprom, i, 1);
                if (ReadByte() == '!')
                {
                    i++;
                }
                else
                    Console.WriteLine("eeprom error, retry");
            }

            buffer[0] = buffer[1] = 0xff;
            Write(buffer, 0, 2);

            return true;
        }


        public bool Upload(byte[] flash, byte[] eeprom)
        {

            while (BytesToRead <= 0 || ReadByte() != '>')
            {
                Thread.Sleep(100);
                Console.WriteLine("Waiting...");
                Application.DoEvents();
            }

            byte[] buffer = new byte[1024];

            int error = 0;
            try
            //if (ReadByte() == '>')
            {
                buffer[0] = (byte)'<';
                Write(buffer, 0, 1);
                pageSize = GetPageSize(ReadByte());
                int deviceID = ReadByte();
                flashSize = GetFlashSize(ReadByte());
                bootSize = GetBootSize(ReadByte());
                eepromSize = GetEEpromSize(ReadByte());

                Console.WriteLine("Connected");
                if (Connected != null)
                    Connected(this, new EventArgs());
                //Console.WriteLine("page:{0} id:{1} flash:{2} boot:{3} eeprom:{4}", pageSize, (char)deviceID, flashSize, bootSize, eepromSize);

                //Console.WriteLine("hex:{0} pages", (flash.Length + pageSize - 1) / pageSize);

                totalPage = (flash.Length + pageSize - 1) / pageSize + (eeprom != null ? (eeprom.Length + pageSize - 1) / pageSize : 0);
                progress = 0;

                Write(buffer, 0, 1);    // <

                ReadByte(); // !
                //byte checksum = 0;
                if (flash != null)
                    WriteFlash(flash);

                ReadByte(); // )
                ReadByte(); // !

                if (eeprom != null)
                    WriteEEprom(eeprom);

                return true;
            }
            catch (TimeoutException te)
            {
                return false;
            }
        }
    }
}
