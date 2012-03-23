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
        char memType;

        int retry = 0;
        int pagePtr = 0;

        int bytePtr = 0;

        byte[] flash;
        byte[] eeprom;

        public void SendFlashPage()
        {
            WriteByte((byte)(pagePtr >> 8 & 255));
            WriteByte((byte)(pagePtr & 255));
            byte b = 0;
            Write(flash, pagePtr * pageSize, pageSize);
            for (int i = 0; i < pageSize; i++)
            {
                b += flash[pagePtr * pageSize + i];
            }
            WriteByte(b);
        }

        public void SendEEpromByte()
        {
            WriteByte((byte)(bytePtr >> 8 & 255));
            WriteByte((byte)(bytePtr & 255));
            WriteByte(eeprom[bytePtr]);
            byte b = (byte)(bytePtr >> 8 & 255);
            b += (byte)(bytePtr & 255);
            b += eeprom[bytePtr];
            WriteByte(b);
        }

        public void WriteByte(byte b)
        {
            Write(new byte[] { b }, 0, 1);
        }

        public void WriteChar(char c)
        {
            WriteByte((byte)c);
        }


        public bool Upload(byte[] flash, byte[] eeprom)
        {
            if (flash != null)
            {
                this.flash = new byte[flash.Length];
                Array.Copy(flash, this.flash, flash.Length);
            }
            else
                this.flash = null;

            if (eeprom != null)
            {
                this.eeprom = new byte[eeprom.Length];
                Array.Copy(eeprom, this.eeprom, eeprom.Length);
            }
            else
                this.eeprom = null;

            bool connected = false;
            bool wait = true;
            int waitCount = 0;
            bool ok = false;

            while (wait)
            {
                while (BytesToRead <= 0)
                {
                    if (connected && waitCount > 20)
                    {
                        Console.WriteLine("Read Timeout");
                        return false;
                    }
                    else if (!connected && waitCount > 100)
                    {
                        Console.WriteLine("Connect Timeout");
                        return false;
                    }

                    waitCount++;
                    Thread.Sleep(100);
                    Application.DoEvents();
                }

                waitCount = 0;

                char c = (char)ReadByte();
                //Console.Write(c);

                switch (c)
                {
                    case 'U':
                        WriteByte(85);
                        break;
                    case '>':
                        connected = true;
                        WriteChar('<');
                        if (Connected != null)
                            Connected(this, new EventArgs());

                        if (flash != null)
                        {
                            memType = 'F';
                            retry = 0;
                            pagePtr = 0;
                        }
                        else
                        {
                            memType = 'E';
                            WriteByte(255);
                            WriteByte(255);
                            ReadChar();
                        }
                        break;
                }

                if (connected)
                {
                    switch (c)
                    {
                        case '!':
                            if (memType == 'F')
                            {
                                if (pagePtr * pageSize >= flash.Length)
                                {
                                    WriteByte(255);
                                    WriteByte(255);
                                    pagePtr = 0;
                                }
                                else
                                {
                                    SendFlashPage();
                                    pagePtr++;
                                    if (Progress != null)
                                    {
                                        Progress(pagePtr * 100 / (this.flash.Length / pageSize));
                                    }
                                }
                            }
                            else if (memType == 'E')
                            {
                                if (eeprom == null || bytePtr >= eeprom.Length)
                                {
                                    WriteByte(255);
                                    WriteByte(255);
                                    bytePtr = 0;
                                    ok = true;
                                    wait = false;
                                }
                                else
                                {
                                    SendEEpromByte();
                                    bytePtr++;
                                }
                            }
                            break;
                        case '@':
                            if (retry > 3)
                            {
                                wait = false;
                                break;
                            }
                            else if (memType == 'F')
                            {
                                pagePtr--;
                                retry++;
                                SendFlashPage();
                                pagePtr++;
                            }
                            else if (memType == 'E')
                            {
                                bytePtr--;
                                retry++;
                                SendEEpromByte();
                                bytePtr++;
                            }
                            break;
                        case ')':
                            memType = 'E';
                            retry = 0;
                            break;
                        case '%':
                            //lockbit;
                            break;
                    }
                    if (c >= 'A' && c <= 'P' || c >= '\u0080' && c <= '\u0089')
                    {
                        //device id
                    }
                    else if (c >= 'Q' && c <= 'T' || c == 'V')
                    {
                        pageSize = c == 'V' ? 512 : (32 << (c - 'Q'));
                        if (this.flash != null && this.flash.Length % pageSize != 0)
                            Array.Resize(ref this.flash, ((this.flash.Length + pageSize - 1) / pageSize) * pageSize);
                    }
                    else if (c >= 'a' && c <= 'f')
                        bootSize = 128 << (c - 'a');
                    else if (c >= 'g' && c <= 'i')
                        flashSize = 1024 << (c - 'g');
                    else if (c >= 'l' && c <= 'r')
                        flashSize = 4096 << (c - 'l');
                    else if (c >= '.' && c <= '1')
                        eepromSize = 512;
                    else if (c >= '2' && c <= '4')
                        eepromSize = 1024 << (c - '2');
                }
            }

            return ok;
        }
    }
}
