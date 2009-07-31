using System;
using System.Windows.Forms;
using Common;

namespace Stub_Experiment
{
    public sealed class Experiment : TwoDevicesExperiment
    {
        private readonly Timer timer = new Timer();
        private TSettings CustomSettings
        {
            get { return Settings as TSettings; }
        } 

        public Experiment()
        {
            Settings = new TSettings();
            timer.Interval = 250;
            timer.Tick += OnDataReceived;
            
            ExperimentCaption = "Заглушка";
        }

        public override void ApplySettingsToChart(IChart chart)
        {
            base.ApplySettingsToChart(chart);
            timer.Interval = CustomSettings.MeasurementPeriod;                      
        }
        public override Type WantedSettingsType
        {
            get
            {
                return typeof (TSettings);
            }
        }

        private int c;
        private void OnDataReceived(object sender, EventArgs e)
        {
            c++;
            double t = c * Settings.MeasurementPeriod/1000.0;
            var p = new Point3D(t, CustomSettings.Y1Transform.Call(t), CustomSettings.Y2Transform.Call(t));
            Chart.AddPoint(p);
            Data.SavePoint(p);
        }

        #region Overrides of TwoDevicesExperiment


        protected override ConnectStatus TryConnect()
        {
            CurrentState = MeasurementState.Ready;
            return ConnectStatus.Successful;
        }

        public override void StartMeasurement()
        {            
            base.StartMeasurement();
            timer.Enabled = true;
        }

        public override void StopMeasurement()
        {
            timer.Enabled = false;
        }

        public override void Disconnect()
        {            
        }

        #endregion
    }
}
