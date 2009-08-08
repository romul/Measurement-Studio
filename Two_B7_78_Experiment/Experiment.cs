using System;
using Common;
using Tsu.Voltmeters;
using System.Windows.Forms;

namespace Two_B7_78_Experiment
{
    [CLSCompliant(true)]
    public class Experiment : AbstractExperiment
    {
        private readonly VoltmeterControl B7_78_1, B7_78_2;
        private int count;
        private Point3D point = new Point3D();

        private TSettings CustomSettings
        {
            get { return Settings as TSettings; }
        }

        public Experiment()
        {
            B7_78_1 = new VoltmeterControl();
            B7_78_1.DataReceived += OnDataReceived;
            B7_78_2 = new VoltmeterControl();
            B7_78_2.DataReceived += OnDataReceived;
            Settings = new TSettings();
            ExperimentCaption = "Два B7 78/1";
        }

        private void OnDataReceived(object sender, DataReceivedEventArgs args)
        {
            if (!point.WasInitialized)
            {
                var y = CustomSettings.Y1Transform.Call(args.Value);
                point = new Point3D(count * 1e-3 * Settings.MeasurementPeriod, y);
            }
            else
            {
                var z = CustomSettings.Y2Transform.Call(args.Value);
                point.Z = z;
                Chart.AddPoint(point);
                Data.SavePoint(point);
                count++;
                point = new Point3D();
            }
        }

        #region Overrides of AbstractExperiment

        protected override ConnectStatus TryConnect()
        {
            B7_78_1.Interval = CustomSettings.MeasurementPeriod;
            B7_78_1.IsDoubleMeasure = false;
            B7_78_1.DeviceName = String.Format("USB0::0x164E::0x0DAD::TW0000{0}::INSTR", CustomSettings.FirstDeviceId);
            B7_78_2.Interval = CustomSettings.MeasurementPeriod;
            B7_78_2.IsDoubleMeasure = false;
            B7_78_2.DeviceName = String.Format("USB0::0x164E::0x0DAD::TW0000{0}::INSTR", CustomSettings.SecondDeviceId);
            try
            {
                B7_78_1.Setup(CustomSettings.FirstInitCommands.Split(new[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries));
                B7_78_2.Setup(CustomSettings.SecondInitCommands.Split(new[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries));
            }
            catch (VoltmeterException ex)
            {
                ErrorLogProvider.WriteToEventLogAndShow(ex.Message);
            }

            if (B7_78_1.Connected && B7_78_2.Connected)
            {
                CurrentState = MeasurementState.Connected;
                return ConnectStatus.Successful;
            }
            else if (B7_78_1.Connected || B7_78_2.Connected)
            {
                return ConnectStatus.OneDeviceNotConnected;
            }
            return ConnectStatus.AllDevicesNotConnected;
        }

        public override void StartMeasurement()
        {
            base.StartMeasurement();
            B7_78_1.StartReading();
            B7_78_2.StartReading();
        }

        public override void StopMeasurement()
        {
            B7_78_1.StopReading();
            B7_78_2.StopReading();
        }

        public override void Disconnect()
        {
            B7_78_1.Disconnect();
            B7_78_2.Disconnect();
            if (!B7_78_1.Connected || !B7_78_2.Connected)
            {
                CurrentState = MeasurementState.Disconnected;
            }
        }

        #endregion
    }
}
