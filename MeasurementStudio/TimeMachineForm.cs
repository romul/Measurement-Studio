using System;
using System.Windows.Forms;
using Common;

namespace MeasurementStudio
{
    public partial class TimeMachineForm : Form
    {
        public TimeMachineForm()
        {
            InitializeComponent();
        }

        private IChart chart;
        private RawData data;
        private int num;

        private void TimeMachineForm_Load(object sender, EventArgs e)
        {
            chart = new ZedGraphProxy(zedGraphControl1);
            AppSettings.Mode.ApplySettingsToChart(chart);
            data = AppSettings.Mode.Data;
        }

        private void tsbStart_Click(object sender, EventArgs e)
        {
            timer.Interval = data.TimeInterval;
            timer.Enabled = true;
        }

        private void timer_Tick(object sender, EventArgs e)
        {
            if (num >= data.Count)
            {
                chart.Clear();
                num = 0;
            }
            chart.AddPoint(data[num]);
            num++;
            tsProgressBar.Value = 100*num/data.Count;
        }

        private void tsbStop_Click(object sender, EventArgs e)
        {
            timer.Enabled = false;
        }

        private void tsbMakeFaster_Click(object sender, EventArgs e)
        {
            timer.Interval /= 2;
            timer.Interval = (int)Math.Ceiling(timer.Interval * 64.0 / 1000) * 1000 / 64;
            float speed = (float)data.TimeInterval / timer.Interval;
            tslCurrentSpeed.Text = Math.Round(speed, 3) + "x";
        }

        private void tsbMakeSlower_Click(object sender, EventArgs e)
        {
            timer.Interval *= 2;
            timer.Interval = (int)Math.Ceiling(timer.Interval * 64.0 / 1000) * 1000 / 64;
            float speed = (float)data.TimeInterval / timer.Interval;
            tslCurrentSpeed.Text = Math.Round(speed, 3) + "x";
        }


    }
}
