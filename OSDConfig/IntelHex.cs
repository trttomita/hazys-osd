using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace OSDConfig
{
    class IntelHex
    {
        public byte[] RawData { get; set; }

        public void Load(string file)
        {
            byte[] FLASH = new byte[1024 * 1024];

            int optionoffset = 0;
            int total = 0;
            bool hitend = false;
            using (StreamReader sr = new StreamReader(file))
            {
                while (!sr.EndOfStream)
                {
                    //toolStripProgressBar1.Value = (int)(((float)sr.BaseStream.Position / (float)sr.BaseStream.Length) * 100);

                    string line = sr.ReadLine();

                    if (line.StartsWith(":"))
                    {
                        int length = Convert.ToInt32(line.Substring(1, 2), 16);
                        int address = Convert.ToInt32(line.Substring(3, 4), 16);
                        int option = Convert.ToInt32(line.Substring(7, 2), 16);
                        //Console.WriteLine("len {0} add {1} opt {2}", length, address, option);

                        if (option == 0)
                        {
                            string data = line.Substring(9, length * 2);
                            for (int i = 0; i < length; i++)
                            {
                                byte byte1 = Convert.ToByte(data.Substring(i * 2, 2), 16);
                                FLASH[optionoffset + address] = byte1;
                                address++;
                                if ((optionoffset + address) > total)
                                    total = optionoffset + address;
                            }
                        }
                        else if (option == 2)
                        {
                            optionoffset = (int)Convert.ToUInt16(line.Substring(9, 4), 16) << 4;
                        }
                        else if (option == 1)
                        {
                            hitend = true;
                        }
                        int checksum = Convert.ToInt32(line.Substring(line.Length - 2, 2), 16);

                        byte checksumact = 0;
                        for (int z = 0; z < ((line.Length - 1 - 2) / 2); z++) // minus 1 for : then mins 2 for checksum itself
                        {
                            checksumact += Convert.ToByte(line.Substring(z * 2 + 1, 2), 16);
                        }
                        checksumact = (byte)(0x100 - checksumact);

                        if (checksumact != checksum)
                        {
                            //MessageBox.Show("The hex file loaded is invalid, please try again.");
                            throw new Exception("Checksum Failed - Invalid Hex");
                        }
                    }
                    //Regex regex = new Regex(@"^:(..)(....)(..)(.*)(..)$"); // length - address - option - data - checksum
                }

                if (!hitend)
                {
                    // MessageBox.Show("The hex file did no contain an end flag. aborting");
                    throw new Exception("No end flag in file");
                }

                Array.Resize<byte>(ref FLASH, total);

                //return FLASH;
                RawData = FLASH;
            }
        }
    }
}
