namespace OSDConfig
{
    partial class ADConfig
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ADConfig));
            this.btnOK = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.cbFunction = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.tbxReading1 = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.tbxReading2 = new System.Windows.Forms.TextBox();
            this.num1 = new System.Windows.Forms.NumericUpDown();
            this.num2 = new System.Windows.Forms.NumericUpDown();
            this.label6 = new System.Windows.Forms.Label();
            this.cbChannel = new System.Windows.Forms.ComboBox();
            this.lReading1 = new System.Windows.Forms.LinkLabel();
            this.lReading2 = new System.Windows.Forms.LinkLabel();
            ((System.ComponentModel.ISupportInitialize)(this.num1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.num2)).BeginInit();
            this.SuspendLayout();
            // 
            // btnOK
            // 
            resources.ApplyResources(this.btnOK, "btnOK");
            this.btnOK.Name = "btnOK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // btnCancel
            // 
            resources.ApplyResources(this.btnCancel, "btnCancel");
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            // 
            // cbFunction
            // 
            resources.ApplyResources(this.cbFunction, "cbFunction");
            this.cbFunction.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbFunction.FormattingEnabled = true;
            this.cbFunction.Items.AddRange(new object[] {
            resources.GetString("cbFunction.Items"),
            resources.GetString("cbFunction.Items1")});
            this.cbFunction.Name = "cbFunction";
            this.cbFunction.SelectedIndexChanged += new System.EventHandler(this.cbxChannel_SelectedIndexChanged);
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // label2
            // 
            resources.ApplyResources(this.label2, "label2");
            this.label2.Name = "label2";
            // 
            // tbxReading1
            // 
            resources.ApplyResources(this.tbxReading1, "tbxReading1");
            this.tbxReading1.Name = "tbxReading1";
            this.tbxReading1.Enter += new System.EventHandler(this.tbxReading_Enter);
            this.tbxReading1.Leave += new System.EventHandler(this.tbxReading_Enter);
            // 
            // label4
            // 
            resources.ApplyResources(this.label4, "label4");
            this.label4.Name = "label4";
            // 
            // tbxReading2
            // 
            resources.ApplyResources(this.tbxReading2, "tbxReading2");
            this.tbxReading2.Name = "tbxReading2";
            this.tbxReading2.Enter += new System.EventHandler(this.tbxReading_Enter);
            this.tbxReading2.Leave += new System.EventHandler(this.tbxReading_Leave);
            // 
            // num1
            // 
            resources.ApplyResources(this.num1, "num1");
            this.num1.Name = "num1";
            this.num1.ValueChanged += new System.EventHandler(this.num1_ValueChanged);
            // 
            // num2
            // 
            resources.ApplyResources(this.num2, "num2");
            this.num2.Name = "num2";
            this.num2.ValueChanged += new System.EventHandler(this.num2_ValueChanged);
            // 
            // label6
            // 
            resources.ApplyResources(this.label6, "label6");
            this.label6.Name = "label6";
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
            // lReading1
            // 
            resources.ApplyResources(this.lReading1, "lReading1");
            this.lReading1.Name = "lReading1";
            this.lReading1.TabStop = true;
            this.lReading1.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.lReading1_LinkClicked);
            // 
            // lReading2
            // 
            resources.ApplyResources(this.lReading2, "lReading2");
            this.lReading2.Name = "lReading2";
            this.lReading2.TabStop = true;
            this.lReading2.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.lReading2_LinkClicked);
            // 
            // ADConfig
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.lReading2);
            this.Controls.Add(this.lReading1);
            this.Controls.Add(this.cbChannel);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.num2);
            this.Controls.Add(this.num1);
            this.Controls.Add(this.tbxReading2);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.tbxReading1);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.cbFunction);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Name = "ADConfig";
            this.Load += new System.EventHandler(this.ADConfig_Load);
            ((System.ComponentModel.ISupportInitialize)(this.num1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.num2)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.ComboBox cbFunction;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox tbxReading1;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox tbxReading2;
        private System.Windows.Forms.NumericUpDown num1;
        private System.Windows.Forms.NumericUpDown num2;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.ComboBox cbChannel;
        private System.Windows.Forms.LinkLabel lReading1;
        private System.Windows.Forms.LinkLabel lReading2;
    }
}