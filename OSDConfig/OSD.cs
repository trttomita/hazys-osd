using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing.Drawing2D;

namespace OSDConfig
{
    public enum VideoMode { PAL, NSTC };

    public partial class OSD : UserControl
    {
        public Bitmap[] Chars { set; get; }

        public new Image BackgroundImage { get; set; }


        //public PanelSetting Setting { get; set; }

        /// <summary>
        /// the un-scaled font render image
        /// </summary>
        Bitmap screen;
        /// <summary>
        /// the scaled to size background control
        /// </summary>
        Bitmap image;

        Graphics gr;

        protected UInt32[,] itemInPos = new UInt32[30, 16];

        protected OSDItem curItem;

        OSDItem selected;
        protected bool mod = true;

        static readonly Size CharSize = new Size(12, 18);
        //const int ColNum = 30;
        //const int PALRowNum = 16;
        //const int NSTCRowNum = 13;

        VideoMode mode;
        public VideoMode Mode
        {
            get { return mode; }
            set
            {
                if (mode != value)
                {
                    mode = value;
                    mod = true;
                }
                //Clear();
                //Draw();
            }
        }

        public int Columns { get { return 30; } }
        public int Rows { get { return mode == VideoMode.PAL ? 16 : 13; } }

        public OSDItem SelectedItem
        {
            get { return selected; }
            set
            {
                if (selected != value)
                {
                    selected = value;
                    mod = true;
                    if (SelectedItemChanged != null)
                        SelectedItemChanged(this, new EventArgs());
                }
            }
        }

        public event EventHandler SelectedItemChanged;

        public event EventHandler ItemPositionChanged;

        int row, col, startRow, startCol;

        bool showGrid;
        public bool ShowGrid { get { return showGrid; } set { if (showGrid != value) { showGrid = value; mod = true; } } }

        public OSD()
        {
            InitializeComponent();
            Clear();
            SelectedItem = OSDItem.NULL;
            //gr = Graphics.FromImage(screen);
        }

        internal OSDSetting setting;
        //[Browse
        public OSDSetting Setting
        {
            get { return setting; }
            set
            {
                if (setting != value)
                {
                    setting = value;
                    mod = true;
                }
                //for (int i = 0; i < infoInPos.GetLength(0); i++)
                //    for (int j = 0; j < infoInPos.GetLength(1); j++)
                //        infoInPos[i, j] = OSDItem.NULL;

                //for (int i = 0; i < (int)OSDItem.NULL; i++)
                //    if (setting.IsEnabled((OSDItem)i))
                //        infoInPos[setting.coord[i, 0], setting.coord[i, 1]] = (OSDItem)i;
            }
        }

        //public override onpain
        

        public virtual void Draw()
        {
            //if (startup)
            //    return;

            //for (int b = 0; b < usedPostion.Length; b++)
            //{
            //    usedPostion[b] = new string[16];
            //}




            float scaleW = pictureBox.Width / (float)screen.Width;
            float scaleH = pictureBox.Height / (float)screen.Height;

            //screen = new Bitmap(screen.Width, screen.Height);

            //gr = Graphics.FromImage(screen);

            //image = new Bitmap(image.Width, image.Height);

            Graphics grfull = Graphics.FromImage(image);

            try
            {
                grfull.DrawImage(BackgroundImage, 0, 0, pictureBox.Width, pictureBox.Height);
            }
            catch { }

            if (ShowGrid)
            {
                for (int b = 1; b < 16; b++)
                {
                    for (int a = 1; a < 30; a++)
                    {
                        grfull.DrawLine(new Pen(Color.Gray, 1), a * 12 * scaleW, 0, a * 12 * scaleW, pictureBox.Height);
                        grfull.DrawLine(new Pen(Color.Gray, 1), 0, b * 18 * scaleH, pictureBox.Width, b * 18 * scaleH);
                    }
                }
            }

            grfull.DrawImage(screen, 0, 0, image.Width, image.Height);
            pictureBox.Image = image;
        }

        // used for printf tracking line and row

        public void printf(string format, params object[] args)
        {
            StringBuilder sb = new StringBuilder();

            sb = new StringBuilder(AT.MIN.Tools.sprintf(format, args));

            //sprintf(sb, format, __arglist(args));

            //Console.WriteLine(sb.ToString());
            bool sel = curItem == selected;
            Size s = getSize(sb.ToString());
            for (int i = 0; i < s.Width; i++)
                for (int j = 0; j < s.Height; j++)
                {
                    int x = (i + startCol) % Columns;
                    int y = (j + startRow);
                    if (y < Rows)
                    {
                        if ((itemInPos[x, y] & ~(1U << (int)curItem)) != 0)
                            sel = true;

                    }
                }

            foreach (char ch in sb.ToString().ToCharArray())
                write(ch);
        }

        Size getSize(string str)
        {
            Size s = new Size(0, 1);
            int cols = 0;
            foreach (var ch in str)
            {
                if (ch == '|')
                {
                    s.Width = Math.Max(cols, s.Width);
                    cols = 0;
                    s.Height++;
                }
                else
                    cols++;
            }
            s.Width = Math.Max(cols, s.Width);
            return s;
        }


        public void write(char ch/*, bool sel = false*/)
        {
            if (ch == '|')
            {
                row++;
                col = startCol;
                return;
            }

            try
            {
                // draw red boxs

                col = (col + Columns) % Columns;


                //int w1 = (this.x / 12 + r) % basesize.Width;
                //int h1 = (this.y / 18 + d);


                if (row < Rows)
                //if (w1 < basesize.Width && h1 < basesize.Height)
                {
                    if (selected == curItem)
                    {
                        Console.WriteLine("Sel {0}", selected);
                        gr.DrawRectangle(Pens.Red, (col * 12) % screen.Width, (row * 18), 12, 18);
                    }
                    // check if this box has bene used
                    //if (usedPostion[w1][h1] != null)
                    //{
                    //    //System.Diagnostics.Debug.WriteLine("'" + used[this.x / 12 + r * 12 / 12][this.y / 18 + d * 18 / 18] + "'");
                    //}
                    //else

                    UInt32 otherItem = (itemInPos[col, row] & ~(1U << (int)curItem));
                    if (otherItem != 0 && otherItem < (1U << (int)curItem))
                    {
                        //Bitmap bmp = new Bitmap(12, 18);
                        //Graphics g = Graphics.FromImage(bmp);
                        //g.DrawLine(Pens.Red, 2, 0, 2, 4);
                        //g.DrawLine(Pens.Red, 0, 2, 4, 2);
                        //Brush b = new TextureBrush(bmp/*, WrapMode.Tile*/);
                        Brush b = new SolidBrush(Color.FromArgb(128, Color.Red));
                        gr.FillRectangle(b, (col * 12) % screen.Width, (row * 18), 12, 18);
                    }

                    itemInPos[col, row] |= (1U << (int)curItem);
                    {
                        gr.DrawImage(Chars[ch], (col * 12) % screen.Width, row * 18, 12, 18);
                        col++;
                        //gr.Flush();
                    }

                    //  usedPostion[w1][h1] = processingpanel;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                System.Diagnostics.Debug.WriteLine("printf exception");
            }
        }

        public void printf_P(string format, params object[] args)
        {
            printf(format, args);
        }

        public void print_P(string str)
        {
            printf("%s", str);
        }

        public void Clear()
        {
            screen = new Bitmap(Columns * CharSize.Width, Rows * CharSize.Height);
            //image = new Bitmap(screen.Width, screen.Height);
            image = new Bitmap(pictureBox.Width, pictureBox.Height);
            gr = Graphics.FromImage(screen);
            Array.Clear(itemInPos, 0, itemInPos.Length);
        }


        public void SetPanel(int col, int row)
        {
            startCol = this.col = col;
            if (Mode == VideoMode.NSTC && row >= 6)
                row -= 3;
            startRow = this.row = row;
        }

        public void OpenSingle(int col, int row)
        {
            SetPanel(col, row);
        }

        
        public void OpenPanel()
        {

        }

        public void ClosePanel()
        {
            row = ++startRow;
            col = startCol;
        }


        bool mousedown;
        Point offset;
        private void pictureBox_MouseDown(object sender, MouseEventArgs e)
        {
            //currentlyselected = getMouseOverItem(e.X, e.Y);
            int c = e.X * screen.Width / (pictureBox.Width * CharSize.Width);
            int r = e.Y * screen.Height / (pictureBox.Height * CharSize.Height);
            bool sel = false;
            for (int i = (int)OSDItem.NULL - 1; i >= 0; i--)
                if ((itemInPos[c, r] & (1U << i)) != 0)
                {
                    SelectedItem = (OSDItem)i;
                    sel = true;
                    break;
                }
            if (!sel)
                SelectedItem = OSDItem.NULL;

            mousedown = true;

            if (selected != OSDItem.NULL)
            {
                int sc = c, sr = r;
                while (sc >= 0 && (itemInPos[sc, sr] & (1U << (int)selected)) != 0)
                    sc--;
                sc++;
                while (sr >= 0 && (itemInPos[sc, sr] & (1U << (int)selected)) != 0)
                    sr--;
                sr++;
                offset = new Point(sc - c, sr - r);
                //offset = new Point(sc * CharSize.Width * pictureBox.Width / screen.Width - e.X, sr * CharSize.Height * pictureBox.Height / screen.Height - e.Y);
                //Console.WriteLine("org [{0},{1}]", sc, sr);

            }
            Draw();

            //if (SelectedItemChanged != null)
            //    SelectedItemChanged(this, new EventArgs());
        }

        private void pictureBox_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Left && mousedown == true && selected != OSDItem.NULL)
            {
                int c = (e.X * screen.Width / (pictureBox.Width * CharSize.Width) + offset.X + Columns) % Columns;
                int r = (e.Y * screen.Height / (pictureBox.Height * CharSize.Height) + offset.Y + Rows) % Rows;

                if (Mode == VideoMode.NSTC && r + 3 >= 6)
                    r += 3;

                SetItemPosition(selected, new Point(c, r));
                Draw();
                pictureBox.Focus();
            }
            else
            {
                mousedown = false;
            }
        }

        private void pictureBox_MouseUp(object sender, MouseEventArgs e)
        {
            //getMouseOverItem(e.X, e.Y);

            mousedown = false;
        }

        public Point GetItemPosition(OSDItem info)
        {
            return new Point(Setting.coord[(int)info, 0], Setting.coord[(int)info, 1]);
        }

        public void SetItemPosition(OSDItem info, Point pos)
        {
            int x = Setting.coord[(int)info, 0], y = Setting.coord[(int)info, 1];

            if (pos.X != x || pos.Y != y)
            {
                Setting.coord[(int)info, 0] = (byte)pos.X;
                Setting.coord[(int)info, 1] = (byte)pos.Y;

                Array.Clear(itemInPos, 0, itemInPos.Length);
                mod = true;
                if (ItemPositionChanged != null)
                    ItemPositionChanged(this, new EventArgs());
            }


            //if (Setting.IsEnabled(info))
            //    Draw();
        }

        public void SetItemEnabled(OSDItem item, bool enabled)
        {
            if (item != OSDItem.NULL)
            {
                if (enabled)
                    setting.enable |= (1U << (int)item);
                else
                    setting.enable &= ~(1U << (int)item);
                mod = true;
            }
        }

        public bool GetOption(OSDOption opt)
        {
            return setting.GetOption(opt);
        }

        public void SetOption(OSDOption opt, bool on)
        {
            mod = true;
            setting.SetOption(opt, on);
        }
    }
}
