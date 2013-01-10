namespace OSDConfig
{
    partial class OSDConfigForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(OSDConfigForm));
            OSDConfig.OSDSetting osdSetting1 = new OSDConfig.OSDSetting();
            this.LIST_items = new System.Windows.Forms.CheckedListBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.NUM_Y = new System.Windows.Forms.NumericUpDown();
            this.NUM_X = new System.Windows.Forms.NumericUpDown();
            this.BUT_WriteOSD = new System.Windows.Forms.Button();
            this.CMB_ComPort = new System.Windows.Forms.ComboBox();
            this.BUT_ReadOSD = new System.Windows.Forms.Button();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.toolStripProgressBar1 = new System.Windows.Forms.ToolStripProgressBar();
            this.toolStripStatusLabel1 = new System.Windows.Forms.ToolStripStatusLabel();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveToFileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.loadFromFileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.loadDefaultsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.videoModeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.nTSCToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.CHK_pal = new System.Windows.Forms.ToolStripMenuItem();
            this.optionsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.showGrid = new System.Windows.Forms.ToolStripMenuItem();
            this.customBGPictureToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.EnglishUIToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.SpanishUIToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.PolishUIToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.ChineseUIToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.updateFirmwareToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.updateFontToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.sendTLogToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.helpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.osd = new OSDConfig.ArduOSD();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.groupBox5 = new System.Windows.Forms.GroupBox();
            this.rbImperial = new System.Windows.Forms.RadioButton();
            this.rbMetric = new System.Windows.Forms.RadioButton();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.label9 = new System.Windows.Forms.Label();
            this.cbADEnable = new System.Windows.Forms.CheckBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.num1 = new System.Windows.Forms.NumericUpDown();
            this.lReading2 = new System.Windows.Forms.LinkLabel();
            this.tbxReading1 = new System.Windows.Forms.TextBox();
            this.tbxReading2 = new System.Windows.Forms.TextBox();
            this.num2 = new System.Windows.Forms.NumericUpDown();
            this.lReading1 = new System.Windows.Forms.LinkLabel();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.numVat0 = new System.Windows.Forms.NumericUpDown();
            this.numVperB = new System.Windows.Forms.NumericUpDown();
            this.label7 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.cbFunction = new System.Windows.Forms.ComboBox();
            this.label3 = new System.Windows.Forms.Label();
            this.cbChannel = new System.Windows.Forms.ComboBox();
            this.label6 = new System.Windows.Forms.Label();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.NUM_Y)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.NUM_X)).BeginInit();
            this.statusStrip1.SuspendLayout();
            this.menuStrip1.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.groupBox5.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.num1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.num2)).BeginInit();
            this.groupBox3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numVat0)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numVperB)).BeginInit();
            this.tableLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // LIST_items
            // 
            resources.ApplyResources(this.LIST_items, "LIST_items");
            this.LIST_items.FormattingEnabled = true;
            this.LIST_items.Name = "LIST_items";
            this.LIST_items.ItemCheck += new System.Windows.Forms.ItemCheckEventHandler(this.LIST_items_ItemCheck);
            this.LIST_items.SelectedIndexChanged += new System.EventHandler(this.LIST_items_SelectedIndexChanged);
            // 
            // groupBox1
            // 
            resources.ApplyResources(this.groupBox1, "groupBox1");
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.NUM_Y);
            this.groupBox1.Controls.Add(this.NUM_X);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.TabStop = false;
            // 
            // label2
            // 
            resources.ApplyResources(this.label2, "label2");
            this.label2.Name = "label2";
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // NUM_Y
            // 
            resources.ApplyResources(this.NUM_Y, "NUM_Y");
            this.NUM_Y.Maximum = new decimal(new int[] {
            15,
            0,
            0,
            0});
            this.NUM_Y.Name = "NUM_Y";
            this.NUM_Y.ValueChanged += new System.EventHandler(this.NUM_ValueChanged);
            // 
            // NUM_X
            // 
            resources.ApplyResources(this.NUM_X, "NUM_X");
            this.NUM_X.Maximum = new decimal(new int[] {
            29,
            0,
            0,
            0});
            this.NUM_X.Name = "NUM_X";
            this.NUM_X.ValueChanged += new System.EventHandler(this.NUM_ValueChanged);
            // 
            // BUT_WriteOSD
            // 
            resources.ApplyResources(this.BUT_WriteOSD, "BUT_WriteOSD");
            this.BUT_WriteOSD.Name = "BUT_WriteOSD";
            this.BUT_WriteOSD.UseVisualStyleBackColor = true;
            this.BUT_WriteOSD.Click += new System.EventHandler(this.BUT_WriteOSD_Click);
            // 
            // CMB_ComPort
            // 
            resources.ApplyResources(this.CMB_ComPort, "CMB_ComPort");
            this.CMB_ComPort.FormattingEnabled = true;
            this.CMB_ComPort.Name = "CMB_ComPort";
            this.CMB_ComPort.Click += new System.EventHandler(this.CMB_ComPort_Click);
            // 
            // BUT_ReadOSD
            // 
            resources.ApplyResources(this.BUT_ReadOSD, "BUT_ReadOSD");
            this.BUT_ReadOSD.Name = "BUT_ReadOSD";
            this.BUT_ReadOSD.UseVisualStyleBackColor = true;
            this.BUT_ReadOSD.Click += new System.EventHandler(this.BUT_ReadOSD_Click);
            // 
            // statusStrip1
            // 
            resources.ApplyResources(this.statusStrip1, "statusStrip1");
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripProgressBar1,
            this.toolStripStatusLabel1});
            this.statusStrip1.Name = "statusStrip1";
            // 
            // toolStripProgressBar1
            // 
            resources.ApplyResources(this.toolStripProgressBar1, "toolStripProgressBar1");
            this.toolStripProgressBar1.Name = "toolStripProgressBar1";
            // 
            // toolStripStatusLabel1
            // 
            resources.ApplyResources(this.toolStripStatusLabel1, "toolStripStatusLabel1");
            this.toolStripStatusLabel1.Name = "toolStripStatusLabel1";
            // 
            // menuStrip1
            // 
            resources.ApplyResources(this.menuStrip1, "menuStrip1");
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.videoModeToolStripMenuItem,
            this.optionsToolStripMenuItem,
            this.helpToolStripMenuItem});
            this.menuStrip1.Name = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            resources.ApplyResources(this.fileToolStripMenuItem, "fileToolStripMenuItem");
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.saveToFileToolStripMenuItem,
            this.loadFromFileToolStripMenuItem,
            this.toolStripSeparator2,
            this.loadDefaultsToolStripMenuItem,
            this.toolStripSeparator1,
            this.exitToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            // 
            // saveToFileToolStripMenuItem
            // 
            resources.ApplyResources(this.saveToFileToolStripMenuItem, "saveToFileToolStripMenuItem");
            this.saveToFileToolStripMenuItem.Image = global::OSDConfig.Properties.Resources.saveHS;
            this.saveToFileToolStripMenuItem.Name = "saveToFileToolStripMenuItem";
            this.saveToFileToolStripMenuItem.Click += new System.EventHandler(this.saveToFileToolStripMenuItem_Click);
            // 
            // loadFromFileToolStripMenuItem
            // 
            resources.ApplyResources(this.loadFromFileToolStripMenuItem, "loadFromFileToolStripMenuItem");
            this.loadFromFileToolStripMenuItem.Image = global::OSDConfig.Properties.Resources.openHS;
            this.loadFromFileToolStripMenuItem.Name = "loadFromFileToolStripMenuItem";
            this.loadFromFileToolStripMenuItem.Click += new System.EventHandler(this.loadFromFileToolStripMenuItem_Click);
            // 
            // toolStripSeparator2
            // 
            resources.ApplyResources(this.toolStripSeparator2, "toolStripSeparator2");
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            // 
            // loadDefaultsToolStripMenuItem
            // 
            resources.ApplyResources(this.loadDefaultsToolStripMenuItem, "loadDefaultsToolStripMenuItem");
            this.loadDefaultsToolStripMenuItem.Name = "loadDefaultsToolStripMenuItem";
            this.loadDefaultsToolStripMenuItem.Click += new System.EventHandler(this.loadDefaultsToolStripMenuItem_Click);
            // 
            // toolStripSeparator1
            // 
            resources.ApplyResources(this.toolStripSeparator1, "toolStripSeparator1");
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            // 
            // exitToolStripMenuItem
            // 
            resources.ApplyResources(this.exitToolStripMenuItem, "exitToolStripMenuItem");
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.Click += new System.EventHandler(this.exitToolStripMenuItem_Click);
            // 
            // videoModeToolStripMenuItem
            // 
            resources.ApplyResources(this.videoModeToolStripMenuItem, "videoModeToolStripMenuItem");
            this.videoModeToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.nTSCToolStripMenuItem,
            this.CHK_pal});
            this.videoModeToolStripMenuItem.Name = "videoModeToolStripMenuItem";
            // 
            // nTSCToolStripMenuItem
            // 
            resources.ApplyResources(this.nTSCToolStripMenuItem, "nTSCToolStripMenuItem");
            this.nTSCToolStripMenuItem.CheckOnClick = true;
            this.nTSCToolStripMenuItem.Name = "nTSCToolStripMenuItem";
            this.nTSCToolStripMenuItem.CheckStateChanged += new System.EventHandler(this.nTSCToolStripMenuItem_CheckStateChanged);
            // 
            // CHK_pal
            // 
            resources.ApplyResources(this.CHK_pal, "CHK_pal");
            this.CHK_pal.Checked = true;
            this.CHK_pal.CheckOnClick = true;
            this.CHK_pal.CheckState = System.Windows.Forms.CheckState.Checked;
            this.CHK_pal.Name = "CHK_pal";
            this.CHK_pal.CheckedChanged += new System.EventHandler(this.CHK_pal_CheckedChanged);
            this.CHK_pal.CheckStateChanged += new System.EventHandler(this.pALToolStripMenuItem_CheckStateChanged);
            // 
            // optionsToolStripMenuItem
            // 
            resources.ApplyResources(this.optionsToolStripMenuItem, "optionsToolStripMenuItem");
            this.optionsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.showGrid,
            this.customBGPictureToolStripMenuItem,
            this.toolStripMenuItem1,
            this.toolStripSeparator3,
            this.updateFirmwareToolStripMenuItem,
            this.updateFontToolStripMenuItem,
            this.sendTLogToolStripMenuItem});
            this.optionsToolStripMenuItem.Name = "optionsToolStripMenuItem";
            // 
            // showGrid
            // 
            resources.ApplyResources(this.showGrid, "showGrid");
            this.showGrid.Checked = true;
            this.showGrid.CheckOnClick = true;
            this.showGrid.CheckState = System.Windows.Forms.CheckState.Checked;
            this.showGrid.Name = "showGrid";
            this.showGrid.CheckedChanged += new System.EventHandler(this.checkBox1_CheckedChanged);
            this.showGrid.Click += new System.EventHandler(this.showGrid_Click);
            // 
            // customBGPictureToolStripMenuItem
            // 
            resources.ApplyResources(this.customBGPictureToolStripMenuItem, "customBGPictureToolStripMenuItem");
            this.customBGPictureToolStripMenuItem.Name = "customBGPictureToolStripMenuItem";
            this.customBGPictureToolStripMenuItem.Click += new System.EventHandler(this.customBGPictureToolStripMenuItem_Click);
            // 
            // toolStripMenuItem1
            // 
            resources.ApplyResources(this.toolStripMenuItem1, "toolStripMenuItem1");
            this.toolStripMenuItem1.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.EnglishUIToolStripMenuItem,
            this.SpanishUIToolStripMenuItem,
            this.PolishUIToolStripMenuItem,
            this.ChineseUIToolStripMenuItem});
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            // 
            // EnglishUIToolStripMenuItem
            // 
            resources.ApplyResources(this.EnglishUIToolStripMenuItem, "EnglishUIToolStripMenuItem");
            this.EnglishUIToolStripMenuItem.Name = "EnglishUIToolStripMenuItem";
            this.EnglishUIToolStripMenuItem.Click += new System.EventHandler(this.UILanguageToolStripMenuItem_Click);
            // 
            // SpanishUIToolStripMenuItem
            // 
            resources.ApplyResources(this.SpanishUIToolStripMenuItem, "SpanishUIToolStripMenuItem");
            this.SpanishUIToolStripMenuItem.Name = "SpanishUIToolStripMenuItem";
            this.SpanishUIToolStripMenuItem.Click += new System.EventHandler(this.UILanguageToolStripMenuItem_Click);
            // 
            // PolishUIToolStripMenuItem
            // 
            resources.ApplyResources(this.PolishUIToolStripMenuItem, "PolishUIToolStripMenuItem");
            this.PolishUIToolStripMenuItem.Name = "PolishUIToolStripMenuItem";
            this.PolishUIToolStripMenuItem.Click += new System.EventHandler(this.UILanguageToolStripMenuItem_Click);
            // 
            // ChineseUIToolStripMenuItem
            // 
            resources.ApplyResources(this.ChineseUIToolStripMenuItem, "ChineseUIToolStripMenuItem");
            this.ChineseUIToolStripMenuItem.Name = "ChineseUIToolStripMenuItem";
            this.ChineseUIToolStripMenuItem.Click += new System.EventHandler(this.UILanguageToolStripMenuItem_Click);
            // 
            // toolStripSeparator3
            // 
            resources.ApplyResources(this.toolStripSeparator3, "toolStripSeparator3");
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            // 
            // updateFirmwareToolStripMenuItem
            // 
            resources.ApplyResources(this.updateFirmwareToolStripMenuItem, "updateFirmwareToolStripMenuItem");
            this.updateFirmwareToolStripMenuItem.Name = "updateFirmwareToolStripMenuItem";
            this.updateFirmwareToolStripMenuItem.Click += new System.EventHandler(this.updateFirmwareToolStripMenuItem_Click);
            // 
            // updateFontToolStripMenuItem
            // 
            resources.ApplyResources(this.updateFontToolStripMenuItem, "updateFontToolStripMenuItem");
            this.updateFontToolStripMenuItem.Name = "updateFontToolStripMenuItem";
            this.updateFontToolStripMenuItem.Click += new System.EventHandler(this.updateFontToolStripMenuItem_Click);
            // 
            // sendTLogToolStripMenuItem
            // 
            resources.ApplyResources(this.sendTLogToolStripMenuItem, "sendTLogToolStripMenuItem");
            this.sendTLogToolStripMenuItem.Name = "sendTLogToolStripMenuItem";
            this.sendTLogToolStripMenuItem.Click += new System.EventHandler(this.sendTLogToolStripMenuItem_Click);
            // 
            // helpToolStripMenuItem
            // 
            resources.ApplyResources(this.helpToolStripMenuItem, "helpToolStripMenuItem");
            this.helpToolStripMenuItem.Name = "helpToolStripMenuItem";
            this.helpToolStripMenuItem.Click += new System.EventHandler(this.helpToolStripMenuItem_Click);
            // 
            // tabControl1
            // 
            resources.ApplyResources(this.tabControl1, "tabControl1");
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            // 
            // tabPage1
            // 
            resources.ApplyResources(this.tabPage1, "tabPage1");
            this.tabPage1.Controls.Add(this.osd);
            this.tabPage1.Controls.Add(this.LIST_items);
            this.tabPage1.Controls.Add(this.groupBox1);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // osd
            // 
            resources.ApplyResources(this.osd, "osd");
            this.osd.Chars = null;
            this.osd.Mode = OSDConfig.VideoMode.PAL;
            this.osd.Name = "osd";
            this.osd.SelectedItem = OSDConfig.OSDItem.NULL;
            this.osd.Setting = osdSetting1;
            this.osd.ShowGrid = true;
            // 
            // tabPage2
            // 
            resources.ApplyResources(this.tabPage2, "tabPage2");
            this.tabPage2.Controls.Add(this.groupBox5);
            this.tabPage2.Controls.Add(this.groupBox4);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // groupBox5
            // 
            resources.ApplyResources(this.groupBox5, "groupBox5");
            this.groupBox5.Controls.Add(this.rbImperial);
            this.groupBox5.Controls.Add(this.rbMetric);
            this.groupBox5.Name = "groupBox5";
            this.groupBox5.TabStop = false;
            // 
            // rbImperial
            // 
            resources.ApplyResources(this.rbImperial, "rbImperial");
            this.rbImperial.Name = "rbImperial";
            this.rbImperial.TabStop = true;
            this.rbImperial.UseVisualStyleBackColor = true;
            // 
            // rbMetric
            // 
            resources.ApplyResources(this.rbMetric, "rbMetric");
            this.rbMetric.Name = "rbMetric";
            this.rbMetric.TabStop = true;
            this.rbMetric.UseVisualStyleBackColor = true;
            this.rbMetric.CheckedChanged += new System.EventHandler(this.rbMetric_CheckedChanged);
            // 
            // groupBox4
            // 
            resources.ApplyResources(this.groupBox4, "groupBox4");
            this.groupBox4.Controls.Add(this.label9);
            this.groupBox4.Controls.Add(this.cbADEnable);
            this.groupBox4.Controls.Add(this.groupBox2);
            this.groupBox4.Controls.Add(this.groupBox3);
            this.groupBox4.Controls.Add(this.cbFunction);
            this.groupBox4.Controls.Add(this.label3);
            this.groupBox4.Controls.Add(this.cbChannel);
            this.groupBox4.Controls.Add(this.label6);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.TabStop = false;
            // 
            // label9
            // 
            resources.ApplyResources(this.label9, "label9");
            this.label9.Name = "label9";
            // 
            // cbADEnable
            // 
            resources.ApplyResources(this.cbADEnable, "cbADEnable");
            this.cbADEnable.Name = "cbADEnable";
            this.cbADEnable.UseVisualStyleBackColor = true;
            this.cbADEnable.CheckedChanged += new System.EventHandler(this.cbADEnable_CheckedChanged);
            // 
            // groupBox2
            // 
            resources.ApplyResources(this.groupBox2, "groupBox2");
            this.groupBox2.Controls.Add(this.num1);
            this.groupBox2.Controls.Add(this.lReading2);
            this.groupBox2.Controls.Add(this.tbxReading1);
            this.groupBox2.Controls.Add(this.tbxReading2);
            this.groupBox2.Controls.Add(this.num2);
            this.groupBox2.Controls.Add(this.lReading1);
            this.groupBox2.Controls.Add(this.label4);
            this.groupBox2.Controls.Add(this.label5);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.TabStop = false;
            // 
            // num1
            // 
            resources.ApplyResources(this.num1, "num1");
            this.num1.DecimalPlaces = 1;
            this.num1.Name = "num1";
            this.num1.ValueChanged += new System.EventHandler(this.num1_ValueChanged);
            // 
            // lReading2
            // 
            resources.ApplyResources(this.lReading2, "lReading2");
            this.lReading2.Name = "lReading2";
            this.lReading2.TabStop = true;
            this.lReading2.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.lReading2_LinkClicked);
            // 
            // tbxReading1
            // 
            resources.ApplyResources(this.tbxReading1, "tbxReading1");
            this.tbxReading1.Name = "tbxReading1";
            // 
            // tbxReading2
            // 
            resources.ApplyResources(this.tbxReading2, "tbxReading2");
            this.tbxReading2.Name = "tbxReading2";
            // 
            // num2
            // 
            resources.ApplyResources(this.num2, "num2");
            this.num2.DecimalPlaces = 1;
            this.num2.Name = "num2";
            this.num2.ValueChanged += new System.EventHandler(this.num2_ValueChanged);
            // 
            // lReading1
            // 
            resources.ApplyResources(this.lReading1, "lReading1");
            this.lReading1.Name = "lReading1";
            this.lReading1.TabStop = true;
            this.lReading1.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.lReading1_LinkClicked);
            // 
            // label4
            // 
            resources.ApplyResources(this.label4, "label4");
            this.label4.Name = "label4";
            // 
            // label5
            // 
            resources.ApplyResources(this.label5, "label5");
            this.label5.Name = "label5";
            // 
            // groupBox3
            // 
            resources.ApplyResources(this.groupBox3, "groupBox3");
            this.groupBox3.Controls.Add(this.numVat0);
            this.groupBox3.Controls.Add(this.numVperB);
            this.groupBox3.Controls.Add(this.label7);
            this.groupBox3.Controls.Add(this.label8);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.TabStop = false;
            // 
            // numVat0
            // 
            resources.ApplyResources(this.numVat0, "numVat0");
            this.numVat0.DecimalPlaces = 2;
            this.numVat0.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.numVat0.Minimum = new decimal(new int[] {
            10000,
            0,
            0,
            -2147483648});
            this.numVat0.Name = "numVat0";
            this.numVat0.ValueChanged += new System.EventHandler(this.numVat0_ValueChanged);
            // 
            // numVperB
            // 
            resources.ApplyResources(this.numVperB, "numVperB");
            this.numVperB.DecimalPlaces = 4;
            this.numVperB.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.numVperB.Minimum = new decimal(new int[] {
            10000,
            0,
            0,
            -2147483648});
            this.numVperB.Name = "numVperB";
            this.numVperB.ValueChanged += new System.EventHandler(this.numVperB_ValueChanged);
            // 
            // label7
            // 
            resources.ApplyResources(this.label7, "label7");
            this.label7.Name = "label7";
            // 
            // label8
            // 
            resources.ApplyResources(this.label8, "label8");
            this.label8.Name = "label8";
            // 
            // cbFunction
            // 
            resources.ApplyResources(this.cbFunction, "cbFunction");
            this.cbFunction.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbFunction.FormattingEnabled = true;
            this.cbFunction.Items.AddRange(new object[] {
            resources.GetString("cbFunction.Items"),
            resources.GetString("cbFunction.Items1"),
            resources.GetString("cbFunction.Items2"),
            resources.GetString("cbFunction.Items3"),
            resources.GetString("cbFunction.Items4")});
            this.cbFunction.Name = "cbFunction";
            this.cbFunction.SelectedIndexChanged += new System.EventHandler(this.cbFunction_SelectedIndexChanged);
            // 
            // label3
            // 
            resources.ApplyResources(this.label3, "label3");
            this.label3.Name = "label3";
            // 
            // cbChannel
            // 
            resources.ApplyResources(this.cbChannel, "cbChannel");
            this.cbChannel.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbChannel.FormattingEnabled = true;
            this.cbChannel.Items.AddRange(new object[] {
            resources.GetString("cbChannel.Items"),
            resources.GetString("cbChannel.Items1"),
            resources.GetString("cbChannel.Items2"),
            resources.GetString("cbChannel.Items3"),
            resources.GetString("cbChannel.Items4"),
            resources.GetString("cbChannel.Items5"),
            resources.GetString("cbChannel.Items6"),
            resources.GetString("cbChannel.Items7")});
            this.cbChannel.Name = "cbChannel";
            this.cbChannel.SelectedIndexChanged += new System.EventHandler(this.cbChannel_SelectedIndexChanged);
            // 
            // label6
            // 
            resources.ApplyResources(this.label6, "label6");
            this.label6.Name = "label6";
            // 
            // tableLayoutPanel1
            // 
            resources.ApplyResources(this.tableLayoutPanel1, "tableLayoutPanel1");
            this.tableLayoutPanel1.Controls.Add(this.BUT_WriteOSD, 3, 0);
            this.tableLayoutPanel1.Controls.Add(this.BUT_ReadOSD, 2, 0);
            this.tableLayoutPanel1.Controls.Add(this.CMB_ComPort, 1, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            // 
            // OSDConfigForm
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.tableLayoutPanel1);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "OSDConfigForm";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.OSD_FormClosed);
            this.Load += new System.EventHandler(this.OSD_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.NUM_Y)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.NUM_X)).EndInit();
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage2.ResumeLayout(false);
            this.groupBox5.ResumeLayout(false);
            this.groupBox5.PerformLayout();
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.num1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.num2)).EndInit();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numVat0)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numVperB)).EndInit();
            this.tableLayoutPanel1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.CheckedListBox LIST_items;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.NumericUpDown NUM_Y;
        private System.Windows.Forms.NumericUpDown NUM_X;
        private System.Windows.Forms.Button BUT_WriteOSD;
        private System.Windows.Forms.ComboBox CMB_ComPort;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button BUT_ReadOSD;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripProgressBar toolStripProgressBar1;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem videoModeToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem nTSCToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem CHK_pal;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveToFileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem loadFromFileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem loadDefaultsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem optionsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem showGrid;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem updateFirmwareToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem customBGPictureToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem sendTLogToolStripMenuItem;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel1;
        private System.Windows.Forms.ToolStripMenuItem updateFontToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem helpToolStripMenuItem;
        private ArduOSD osd;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.ComboBox cbChannel;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ComboBox cbFunction;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.NumericUpDown num1;
        private System.Windows.Forms.LinkLabel lReading2;
        private System.Windows.Forms.TextBox tbxReading1;
        private System.Windows.Forms.TextBox tbxReading2;
        private System.Windows.Forms.NumericUpDown num2;
        private System.Windows.Forms.LinkLabel lReading1;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.NumericUpDown numVat0;
        private System.Windows.Forms.NumericUpDown numVperB;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem EnglishUIToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem ChineseUIToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem PolishUIToolStripMenuItem;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.ToolStripMenuItem SpanishUIToolStripMenuItem;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.CheckBox cbADEnable;
        private System.Windows.Forms.GroupBox groupBox5;
        private System.Windows.Forms.RadioButton rbImperial;
        private System.Windows.Forms.RadioButton rbMetric;
    }
}

