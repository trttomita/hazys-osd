using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO.Ports;

namespace OSDConfig
{
    public partial class ADConfig : Form
    {
        public ADConfig()
        {
            InitializeComponent();
            for (int i = 0; i < cbFunction.Items.Count; i++)
                extconfigs.Add(new ADChannelSetting());

            cbFunction.SelectedIndex = 0;
        }

        class ADChannelSetting : ADSetting
        {
            public decimal value1 = 0;
            public ushort read1 = 0;
            public decimal value2 = 0;
            public ushort read2 = 0;

            public ADChannelSetting()
            {
            }

            public ADChannelSetting(ADSetting setting)
            {
                channel = setting.channel;
                k = setting.k;
                b = setting.b;
            }
        }

        List<ADChannelSetting> extconfigs = new List<ADChannelSetting>();

        bool fromuser = true;
        //public List<ADChannelSetting> ChannelConfigs { get; private set; }

        public List<ADSetting> Configs
        {
            get
            {
                List<ADSetting> configs = new List<ADSetting>();
                foreach (var c in extconfigs)
                    configs.Add(c);
                return configs;
            }
            set
            {
                if (value != null)
                {
                    for (int i = 0; i < value.Count; i++)
                        extconfigs[i] = new ADChannelSetting(value[i]);
                }
            }
        }

        public ArduOSDPort Port { get; set; }


        private void btnOK_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
        }

        void Calibrate()
        {
            //var config = ChannelConfigs[cbFunction.SelectedIndex];

            if (tbxReading1.Text != tbxReading2.Text)
            {
                numVperB.Value = (num1.Value - num2.Value) / (int.Parse(tbxReading1.Text) - int.Parse(tbxReading2.Text));
                numVat0.Value = (decimal)num1.Value - numVperB.Value * int.Parse(tbxReading1.Text);
            }
            else
            {
                numVperB.Value = 0;
                numVat0.Value = num1.Value;
            }
        }

        private void cbxChannel_SelectedIndexChanged(object sender, EventArgs e)
        {
            
        }

        private void lReading1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            ushort reading = 0;
            int channel = cbChannel.SelectedIndex;
            int func = cbFunction.SelectedIndex;

            bool ok = Port.GetAnalog(channel, out reading);
            Port.Close();
            if (ok)
            {
                tbxReading1.Text = reading.ToString();
                extconfigs[func].read1 = reading;
                Calibrate();
            }
            else
            {
                MessageBox.Show(this, "Get ADC reading failed", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void lReading2_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            ushort reading = 0;
            int channel = cbChannel.SelectedIndex;
            int func = cbFunction.SelectedIndex;

            bool ok = Port.GetAnalog(channel, out reading);
            Port.Close();

            if (ok)
            {
                tbxReading2.Text = reading.ToString();
                extconfigs[channel].read2 = reading;
                Calibrate();
            }
            else
            {
                MessageBox.Show(this, "Get ADC reading failed", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void num1_ValueChanged(object sender, EventArgs e)
        {
            if (cbFunction.SelectedIndex >= 0 && fromuser)
            {
                extconfigs[cbFunction.SelectedIndex].value1 = num1.Value;
                Calibrate();
            }
        }

        private void num2_ValueChanged(object sender, EventArgs e)
        {
            if (cbFunction.SelectedIndex >= 0 && fromuser)
            {
                extconfigs[cbFunction.SelectedIndex].value2 = num2.Value;
                Calibrate();
            }
        }

        private void cbChannel_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbFunction.SelectedIndex >= 0 && fromuser)
                extconfigs[cbFunction.SelectedIndex].channel = (byte)cbFunction.SelectedIndex;
        }

        private void ADConfig_Load(object sender, EventArgs e)
        {

        }



        private void numVperB_ValueChanged(object sender, EventArgs e)
        {
            if (cbFunction.SelectedIndex >= 0 && fromuser)
            {
                extconfigs[cbFunction.SelectedIndex].k = (float)numVperB.Value;
            }
        }

        private void numVat0_ValueChanged(object sender, EventArgs e)
        {
            if (cbFunction.SelectedIndex >= 0 && fromuser)
            {
                extconfigs[cbFunction.SelectedIndex].b = (float)numVat0.Value;
            }
        }

        private void ADConfig_Shown(object sender, EventArgs e)
        {
            cbxFunction_SelectedIndexChanged(this, new EventArgs());
        }

        private void cbxFunction_SelectedIndexChanged(object sender, EventArgs e)
        {
            fromuser = false;
            int idx = cbFunction.SelectedIndex;
            var config = extconfigs[idx];
            tbxReading1.Text = config.read1.ToString();
            tbxReading2.Text = config.read2.ToString();

            num1.Value = (decimal)config.value1;
            num2.Value = (decimal)config.value2;

            Calibrate();

            cbChannel.SelectedIndex = config.channel;
            fromuser = true;
        }
    }
}
