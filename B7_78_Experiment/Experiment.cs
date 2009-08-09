using System;
using Common;
using Tsu.Voltmeters;
using System.Windows.Forms;

namespace B7_78_Experiment
{
    [CLSCompliant(true)]
    public sealed class Experiment : AbstractExperiment
    {
        private readonly VoltmeterControl B7_78;
        private int count;
        private TSettings CustomSettings
        {
            get { return Settings as TSettings; }
        } 

        public Experiment()
        {
            B7_78 = new VoltmeterControl();
            B7_78.DataReceived += OnDataReceived;
            Settings = new TSettings();
            ExperimentCaption = "B7 78/1";
        }

        public override void ApplySettingsToChart(IChart chart)
        {
            base.ApplySettingsToChart(chart);
        }

        private void OnDataReceived(object sender, DataReceivedEventArgs args)
        {
            var y = CustomSettings.Y1Transform.Call(args.Value);
            var point = new Point3D(count * 1e-3 * Settings.MeasurementPeriod, y);
            Chart.AddPoint(point);
            Data.SavePoint(point);
            count++;
        }




        #region Overrides of AbstractExperiment


        protected override ConnectStatus TryConnect()
        {
            B7_78.Interval = CustomSettings.MeasurementPeriod;
            B7_78.IsDoubleMeasure = false;
            B7_78.DeviceName = String.Format("USB0::0x164E::0x0DAD::TW0000{0}::INSTR", CustomSettings.DeviceId);
            try
            {
                B7_78.Setup(CustomSettings.InitCommands.Split(new[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries)); 
            }
            catch (VoltmeterException ex)
            {
                ErrorLogProvider.WriteToEventLogAndShow(ex.Message);                
            }
            
            if (B7_78.Connected)
            {
                CurrentState = MeasurementState.Connected;
                return ConnectStatus.Successful;
            }
            return ConnectStatus.AllDevicesNotConnected;
        }


        public override void StartMeasurement()
        {
            base.StartMeasurement();
            B7_78.StartReading();
        }

        public override void StopMeasurement()
        {
            B7_78.StopReading();
        }

        public override void Disconnect()
        {
            B7_78.Disconnect();
            if (!B7_78.Connected) CurrentState = MeasurementState.Disconnected;            
        }

        #endregion

        #region IDisposable Members

        public sealed override void Dispose()
        {
            B7_78.Dispose();
        }

        #endregion
    }
}
