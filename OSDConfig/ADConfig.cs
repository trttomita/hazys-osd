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
            ChannelConfigs = new List<ADChannelSetting>();
            for (int i = 0; i < cbFunction.Items.Count; i++)
                ChannelConfigs.Add(new ADChannelSetting());
        }

        public class ADChannelSetting : ADSetting
        {
            public decimal value1;
            public ushort read1;
            public decimal value2;
            public ushort read2;
        }

        public List<ADChannelSetting> ChannelConfigs { get; private set; }

        public ArduOSDPort Port { get; set; }

        TextBox activeBox;

        private void tbxReading_Enter(object sender, EventArgs e)
        {
            activeBox = (TextBox)sender;
        }

        private void tbxReading_Leave(object sender, EventArgs e)
        {

        }

        private void btnPull_Click(object sender, EventArgs e)
        {
            ushort reading = 0;
            int channel = cbChannel.SelectedIndex;

            if (activeBox != null && Port.GetAnalog(channel, out reading))
            {
                activeBox.Text = reading.ToString();
                if (activeBox == tbxReading1)
                    (ChannelConfigs[cbFunction.SelectedIndex]).read1 = reading;
                else if (activeBox == tbxReading2)
                    ChannelConfigs[cbFunction.SelectedIndex].read2 = reading;
            }
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < ChannelConfigs.Count; i++)
            {
                var config = ChannelConfigs[i];

                if (config.read1 != config.read2)
                {
                    config.k = (float)(config.value1 - config.value2) / (config.read1 - config.read2);
                    config.b = (float)config.value1 - config.k * config.read1;
                }
                else
                {
                    config.k = 0;
                    config.b = config.read1;
                }
            }
            DialogResult = DialogResult.OK;
        }

        private void cbxChannel_SelectedIndexChanged(object sender, EventArgs e)
        {
            int idx = cbFunction.SelectedIndex;
            var config = ChannelConfigs[idx];
            num1.Value = (decimal)config.value1;
            tbxReading1.Text = config.read1.ToString();

            num2.Value = (decimal)config.value2;
            tbxReading2.Text = config.read2.ToString();
            cbChannel.SelectedIndex = config.channel;
        }

        private void lReading1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            ushort reading = 0;
            int channel = cbChannel.SelectedIndex;
            int func = cbFunction.SelectedIndex;

            if (Port.GetAnalog(channel, out reading))
            {
                tbxReading1.Text = reading.ToString();
                ChannelConfigs[func].read1 = reading;
            }
        }

        private void lReading2_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            ushort reading = 0;
            int channel = cbChannel.SelectedIndex;
            int func = cbFunction.SelectedIndex;

            if (Port.GetAnalog(channel, out reading))
            {
                tbxReading2.Text = reading.ToString();
                ChannelConfigs[channel].read2 = reading;
            }
        }

        private void num1_ValueChanged(object sender, EventArgs e)
        {
            if (cbFunction.SelectedIndex >= 0)
            {
                ChannelConfigs[cbFunction.SelectedIndex].value1 = num1.Value;
            }
        }

        private void num2_ValueChanged(object sender, EventArgs e)
        {
            if (cbFunction.SelectedIndex >= 0)
                ChannelConfigs[cbFunction.SelectedIndex].value2 = num2.Value;
        }

        private void cbChannel_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbFunction.SelectedIndex >= 0)
                ChannelConfigs[cbFunction.SelectedIndex].channel = (byte)cbFunction.SelectedIndex;
        }

        private void ADConfig_Load(object sender, EventArgs e)
        {

        }
    }
}
