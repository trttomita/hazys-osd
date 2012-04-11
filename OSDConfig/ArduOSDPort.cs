using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO.Ports;
using System.Windows.Forms;

namespace OSDConfig
{
    public class ArduOSDPort : SerialPort
    {
        public void EnterCLI()
        {
            Open();

            System.Threading.Thread.Sleep(200);

            ReadExisting();
            for (int i = 0; i < 4; i++)
                Write("\r\n");
        }

        public bool UploadFont(string fontFile)
        {
            bool fail = false;

            byte[][] fonts = mcm.readNVM(fontFile);

            try
            {
                EnterCLI();

                Write("F");

                int timeout = 0;

                while (BytesToRead == 0)
                {
                    System.Threading.Thread.Sleep(500);
                    Console.WriteLine("Waiting...");
                    timeout++;

                    if (timeout > 6)
                    {
                        MessageBox.Show("Error entering font mode - No Data");
                        Close();
                        return false;
                    }
                }

                int ack = ReadByte();
                if (ack != 'F')
                {
                    MessageBox.Show("Error entering CharSet upload mode - invalid data");
                    Close();
                    return false;
                }
                else
                {
                    Console.WriteLine("connected");
                }

                byte[] ck = new byte[1];
                // toolStripProgressBar1.Value = 0;

                int i = 0;
                for (; i < fonts.Length; i++)
                {
                    //Console.Write("font {0} ", i);


                    byte checksum = 0;
                    Write(fonts[i], 0, 54);

                    for (int j = 0; j < 54; j++)
                    {
                        checksum += fonts[i][j];
                    }
                    ck[0] = checksum;
                    Write(ck, 0, 1);

                    ack = ReadByte();
                    if (ack != '!')
                    {
                        Console.WriteLine("write error");
                        break;
                    }
                    else
                    {
                        //toolStripProgressBar1.Value = i + 1;
                    }
                }
                fail = i != 256;
            }
            catch (TimeoutException te)
            {
                Console.WriteLine("read timeout");
            }


            Close();

            return !fail;
            //if (fail)
            //    toolStripStatusLabel1.Text = "Update CharSet Failed";
            //else
            //    toolStripStatusLabel1.Text = "CharSet Done";
        }

        public bool GetSetting(out OSDSetting setting)
        {
            bool ok = false;
            setting = null;
            try
            {
                EnterCLI();
                Write("s");


                if (ReadByte() == 's')
                {
                    int size = ReadByte();
                    byte[] buf = new byte[size + 1];

                    int tl = 0;
                    while ((tl += Read(buf, tl, size + 1 - tl)) < size + 1)
                    {
                        //toolStripProgressBar1.Value = tl * 100 / (size + 1);
                    }

                    byte ck = 0;
                    for (int i = 0; i < size; i++)
                        ck = (byte)(ck + buf[i]);

                    if (ck == buf[size])
                    {
                        setting = new OSDSetting();
                        setting.FromBytes(buf, 0);
                        ok = true;
                    }
                }

                Close();
            }
            catch (Exception)
            {
            }
            return ok;
        }

        public bool UploadSetting(OSDSetting setting)
        {
            byte[] data = setting.ToBytes();
            byte[] buf = new byte[data.Length + 2];
            buf[0] = (byte)data.Length;
            Array.Copy(data, 0, buf, 1, data.Length);

            int ck = 0;
            for (int i = 0; i < data.Length; i++)
                ck += data[i];
            buf[buf.Length - 1] = (byte)ck;

            bool ok = true;
            try
            {
                EnterCLI();
                Write("S");
                if (ReadByte() == 'S')
                {
                    Write(buf, 0, buf.Length);

                    int ack = ReadByte();
                    if (ack != '!')
                        MessageBox.Show("write setting error");
                    else
                        ok = false;
                }
                Close();
            }
            catch (Exception)
            {
            }
            return ok;
        }

        public bool GetAnalog(int channel, out ushort reading)
        {
            bool ok = false;
            reading = 0;
            try
            {
                EnterCLI();
                Write("a");


                if (ReadByte() == 'a')
                {
                    Write(new byte[] { (byte)channel }, 0, 1);
                    reading = (ushort)(ReadByte() | (ReadByte() << 8));
                }

                Close();
            }
            catch (Exception)
            {
            }
            return ok;
        }

        public bool Reboot()
        {
            bool ok = false;
            try
            {
                EnterCLI();
                Write("R");


                if (ReadByte() == 'R')
                    ok = true;

                Close();
            }
            catch (Exception)
            {
            }
            return ok;
        }
    }
}
