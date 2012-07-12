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
using System.Globalization;
using System.Resources;
using OSDConfig.Properties;


namespace OSDConfig
{
    public partial class OSDConfigForm : Form
    {

        ResourceManager rmMessages = new ResourceManager("OSDConfig.Messages", typeof(OSDConfigForm).Assembly);
        ResourceManager rmItemNames = new ResourceManager("OSDConfig.OSDItemNames", typeof(OSDConfigForm).Assembly);

        ArduOSDPort osdPort = new ArduOSDPort();

        bool fromuser = true;

        ToolStripMenuItem[] langMenus;
        CultureInfo[] langs;


        string lang = "";
        bool pal = true;
        int bootRate = 9600;
        int osdRate = 57600;
        string bgImage = "vlcsnap-2012-01-28-07h46m04s95.png";
        string comPort = "";


        class ADReading
        {
            public int reading = 0;
            public decimal value = 0;
        }
        List<ADReading[]> adreadings = new List<ADReading[]>();


        bool fromOSD = true;


        public OSDConfigForm()
        {
            xmlconfig(false);
            InitializeComponent();
            langMenus = new ToolStripMenuItem[] { EnglishUIToolStripMenuItem, PolishUIToolStripMenuItem, ChineseUIToolStripMenuItem };
            langs = new CultureInfo[] { new CultureInfo("en-US"), new CultureInfo("pl"), new CultureInfo("zh-Hans") };
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
            if (fromOSD)
            {
                if (osd.SelectedItem == OSDItem.NULL)
                    LIST_items.SelectedItem = null;
                else
                {
                    bool isalt = false;
                    foreach (var alt in OSDItemList.Alternates)
                        if (osd.SelectedItem == alt.Key && osd.Setting.IsEnabled(alt.Value))
                        {
                            //LIST_items.SelectedItem = OSDItemList.Names[(int)alt.Value];
                            LIST_items.SelectedIndex = Array.IndexOf(OSDItemList.Avaliable, alt.Value);
                            isalt = true;
                            break;
                        }
                    if (!isalt)
                        //LIST_items.SelectedItem = OSDItemList.Names[(int)osd.SelectedItem];
                        LIST_items.SelectedIndex = Array.IndexOf(OSDItemList.Avaliable, osd.SelectedItem);
                }
            }
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
            //xmlconfig(false);

            string strVersion = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();
            this.Text = this.Text + " " + strVersion;

            //string lang = Thread.CurrentThread.CurrentUICulture.Name;

            if (lang.StartsWith("zh", StringComparison.CurrentCultureIgnoreCase))
                ChineseUIToolStripMenuItem.Checked = true;
            else if (lang.StartsWith("pl", StringComparison.CurrentCultureIgnoreCase))
                PolishUIToolStripMenuItem.Checked = true;
            else
                EnglishUIToolStripMenuItem.Checked = true;

            CHK_pal.Checked = pal;

            CMB_ComPort.Items.AddRange(GetPortNames());

            if (CMB_ComPort.Items.IndexOf(comPort) >= 0)
                CMB_ComPort.Text = comPort;


            for (int i = 0; i < OSDItemList.Avaliable.Length; i++)
                LIST_items.Items.Add(rmItemNames.GetString(OSDItemList.Avaliable[i].ToString()),
                osd.Setting.IsEnabled(OSDItemList.Avaliable[i]));

            for (int i = 0; i < osd.Setting.ad_setting.Length; i++)
            {
                var r = new ADReading[] { 
                    new ADReading(), 
                    new ADReading()};
                adreadings.Add(r);
            }

            osd.SelectedItemChanged += new EventHandler(osd_SelectedItemChanged);
            osd.ItemPositionChanged += new EventHandler(osd_ItemPositionChanged);

            osdPort.ReadTimeout = 2000;
            osdPort.BaudRate = osdRate;

            osd.Chars = mcm.readMCM2("OSD_SA_v5.mcm");

            try
            {
                osd.BackgroundImage/*bgpicture*/ = Image.FromFile(bgImage);
            }
            catch { }

            osd.Draw();

            cbFunction.SelectedIndex = 0;
        }

        private void LIST_items_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (((CheckedListBox)sender).SelectedItem == null)
            {
                NUM_X.Value = NUM_Y.Value = 0;
            }
            else
            {

                //string item = ((CheckedListBox)sender).SelectedItem.ToString();
                OSDItem sel = OSDItemList.Avaliable[LIST_items.SelectedIndex];//(OSDItem)Array.IndexOf(OSDItemList.Names, item);


                foreach (var alt in OSDItemList.Alternates)
                    if (sel == alt.Value)
                    {
                        sel = alt.Key;
                        break;
                    }

                fromOSD = false;
                osd.SelectedItem = sel;
                fromOSD = true;

                Point p = osd.GetItemPosition(sel);
                NUM_X.Value = p.X;
                NUM_Y.Value = p.Y;
                osd.Draw();
            }
        }

        private void LIST_items_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            // if (((CheckedListBox)sender).SelectedItem != null && ((CheckedListBox)sender).SelectedItem.ToString() == "Horizon")
            int idx = -1;
            if (((CheckedListBox)sender).SelectedItem != null)
            {
                OSDItem item = OSDItemList.Avaliable[LIST_items.SelectedIndex];//(OSDItem)Array.IndexOf(OSDItemList.Names, ((CheckedListBox)sender).SelectedItem);

                if (e.NewValue == CheckState.Checked)
                {
                    foreach (var conflict in OSDItemList.Conflits)
                    {
                        if (item == conflict.Key && (idx = Array.IndexOf(OSDItemList.Avaliable, conflict.Value)) >= 0)
                        {
                            LIST_items.SetItemChecked(idx, false);
                            osd.SetItemEnabled(conflict.Value, false);
                        }
                        else if (item == conflict.Value && (idx = Array.IndexOf(OSDItemList.Avaliable, conflict.Key)) >= 0)
                        {
                            LIST_items.SetItemChecked(idx, false);
                            osd.SetItemEnabled(conflict.Key, false);
                        }
                    }
                }

                foreach (var alt in OSDItemList.Alternates)
                {
                    if (item == alt.Key && e.NewValue == CheckState.Checked && (idx = Array.IndexOf(OSDItemList.Avaliable, alt.Value)) >= 0)
                    {
                        LIST_items.SetItemChecked(idx, false);
                        osd.SetItemEnabled(alt.Value, false);
                        break;
                    }
                    else if (item == alt.Value)
                    {
                        if (e.NewValue == CheckState.Checked
                            //&& OSDItemList.Names[(int)alt.Key] != null
                            && (idx = Array.IndexOf(OSDItemList.Avaliable, alt.Key)) >= 0)
                            LIST_items.SetItemChecked(idx, false);
                        osd.SetItemEnabled(alt.Key, e.NewValue == CheckState.Checked);
                        break;
                    }
                }

                osd.SetItemEnabled(item, e.NewValue == CheckState.Checked);
            }

            // add a delay to this so it runs after the control value has been defined.
            if (this.IsHandleCreated)
                this.BeginInvoke((MethodInvoker)delegate { /*osdDraw();*/osd.Draw(); });
        }


        private void BUT_WriteOSD_Click(object sender, EventArgs e)
        {
            //OSDConfig.Messages.


            toolStripProgressBar1.Style = ProgressBarStyle.Continuous;
            toolStripProgressBar1.Value = 0;
            toolStripProgressBar1.Maximum = 100;
            this.toolStripStatusLabel1.Text = "";

            osdPort.PortName = CMB_ComPort.Text;
            osdPort.Open();
            bool ok = osdPort.UploadSetting(osd.Setting);
            osdPort.Close();
            if (ok)
                MessageBox.Show(this, rmMessages.GetString("Write_OSD_Done"), rmMessages.GetString("Info"), MessageBoxButtons.OK, MessageBoxIcon.Information);
            else
                MessageBox.Show(this, rmMessages.GetString("Write_OSD_Failed"), rmMessages.GetString("Error"), MessageBoxButtons.OK, MessageBoxIcon.Error);
        }


        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            osd.ShowGrid = showGrid.Checked;
            osd.Draw();
        }

        void LoadSetting(OSDSetting setting)
        {
            for (int i = 0; i < OSDItemList.Avaliable.Length; i++)
            {
                bool alt = false;
                foreach (var a in OSDItemList.Alternates)
                    if (OSDItemList.Avaliable[i] == a.Key && setting.IsEnabled(a.Value))
                    {
                        LIST_items.SetItemChecked(i, false);
                        alt = true;
                    }

                if (!alt)
                    LIST_items.SetItemChecked(i, setting.IsEnabled(OSDItemList.Avaliable[i]));
            }

            foreach (var r in adreadings)
            {
                r[0].reading = r[1].reading = 0;
                r[0].value = r[1].value = 0;
            }

            cbFunction_SelectedIndexChanged(this, new EventArgs());

            osd.Setting = setting;
            osd.Draw();
        }

        private void BUT_ReadOSD_Click(object sender, EventArgs e)
        {
            toolStripProgressBar1.Style = ProgressBarStyle.Continuous;
            toolStripProgressBar1.Maximum = 100;
            toolStripProgressBar1.Value = 0;
            this.toolStripStatusLabel1.Text = "";

            //ArduinoSTK sp;
            OSDSetting setting;
            osdPort.PortName = CMB_ComPort.Text;
            osdPort.Open();
            bool ok = osdPort.GetSetting(out setting);
            osdPort.Close();
            if (ok)
            {
                LoadSetting(setting);
                MessageBox.Show(this, rmMessages.GetString("Read_OSD_Done"), rmMessages.GetString("Info"), MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show(this, rmMessages.GetString("Read_OSD_Failed"), rmMessages.GetString("Error"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            osdPort.Close();
        }

        void sp_Progress(int progress)
        {
            toolStripStatusLabel1.Text = rmMessages.GetString("Uploading") + " " + progress + " %";
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

            if (sfd.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    using (Stream s = sfd.OpenFile())
                    {

                        byte[] bytes = Encoding.ASCII.GetBytes("HOSD");
                        s.Write(bytes, 0, 4);
                        bytes = osd.Setting.ToBytes();
                        s.Write(BitConverter.GetBytes(bytes.Length), 0, sizeof(int));
                        s.Write(bytes, 0, bytes.Length);
                    }
                }
                catch
                {
                    MessageBox.Show(this, rmMessages.GetString("Save_Setting_Failed"), rmMessages.GetString("Error"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void loadFromFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog() { Filter = "*.osd|*.osd" };
            bool ok = false;

            if (ofd.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    using (Stream f = ofd.OpenFile())
                    {
                        OSDSetting setting = new OSDSetting();
                        byte[] buf = new byte[128];
                        f.Read(buf, 0, 4);
                        if (Encoding.ASCII.GetString(buf, 0, 4) == "HOSD")
                        {
                            f.Read(buf, 0, 4);
                            f.Read(buf, 0, BitConverter.ToInt32(buf, 0));
                            if (ok = setting.FromBytes(buf, 0))
                                LoadSetting(setting);
                        }
                        if (!ok)
                            MessageBox.Show(this, rmMessages.GetString("Invalid_File_Format"), rmMessages.GetString("Error"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                catch
                {
                    MessageBox.Show(this, rmMessages.GetString("Read_Setting_Failed"), rmMessages.GetString("Error"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }

            osd.Draw();
        }

        private void loadDefaultsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //osd.Setting = new OSDSetting();
            LoadSetting(new OSDSetting());
            //osd.Draw();
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

            if (ofd.ShowDialog() == DialogResult.OK)
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

                toolStripStatusLabel1.Text = "Rebooting";

                MegaLoad sp = new MegaLoad();
                toolStripProgressBar1.Maximum = 100;
                toolStripProgressBar1.Style = ProgressBarStyle.Continuous;
                toolStripProgressBar1.Value = 0;

                sp.Progress += sp_Progress;
                sp.Connected += (s, ce) => { toolStripStatusLabel1.Text = "Programming"; };
                try
                {

                    osdPort.PortName = CMB_ComPort.Text;
                    osdPort.Open();
                    osdPort.Reboot();
                    osdPort.Close();


                    sp.PortName = CMB_ComPort.Text;
                    sp.BaudRate = bootRate;
                    sp.WriteBufferSize = 32;
                    sp.ReadTimeout = 5000;

                    sp.Open();
                }
                catch { MessageBox.Show("Error opening com port", rmMessages.GetString("Error"), MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

                toolStripStatusLabel1.Text = "Connecting";

                //sp.ReadExisting();
                fail = !sp.Upload(FLASH, null);
                sp.Close();

                if (!fail)
                {
                    toolStripStatusLabel1.Text = "Program Done";
                }
                else
                {
                    toolStripStatusLabel1.Text = "Program Failed";
                }
            }
        }

        private void customBGPictureToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "jpg, bmp or png|*.jpg;*.bmp;*.png";

            if (ofd.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    osd.BackgroundImage = Image.FromFile(ofd.FileName);
                    osd.Draw();
                    bgImage = ofd.FileName;
                }
                catch { MessageBox.Show("Bad Image"); }
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
                if (osdPort.IsOpen)
                    osdPort.Close();

                try
                {
                    osdPort.PortName = CMB_ComPort.Text;
                    osdPort.BaudRate = osdRate;
                    osdPort.Open();

                }
                catch
                {
                    MessageBox.Show("Error opening com port", rmMessages.GetString("Error"), MessageBoxButtons.OK, MessageBoxIcon.Error); return;
                }

                BinaryReader br = new BinaryReader(ofd.OpenFile());

                this.toolStripProgressBar1.Style = ProgressBarStyle.Marquee;
                this.toolStripStatusLabel1.Text = "Sending TLOG data...";

                while (br.BaseStream.Position < br.BaseStream.Length && !this.IsDisposed)
                    while (true)
                    {
                        try
                        {
                            byte[] bytes = br.ReadBytes(20);
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

                    osdPort.Close();
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

                    xmlwriter.WriteElementString("ComPort", CMB_ComPort == null ? "" : CMB_ComPort.Text);
                    xmlwriter.WriteElementString("BootBuadRate", bootRate.ToString());
                    xmlwriter.WriteElementString("OsdBuadRate", osdRate.ToString());

                    xmlwriter.WriteElementString("Pal", CHK_pal == null ? "True" : CHK_pal.Checked.ToString());//osd.Mode.ToString());
                    //xmlwriter.WriteElementString("Pal", CHK_pal.Checked.ToString());
                    xmlwriter.WriteElementString("BackgroudImage", bgImage);
                    xmlwriter.WriteElementString("Language", (lang = Thread.CurrentThread.CurrentUICulture.Name));

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
                                        comPort = xmlreader.ReadString();
                                        break;
                                    case "Pal":
                                        bool.TryParse(xmlreader.ReadString(), out pal);
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
                                    case "BackgroundImage":
                                        bgImage = xmlreader.ReadString();
                                        break;
                                    case "Language":
                                        try
                                        {
                                            lang = xmlreader.ReadString();
                                            Thread.CurrentThread.CurrentUICulture = new CultureInfo(lang);
                                        }
                                        catch (Exception)
                                        {
                                        }
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

            bool ok = true;

            if (ofd.ShowDialog() != DialogResult.OK)
                return;

            osdPort.PortName = CMB_ComPort.Text;
            osdPort.Open();
            ok = osdPort.UploadFont(ofd.FileName);
            osdPort.Close();

            if (ok)
            {
                MessageBox.Show(this, rmMessages.GetString("Upload_CharSet_Done"), rmMessages.GetString("Info"), MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show(this, rmMessages.GetString("Upload_CharSet_Failed"), rmMessages.GetString("Error"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            //if (ok)
            //    toolStripStatusLabel1.Text = "CharSet Done";
            //else
            //    toolStripStatusLabel1.Text = "Update CharSet Failed";

        }

        private void helpToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                System.Diagnostics.Process.Start("http://code.google.com/p/hazys-osd/wiki/Introduction?tm=6");
            }
            catch { MessageBox.Show("Webpage open failed... do you have a virus?"); }
        }


        private void NUM_ValueChanged(object sender, EventArgs e)
        {
            try
            {
                //OSDItem info = (OSDItem)Array.IndexOf(OSDItemList.Names, LIST_items.SelectedItem.ToString());
                OSDItem item = OSDItemList.Avaliable[LIST_items.SelectedIndex];
                foreach (var alt in OSDItemList.Alternates)
                    if (item == alt.Value)
                    {
                        item = alt.Key;
                        break;
                    }

                osd.SetItemPosition(item, new Point((int)NUM_X.Value, (int)NUM_Y.Value));
                osd.Draw();
            }
            catch { return; }
        }

        private void CMB_ComPort_Click(object sender, EventArgs e)
        {
            CMB_ComPort.Items.Clear();
            CMB_ComPort.Items.AddRange(GetPortNames());
        }

        private void showGrid_Click(object sender, EventArgs e)
        {
            osd.ShowGrid = showGrid.Checked;
            osd.Draw();
        }

        private void numVperB_ValueChanged(object sender, EventArgs e)
        {
            if (cbFunction.SelectedIndex >= 0 && fromuser)
                osd.Setting.ad_setting[cbFunction.SelectedIndex].k = (float)numVat0.Value;
        }

        private void numVat0_ValueChanged(object sender, EventArgs e)
        {
            if (cbFunction.SelectedIndex >= 0 && fromuser)
                osd.Setting.ad_setting[cbFunction.SelectedIndex].b = (float)numVat0.Value;
        }

        private void num1_ValueChanged(object sender, EventArgs e)
        {
            if (cbFunction.SelectedIndex >= 0 && fromuser)
            {
                adreadings[cbFunction.SelectedIndex][0].value = num1.Value;
                Calibrate();
            }
        }

        private void num2_ValueChanged(object sender, EventArgs e)
        {
            if (cbFunction.SelectedIndex >= 0 && fromuser)
            {
                adreadings[cbFunction.SelectedIndex][1].value = num2.Value;
                Calibrate();
            }
        }

        private void lReading1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            ushort reading = 0;
            int channel = cbChannel.SelectedIndex;
            int func = cbFunction.SelectedIndex;

            bool ok = osdPort.GetAnalog(channel, out reading);
            osdPort.Close();
            if (ok)
            {
                tbxReading1.Text = reading.ToString();
                adreadings[func][0].reading = reading;
                Calibrate();
            }
            else
            {
                MessageBox.Show(this, rmMessages.GetString("Get_ADC_Failed"), rmMessages.GetString("Error"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void lReading2_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            ushort reading = 0;
            int channel = cbChannel.SelectedIndex;
            int func = cbFunction.SelectedIndex;

            bool ok = osdPort.GetAnalog(channel, out reading);
            osdPort.Close();
            if (ok)
            {
                tbxReading1.Text = reading.ToString();
                adreadings[func][1].reading = reading;
                Calibrate();
            }
            else
            {
                MessageBox.Show(this, rmMessages.GetString("Get_ADC_Failed"), rmMessages.GetString("Error"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void cbFunction_SelectedIndexChanged(object sender, EventArgs e)
        {
            fromuser = false;
            int idx = cbFunction.SelectedIndex;
            var reading = adreadings[idx];
            tbxReading1.Text = reading[0].reading.ToString();
            tbxReading2.Text = reading[1].reading.ToString();

            num1.Value = reading[0].value;
            num2.Value = reading[1].value;

            //Calibrate();
            numVperB.Value = (decimal)osd.Setting.ad_setting[idx].k;
            numVat0.Value = (decimal)osd.Setting.ad_setting[idx].b;
            cbChannel.SelectedIndex = osd.Setting.ad_setting[idx].channel;
            fromuser = true;
        }

        private void cbChannel_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbFunction.SelectedIndex >= 0 && fromuser)
                osd.Setting.ad_setting[cbFunction.SelectedIndex].channel = (byte)cbChannel.SelectedIndex;
        }

        void Calibrate()
        {
            //var config = ChannelConfigs[cbFunction.SelectedIndex];

            if (tbxReading1.Text != tbxReading2.Text)
            {
                numVperB.Value = (num1.Value - num2.Value) / (int.Parse(tbxReading1.Text) - int.Parse(tbxReading2.Text));
                numVat0.Value = (decimal)num1.Value - numVperB.Value * int.Parse(tbxReading1.Text);
            }
            else if (num1.Value == num2.Value)
            {
                numVperB.Value = 0;
                numVat0.Value = num1.Value;
            }
        }

        private void UILanguageToolStripMenuItem_Click(object sender, EventArgs e)
        {

            for (int i = 0; i < langMenus.Length; i++)
                if (object.ReferenceEquals(sender, langMenus[i]))
                {
                    langMenus[i].Checked = true;
                    ChangeLanguage(langs[i]);
                }
                else
                    langMenus[i].Checked = false;

        }

        private void ChangeLanguage(CultureInfo culture)
        {
            Thread.CurrentThread.CurrentUICulture = culture;

            LIST_items.Items.Clear();
            for (int i = 0; i < OSDItemList.Avaliable.Length; i++)
            {
                LIST_items.Items.Add(rmItemNames.GetString(OSDItemList.Avaliable[i].ToString()),
                    osd.Setting.IsEnabled(OSDItemList.Avaliable[i]));

                //LIST_items.Items.Add(OSDItemList.Names[(int)OSDItemList.Avaliable[i]],
                //osd.Setting.IsEnabled(OSDItemList.Avaliable[i]));
            }

            ComponentResourceManager rm = new ComponentResourceManager(this.GetType());
            rm.ApplyResources(this);

            string strVersion = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();
            this.Text = rm.GetString("$this.Text") + " " + strVersion;

            //cbFunctions
            int idx = cbFunction.SelectedIndex;
            cbFunction.Items.Clear();
            cbFunction.Items.AddRange(new object[] {
            rm.GetString("cbFunction.Items"),
            rm.GetString("cbFunction.Items1"),
            rm.GetString("cbFunction.Items2"),
            rm.GetString("cbFunction.Items3"),
            rm.GetString("cbFunction.Items4")});
            cbFunction.SelectedIndex = idx;

            xmlconfig(true);

        }

        private void PolishUIToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }
    }
}