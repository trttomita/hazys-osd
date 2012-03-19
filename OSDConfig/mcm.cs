using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Drawing;

namespace OSDConfig
{
    class mcm
    {
        public static Bitmap[] readMCM2(string file)
        {
            Bitmap[] imagearray = new Bitmap[256];
            for (int a = 0; a < imagearray.Length; a++)
            {
                imagearray[a] = new Bitmap(12, 18);
            }

            StreamReader sr = new StreamReader(file);
            string device = sr.ReadLine();

            string line = null;

            for (int i = 0; i < imagearray.Length; i++)
            {
                int x = 0, y = 0;

                for (int j = 0; j < 54; j++)
                {
                    line = sr.ReadLine();
                    Color c = Color.White;
                    for (int k = 0; k < 4; k++)
                    {
                        switch (line.Substring(k * 2, 2))
                        {
                            case "00":
                                c = Color.Black;
                                break;
                            case "10":
                                c = Color.White;
                                break;
                            case "01":
                            case "11":
                                c = Color.Transparent;
                                break;
                        }
                        imagearray[i].SetPixel(x++, y, c);
                    }
                    if (x == 12)
                    {
                        x = 0;
                        y++;
                    }
                }
                for (int j = 0; j < 64 - 54; j++)
                    sr.ReadLine();
            }

            return imagearray;
        }

        public static byte[][] readNVM(string file)
        {
            byte[][] imagearray = new byte[256][];
            for (int a = 0; a < imagearray.Length; a++)
            {
                imagearray[a] = new byte[64];
            }

            StreamReader sr = new StreamReader(file);
            string device = sr.ReadLine();

            string line = null;

            for (int i = 0; i < imagearray.Length; i++)
            {
                for (int j = 0; j < 54; j++)
                {
                    line = sr.ReadLine();
                    byte b = 0;
                    for (int k = 0; k < line.Length; k++)
                        b |= (byte)((line[k] == '1' ? 1 : 0) << (7 - k));
                    /*for (int k = 0; k < 8; k += 2)
                    {
                        switch (line.Substring(k, 2))
                        {
                            case "10":
                                b = (byte)((b << 2) | 2);
                                break;
                            case "11":
                            case "01":
                                b = (byte)((b << 2) | 1);
                                break;
                        }
                    }*/
                    imagearray[i][j] = b;
                }
                for (int j = 0; j < 64 - 54; j++)
                    sr.ReadLine();
            }

            return imagearray;
        }

        public static Bitmap[] readMCM(string file)
        {
            Bitmap[] imagearray = new Bitmap[256];

            if (!File.Exists(file))
            {
                System.Windows.Forms.MessageBox.Show("Font file does not exist : " + file);
                return imagearray;
            }

            for (int a = 0; a < imagearray.Length; a++)
            {
                imagearray[a] = new Bitmap(12, 18);
            }

            StreamReader sr = new StreamReader(file);

            string device = sr.ReadLine();

            // 00 black   10 white   x1 = trans/grey

            int x = 0, y = 0;

            int image = 0;

            while (!sr.EndOfStream)
            {
                string line = "";
                y = 0;
                while (y < 18)
                {
                    x = 0;
                    while (x < 12)
                    {
                        if (x == 0 || x == 4 || x == 8)
                        {
                            //Console.WriteLine("line");
                            line = sr.ReadLine();
                            if (line == null)
                                return imagearray;
                        }

                        string i1 = line.Substring((x % 4) * 2, 2);

                        //Console.WriteLine(image + " " + line + " " + i1 + " " + x + " " + y);

                        if (i1 == "01" || i1 == "11")
                        {
                            imagearray[image].SetPixel(x, y, Color.Transparent);
                        }
                        else if (i1 == "00")
                        {
                            imagearray[image].SetPixel(x, y, Color.Black);
                        }
                        else if (i1 == "10")
                        {
                            imagearray[image].SetPixel(x, y, Color.White);
                        }

                        x++;
                    }
                    y++;
                }

                // left
                int left = 256 - 216;
                while ((left / 4) > 0)
                {
                    sr.ReadLine(); // 1
                    left -= 4;
                }

                image++;
            }

            return imagearray;
        }

        public static byte[] ToNVM(Bitmap character)
        {
            if (character.Width != 12 || character.Height != 18)
            {
                Console.WriteLine("invalid character");
                return null;
            }

            byte[] bytes = new byte[54];
            Array.Clear(bytes, 0, bytes.Length);

            for (int x = 0; x < 12; x++)
                for (int y = 0; y < 18; y++)
                {
                    int c = character.GetPixel(x, y).ToArgb();
                    byte b = 0;
                    if (c == Color.White.ToArgb())
                        b = 2;
                    else if (c == Color.Transparent.ToArgb())
                        b = 1;

                    bytes[3 * y + x / 4] |= (byte)(b << (6 - (x % 4)));
                }
            return bytes;
        }
    }
}
