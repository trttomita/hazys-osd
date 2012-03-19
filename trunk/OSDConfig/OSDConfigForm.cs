using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.IO.Ports;
using System.IO;
using ArdupilotMega;
using System.Xml;
using System.Threading;


namespace OSDConfig
{
    public partial class OSDConfigForm : Form
    {
        /// <summary>
        /// 328 eeprom memory
        /// </summary>
        byte[] eeprom = new byte[1024];
        /// <summary>
        /// background image
        /// </summary>
        bool incli = false;

        SerialPort comPort = new SerialPort();

        int bootRate = 9600;
        int osdRate = 57600;



        public OSDConfigForm()
        {
            InitializeComponent();

            // load default font
            osd.Chars = mcm.readMCM2("OSD_SA_v5.mcm");
            // load default bg picture

            //var chars = mcm.readMCM2("OSD_SA_v5.mcm");
            //for (int i = 0; i < osd.Chars.Length; i++)
            //    osd.Chars[i].Save(i.ToString() + ".png");


            try
            {
                osd.BackgroundImage/*bgpicture*/ = Image.FromFile("vlcsnap-2012-01-28-07h46m04s95.png");
            }
            catch { }

            for (int i = 0; i < OSDItemName.Name.Length; i++)
                if (OSDItemName.Name[i] != null)
                    LIST_items.Items.Add(OSDItemName.Name[i], osd.Setting.IsEnabled((OSDItem)i));

            osd.SelectedItemChanged += new EventHandler(osd_SelectedItemChanged);
            osd.ItemPositionChanged += new EventHandler(osd_ItemPositionChanged);
        }

        void osd_ItemPositionChanged(object sender, EventArgs e)
        {
            //throw new NotImplementedException();
            if (osd.SelectedItem != OSDItem.NULL)
            {
                Point p = osd.GetItemPosition(osd.SelectedItem);
                NUM_X.Value = p.X;
                NUM_Y.Value = p.Y;
            }
        }

        void osd_SelectedItemChanged(object sender, EventArgs e)
        {
            if (osd.SelectedItem == OSDItem.NULL)
                LIST_items.SelectedItem = null;
            else
                LIST_items.SelectedItem = OSDItemName.Name[(int)osd.SelectedItem];
            //throw new NotImplementedException();
        }


        private string[] GetPortNames()
        {
            string[] devs = new string[0];

            if (Directory.Exists("/dev/"))
                devs = Directory.GetFiles("/dev/", "*ACM*");

            string[] ports = SerialPort.GetPortNames();

            string[] all = new string[devs.Length + ports.Length];

            devs.CopyTo(all, 0);
            ports.CopyTo(all, devs.Length);

            return all;
        }


        private void OSD_Load(object sender, EventArgs e)
        {

            string strVersion = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();
            this.Text = this.Text + " " + strVersion;

            CMB_ComPort.Items.AddRange(GetPortNames());

            if (CMB_ComPort.Items.Count > 0)
                CMB_ComPort.SelectedIndex = 0;

            xmlconfig(false);

            //osdDraw();
            osd.Draw();
        }

        private void checkedListBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

            if (((CheckedListBox)sender).SelectedItem == null)
            {
                NUM_X.Value = NUM_Y.Value = 0;
            }
            else
            {
                string item = ((CheckedListBox)sender).SelectedItem.ToString();
                OSDItem sel = (OSDItem)Array.IndexOf(OSDItemName.Name, item);
                //currentlyselected = item;
                osd.SelectedItem = sel;

                Point p = osd.GetItemPosition(sel);
                NUM_X.Value = p.X;
                NUM_Y.Value = p.Y;
                //osdDraw();
                osd.Draw();
            }
        }

        private void checkedListBox1_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            // if (((CheckedListBox)sender).SelectedItem != null && ((CheckedListBox)sender).SelectedItem.ToString() == "Horizon")
            if (((CheckedListBox)sender).SelectedItem != null)
            {
                /*    if (((CheckedListBox)sender).SelectedItem.ToString() == "Horizon" && e.NewValue == CheckState.Checked)
                    {
                        int index = LIST_items.Items.IndexOf("Center");
                        LIST_items.SetItemChecked(index, false);
                    }
                    else if (((CheckedListBox)sender).SelectedItem.ToString() == "Center" && e.NewValue == CheckState.Checked)
                    {
                        int index = LIST_items.Items.IndexOf("Horizon");
                        LIST_items.SetItemChecked(index, false);
                    }*/
                OSDItem item = (OSDItem)Array.IndexOf(OSDItemName.Name, ((CheckedListBox)sender).SelectedItem);
                if (item == OSDItem.Hor && e.NewValue == CheckState.Checked)
                {
                    int index = LIST_items.Items.IndexOf(OSDItemName.Name[(int)OSDItem.Cen]);
                    LIST_items.SetItemChecked(index, false);
                    osd.SetItemEnabled(OSDItem.Cen, false);
                }
                else if (item == OSDItem.Cen && e.NewValue == CheckState.Checked)
                {
                    int index = LIST_items.Items.IndexOf(OSDItemName.Name[(int)OSDItem.Hor]);
                    LIST_items.SetItemChecked(index, false);
                    osd.SetItemEnabled(OSDItem.Hor, false);
                }
                osd.SetItemEnabled(item, e.NewValue == CheckState.Checked);
            }

            // add a delay to this so it runs after the control value has been defined.
            if (this.IsHandleCreated)
                this.BeginInvoke((MethodInvoker)delegate { /*osdDraw();*/osd.Draw(); });
        }


        private void BUT_WriteOSD_Click(object sender, EventArgs e)
        {
            toolStripProgressBar1.Style = ProgressBarStyle.Continuous;
            this.toolStripStatusLabel1.Text = "";

            int size = 4 + osd.Setting.coord.Length;
            byte[] buf = new byte[1 + size + 1];
            buf[0] = (byte)size;
            Array.Copy(BitConverter.GetBytes(osd.Setting.enable), 0, buf, 1, 4);
            Buffer.BlockCopy(osd.Setting.coord, 0, buf, 5, osd.Setting.coord.Length);
            int ck = 0;
            for (int i = 1; i < size + 1; i++)
                ck += buf[i];
            buf[size + 1] = (byte)ck;

            try
            {
                EnterCLI();
                comPort.Write("S");
                if (comPort.ReadByte() == 'S')
                {
                    comPort.Write(buf, 0, buf.Length);

                    int ack = comPort.ReadByte();
                    if (ack != '!')
                        MessageBox.Show("write setting error");
                }
            }
            catch { MessageBox.Show("Error opening com port", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }
        }


        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void comboBox1_Click(object sender, EventArgs e)
        {
            CMB_ComPort.Items.Clear();
            CMB_ComPort.Items.AddRange(GetPortNames());
        }



        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            osd.ShowGrid = checkBox1.Checked;
            osd.Draw();
        }



        private void BUT_ReadOSD_Click(object sender, EventArgs e)
        {
            toolStripProgressBar1.Style = ProgressBarStyle.Continuous;
            this.toolStripStatusLabel1.Text = "";

            bool fail = false;
            //ArduinoSTK sp;

            try
            {
                EnterCLI();
                comPort.Write("s");

                if (comPort.ReadByte() == 's')
                {
                    int size = comPort.ReadByte();
                    byte[] buf = new byte[size + 1];

                    int tl = 0;
                    while ((tl += comPort.Read(buf, tl, size + 1 - tl)) < size + 1)
                        ;

                    byte ck = 0;
                    for (int i = 0; i < size; i++)
                        ck = (byte)(ck + buf[i]);

                    if (ck == buf[size])
                    {
                        OSDSetting setting = new OSDSetting();
                        setting.enable = BitConverter.ToUInt32(buf, 0);
                        Buffer.BlockCopy(buf, 4, setting.coord, 0, size - 4);

                        osd.Setting = setting;

                        //reload 
                        LIST_items.Items.Clear();
                        for (int i = 0; i < OSDItemName.Name.Length; i++)
                            if (OSDItemName.Name[i] != null)
                                LIST_items.Items.Add(OSDItemName.Name[i], osd.Setting.IsEnabled((OSDItem)i));

                        osd.Draw();
                    }
                }
            }
            catch { MessageBox.Show("Error opening com port", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

        }




        void sp_Progress(int progress)
        {
            toolStripStatusLabel1.Text = "Uploading " + progress + " %";
            toolStripProgressBar1.Value = progress;

            statusStrip1.Refresh();
        }

        private void CHK_pal_CheckedChanged(object sender, EventArgs e)
        {
            osd.Mode = CHK_pal.Checked ? VideoMode.PAL : VideoMode.NSTC;
            osd.Draw();
        }

        private void pALToolStripMenuItem_CheckStateChanged(object sender, EventArgs e)
        {
            nTSCToolStripMenuItem.Checked = !CHK_pal.Checked;
        }

        private void nTSCToolStripMenuItem_CheckStateChanged(object sender, EventArgs e)
        {
            CHK_pal.Checked = !nTSCToolStripMenuItem.Checked;
        }

        private void saveToFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog() { Filter = "*.osd|*.osd" };

            sfd.ShowDialog();

            if (sfd.FileName != "")
            {
                try
                {
                    using (Stream s = sfd.OpenFile())
                    {
                        byte[] buf = BitConverter.GetBytes(osd.Setting.enable);
                        s.Write(buf, 0, buf.Length);
                        buf = new byte[osd.Setting.coord.Length];
                        Buffer.BlockCopy(osd.Setting.coord, 0, buf, 0, buf.Length);
                        //Array.Copy(osd.Setting.coord, buf, buf.Length);
                        s.Write(buf, 0, osd.Setting.coord.Length);
                    }
                }
                catch
                {
                    MessageBox.Show("Error writing file");
                }
            }
        }

        private void loadFromFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog() { Filter = "*.osd|*.osd" };

            ofd.ShowDialog();

            if (ofd.FileName != "")
            {
                try
                {
                    using (Stream f = ofd.OpenFile())
                    {
                        OSDSetting setting = new OSDSetting();
                        byte[] buf = new byte[sizeof(UInt32) + setting.coord.Length];
                        f.Read(buf, 0, buf.Length);
                        setting.enable = BitConverter.ToUInt32(buf, 0);
                        Buffer.BlockCopy(buf, sizeof(UInt32), setting.coord, 0, setting.coord.Length);
                        osd.Setting = setting;
                    }
                }
                catch
                {
                    MessageBox.Show("Error Reading file");
                }
            }

            osd.Draw();
        }

        private void loadDefaultsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            osd.Setting = new OSDSetting();
            osd.Draw();
            //setupFunctions();
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void updateFirmwareToolStripMenuItem_Click(object sender, EventArgs e)
        {
            toolStripProgressBar1.Style = ProgressBarStyle.Continuous;
            this.toolStripStatusLabel1.Text = "";

            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "*.hex|*.hex";

            ofd.ShowDialog();

            if (ofd.FileName != "")
            {
                byte[] FLASH;
                try
                {
                    toolStripStatusLabel1.Text = "Reading Hex File";

                    statusStrip1.Refresh();

                    IntelHex hex = new IntelHex();
                    hex.Load(ofd.FileName);
                    FLASH = hex.RawData;
                }
                catch { MessageBox.Show("Bad Hex File"); return; }

                bool fail = false;
                //ArduinoSTK sp;




                MegaLoad sp = new MegaLoad();
                toolStripProgressBar1.Maximum = 100;
                toolStripProgressBar1.Style = ProgressBarStyle.Continuous;
                toolStripProgressBar1.Value = 0;

                sp.Progress += (p) => { toolStripProgressBar1.Value = p; };
                sp.Connected += (s, ce) => { toolStripStatusLabel1.Text = "Programming"; };
                try
                {
                    EnterCLI();
                    comPort.Write(Encoding.ASCII.GetBytes("R"), 0, 1);
                    comPort.ReadExisting();
                    comPort.Close();

                    //sp = new MegaLoad();
                    sp.PortName = CMB_ComPort.Text;
                    //sp.BaudRate = 9600;
                    sp.BaudRate = bootRate;
                    sp.WriteBufferSize = 32;
                    sp.ReadTimeout = 3000;
                    //sp.DtrEnable = true;

                    sp.Open();
                }
                catch { MessageBox.Show("Error opening com port", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

                toolStripStatusLabel1.Text = "Connecting to Board";

                sp.ReadExisting();
                fail = !sp.Upload(FLASH, null);
                sp.Close();

                if (!fail)
                //if (!fail)
                {
                    toolStripStatusLabel1.Text = "Done";
                    MessageBox.Show("Done!");
                }
                else
                {
                    toolStripStatusLabel1.Text = "Failed";
                }


                //    ).Start();
                /*
                if (sp.connectAP())
                {
                    sp.Progress += new ArduinoSTK.ProgressEventHandler(sp_Progress);
                    try
                    {
                        if (!sp.uploadflash(FLASH, 0, FLASH.Length, 0))
                        {
                            if (sp.IsOpen)
                                sp.Close();

                            MessageBox.Show("Upload failed. Lost sync. Try using Arduino to upload instead",
                                "Error",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Warning);
                        }
                    }
                    catch (Exception ex)
                    {
                        fail = true;
                        MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }

                }
                else
                {
                    MessageBox.Show("Failed to talk to bootloader");
                }
                */



            }
        }

        private void customBGPictureToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "jpg or bmp|*.jpg;*.bmp";

            ofd.ShowDialog();

            if (ofd.FileName != "")
            {
                try
                {
                    osd.BackgroundImage = Image.FromFile(ofd.FileName);

                }
                catch { MessageBox.Show("Bad Image"); }

                osd.Draw();
            }
        }

        private void sendTLogToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //byte[] msg = new byte[] { 0x55, 3, 0x36, 1, 1, 0, 2, 3, 2, 0x76, 0xcb };
            //            ArdupilotMega.MAVLink.__mavlink_heartbeat_t hb = new ArdupilotMega.MAVLink.__mavlink_heartbeat_t();
            //          hb.autopilot = (byte)ArdupilotMega.MAVLink.MAV_TYPE.MAV_FIXED_WING;
            /*MAVLink.__mavlink_attitude_t at = new MAVLink.__mavlink_attitude_t();
            at.pitch = (float)(-10 * Math.PI / 180);
            at.roll = (float)(20 * Math.PI / 180);
            MAVLink.__mavlink_gps_status_t gs = new MAVLink.__mavlink_gps_status_t();
            gs.satellite_azimuth = new byte[20];
            gs.satellite_elevation = new byte[20];
            gs.satellite_prn = new byte[20];
            gs.satellite_snr = new byte[20];
            gs.satellite_used = new byte[20];

            object[] msgs = new object[]{
                new MAVLink.__mavlink_heartbeat_t(),
                new MAVLink.__mavlink_sys_status_t(),
                new MAVLink.__mavlink_gps_raw_t(),
                //new MAVLink.__mavlink_gps_raw_int_t(),
                //new MAVLink.__mavlink_gps_status_t(),
                gs,
                //new MAVLink.__mavlink_attitude_t(),
                at,
                new MAVLink.__mavlink_vfr_hud_t()
            };
            byte[] types = new byte[]
            {
                MAVLink.MAVLINK_MSG_ID_HEARTBEAT,
                MAVLink.MAVLINK_MSG_ID_SYS_STATUS,
                MAVLink.MAVLINK_MSG_ID_GPS_RAW,
                //MAVLink.MAVLINK_MSG_ID_GPS_RAW_INT,
                MAVLink.MAVLINK_MSG_ID_GPS_STATUS,
                MAVLink.MAVLINK_MSG_ID_ATTITUDE,
                MAVLink.MAVLINK_MSG_ID_VFR_HUD
            };*/


            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "Tlog|*.tlog";

            ofd.ShowDialog();

            if (ofd.FileName != "")
            {
                if (comPort.IsOpen)
                    comPort.Close();

                try
                {
                    comPort.PortName = CMB_ComPort.Text;
                    comPort.BaudRate = osdRate;
                    comPort.Open();

                }
                catch { MessageBox.Show("Error opening com port", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

                BinaryReader br = new BinaryReader(ofd.OpenFile());

                this.toolStripProgressBar1.Style = ProgressBarStyle.Marquee;
                this.toolStripStatusLabel1.Text = "Sending TLOG data...";

                while (br.BaseStream.Position < br.BaseStream.Length && !this.IsDisposed)
                    while (true)
                    {
                        try
                        {
                            byte[] bytes = br.ReadBytes(20);
                            /*for (int i = 0; i < msgs.Length; i++)
                            {
                                //if (i == 1)
                                //    continue;
                                byte[] bytes = MavLink.generatePacket(types[i], msgs[i]);
                                comPort.Write(bytes, 0, bytes.Length);
                                // comPort.Write(msg, 0, msg.Length);

                                //System.Threading.Thread.Sleep(1000);
                                System.Threading.Thread.Sleep(100);
                                string ack = comPort.ReadExisting();
                                //Console.Write("{0}:  ", MavLink.packetcount);
                                if (!string.IsNullOrEmpty(ack))
                                    Console.WriteLine(ack);
                            }*/
                            Thread.Sleep(10);
                        }
                        catch (Exception ce)
                        {
                            Console.WriteLine(ce.StackTrace);
                            /*break;*/
                        }

                        Application.DoEvents();
                    }

                try
                {
                    toolStripProgressBar1.Style = ProgressBarStyle.Continuous;
                    toolStripStatusLabel1.Text = "";

                    comPort.Close();
                }
                catch { }
            }
        }

        private void OSD_FormClosed(object sender, FormClosedEventArgs e)
        {
            xmlconfig(true);

        }

        private void xmlconfig(bool write)
        {
            if (write || !File.Exists(Path.GetDirectoryName(Application.ExecutablePath) + Path.DirectorySeparatorChar + @"config.xml"))
            {
                try
                {
                    XmlTextWriter xmlwriter = new XmlTextWriter(Path.GetDirectoryName(Application.ExecutablePath) + Path.DirectorySeparatorChar + @"config.xml", Encoding.ASCII);
                    xmlwriter.Formatting = Formatting.Indented;

                    xmlwriter.WriteStartDocument();

                    xmlwriter.WriteStartElement("Config");

                    xmlwriter.WriteElementString("ComPort", CMB_ComPort.Text);
                    xmlwriter.WriteElementString("BootBuadRate", bootRate.ToString());
                    xmlwriter.WriteElementString("OsdBuadRate", osdRate.ToString());

                    xmlwriter.WriteElementString("Pal", CHK_pal.Checked.ToString());//osd.Mode.ToString());
                    //xmlwriter.WriteElementString("Pal", CHK_pal.Checked.ToString());

                    xmlwriter.WriteEndElement();

                    xmlwriter.WriteEndDocument();
                    xmlwriter.Close();

                    //appconfig.Save();
                }
                catch (Exception ex) { MessageBox.Show(ex.ToString()); }
            }
            else
            {
                try
                {
                    using (XmlTextReader xmlreader = new XmlTextReader(Path.GetDirectoryName(Application.ExecutablePath) + Path.DirectorySeparatorChar + @"config.xml"))
                    {
                        while (xmlreader.Read())
                        {
                            xmlreader.MoveToElement();
                            try
                            {
                                switch (xmlreader.Name)
                                {
                                    case "ComPort":
                                        string temp = xmlreader.ReadString();
                                        //CMB_ComPort.Text = temp;
                                        if (CMB_ComPort.Items.IndexOf(temp) >= 0)
                                            CMB_ComPort.Text = temp;

                                        break;
                                    case "VideoMode":
                                        string temp2 = xmlreader.ReadString();
                                        CHK_pal.Checked = (temp2 == "True");
                                        break;
                                    case "BootBuadRate":
                                        int brate = 0;
                                        if (int.TryParse(xmlreader.ReadString(), out brate))
                                            bootRate = brate;
                                        break;
                                    case "OsdBuadRate":
                                        int orate = 0;
                                        if (int.TryParse(xmlreader.ReadString(), out orate))
                                            osdRate = orate;
                                        break;
                                    case "Config":
                                        break;
                                    case "xml":
                                        break;
                                    default:
                                        if (xmlreader.Name == "") // line feeds
                                            break;
                                        break;
                                }
                            }
                            catch (Exception ee) { Console.WriteLine(ee.Message); } // silent fail on bad entry
                        }
                    }
                }
                catch (Exception ex) { Console.WriteLine("Bad Config File: " + ex.ToString()); } // bad config file
            }
        }

        private void testToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //DTR d = new DTR();
            //d.Show();
        }

        private void updateFontToolStripMenuItem_Click(object sender, EventArgs e)
        {
            toolStripProgressBar1.Style = ProgressBarStyle.Continuous;
            toolStripProgressBar1.Maximum = 256;

            toolStripStatusLabel1.Text = "";

            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "mcm|*.mcm";

            if (ofd.ShowDialog() != DialogResult.OK)
                return;

            byte[][] fonts = mcm.readNVM(ofd.FileName);

            try
            {
                EnterCLI();
                //comPort.ReadTimeout = 1000;

                comPort.Write("F");

                int timeout = 0;

                while (comPort.BytesToRead == 0)
                {
                    System.Threading.Thread.Sleep(500);
                    Console.WriteLine("Waiting...");
                    timeout++;

                    if (timeout > 6)
                    {
                        MessageBox.Show("Error entering font mode - No Data");
                        comPort.Close();
                        return;
                    }
                }

                int ack = comPort.ReadByte();
                if (ack != 'F')
                {
                    MessageBox.Show("Error entering CharSet upload mode - invalid data");
                    comPort.Close();
                    return;
                }
                else
                {
                    Console.WriteLine("connected");
                }
            }
            catch { MessageBox.Show("Error opening com port", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }




            try
            {
                byte[] ck = new byte[1];
                toolStripProgressBar1.Value = 0;

                for (int i = 0; i < fonts.Length; i++)
                {
                    //Console.Write("font {0} ", i);


                    byte checksum = 0;
                    comPort.Write(fonts[i], 0, 54);

                    for (int j = 0; j < 54; j++)
                    {
                        checksum += fonts[i][j];
                    }
                    ck[0] = checksum;
                    comPort.Write(ck, 0, 1);

                    int ack = comPort.ReadByte();
                    if (ack != '!')
                    {
                        Console.WriteLine("write error");
                        break;
                    }
                    else
                    {
                        Console.WriteLine("done");
                        toolStripProgressBar1.Value = i + 1;
                    }
                }
            }
            catch (TimeoutException te)
            {
                Console.WriteLine("read timeout");
            }

            /*
            using (var stream = ofd.OpenFile())
            {

                //BinaryReader br = new BinaryReader(stream);
                StreamReader sr2 = new StreamReader(stream);

                string device = sr2.ReadLine();

                if (device != "MAX7456")
                {
                    MessageBox.Show("Invalid MCM");
                    comPort.Close();
                    return;
                }

                //br.BaseStream.Seek(0, SeekOrigin.Begin);

                //long length = br.BaseStream.Length;
                int length = (int)stream.Length;
                string line = null;

                int pos = 0;
                toolStripStatusLabel1.Text = "CharSet Uploading";
                int byte_count = 0;

                while ((line = sr2.ReadLine()) != null)//br.BaseStream.Position < br.BaseStream.Length && !this.IsDisposed)
                {
                    try
                    {
                        pos += line.Length + 2;
                        toolStripProgressBar1.Value = pos * 100 / length;//(int)((br.BaseStream.Position / (float)br.BaseStream.Length) * 100);

                        byte[] buffer = Encoding.ASCII.GetBytes(line + "\r\n");
                        //
                        for (int i = 0; i < buffer.Length; i++)
                        {
                            comPort.Write(buffer, i, 1);
                            Thread.Sleep(1);
                        }


                        //comPort.Write(buffer, 0, buffer.Length);
                        byte_count++;
                        Thread.Sleep(10);

                        int timeout = 0;

                        if (byte_count == 64)
                        {
                            while (comPort.BytesToRead == 0)
                            {
                                System.Threading.Thread.Sleep(10);
                                timeout++;

                                if (timeout > 10)
                                {
                                    MessageBox.Show("CharSet upload failed - no response");
                                    comPort.Close();
                                    return;
                                }
                            }
                            Console.WriteLine(comPort.ReadExisting());
                            byte_count = 0;
                        }

                    }
                    catch { break; }

                    Application.DoEvents();
                }

                /*
                comPort.WriteLine("\n\n\n\n\n\n\n\n\n\n\n\n\n\n");

                comPort.DtrEnable = false;
                comPort.RtsEnable = false;

                System.Threading.Thread.Sleep(50);

                comPort.DtrEnable = true;
                comPort.RtsEnable = true;

                System.Threading.Thread.Sleep(50);
                */
            comPort.Close();

            //comPort.DtrEnable = false;
            //comPort.RtsEnable = false;

            //toolStripProgressBar1.Value = 100;
            toolStripStatusLabel1.Text = "CharSet Done";
            //}
            //}
        }

        private void helpToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                System.Diagnostics.Process.Start("https://code.google.com/p/arducam-osd/wiki/arducam_osd?tm=6");
            }
            catch { MessageBox.Show("Webpage open failed... do you have a virus?"); }
        }


        private void EnterCLI()
        {
            //if (incli)
            //    return;
            //incli = true;

            if (comPort.IsOpen)
                comPort.Close();


            comPort.PortName = CMB_ComPort.Text;
            comPort.BaudRate = osdRate;
            comPort.ReadTimeout = 3000;
            comPort.Open();

            //comPort.DtrEnable = false;
            //comPort.RtsEnable = false;

            //System.Threading.Thread.Sleep(50);

            //comPort.DtrEnable = true;
            //comPort.RtsEnable = true;

            System.Threading.Thread.Sleep(1000);

            comPort.ReadExisting();
            for (int i = 0; i < 4; i++)
                comPort.Write("\r\n");
        }


        private void NUM_ValueChanged(object sender, EventArgs e)
        {
            try
            {
                OSDItem info = (OSDItem)Array.IndexOf(OSDItemName.Name, LIST_items.SelectedItem.ToString());
                osd.SetItemPosition(info, new Point((int)NUM_X.Value, (int)NUM_Y.Value));
                osd.Draw();
            }
            catch { return; }
        }
    }
}