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
            ChannelConfigs = new List<ADChannelConfig>();
            for (int i = 0; i < cbxChannel.Items.Count; i++)
                ChannelConfigs.Add(new ADChannelConfig());
        }

        public class ADChannelConfig
        {
            public double Value1;
            public int Read1;
            public double Value2;
            public int Read2;
        }

        public List<ADChannelConfig> ChannelConfigs { get; private set; }

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
            int reading = 0;
            int channel = cbxChannel.SelectedIndex;
            if (activeBox != null && Port.GetAnalog(channel + 1, out reading))
            {
                activeBox.Text = reading.ToString();
                if (activeBox == tbxMax)
                    (ChannelConfigs[channel]).Read1 = reading;
                else if (activeBox == tbxMin)
                    ChannelConfigs[channel].Read2 = reading;
            }
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
        }

        private void ADConfig_Load(object sender, EventArgs e)
        {
            
        }

        private void cbxChannel_SelectedIndexChanged(object sender, EventArgs e)
        {
            int idx = cbxChannel.SelectedIndex;
            numMax.Value = (decimal)ChannelConfigs[idx].Value1;
            tbxMax.Text = ChannelConfigs[idx].Read1.ToString();
            numMin.Value = (decimal)ChannelConfigs[idx].Value2;
            tbxMin.Text = ChannelConfigs[idx].Read2.ToString();
        }
    }
}
