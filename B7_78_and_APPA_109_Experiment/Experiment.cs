using System;
using Common;
using Tsu.Voltmeters;
using System.Windows.Forms;

namespace Two_B7_78_Experiment
{
    class Experiment : AbstractExperiment
    {
        private readonly VoltmeterControl B7_78;
        private readonly Tsu.Voltmeters.Appa.Multimeter109NControl appa109N;
        private int count;
        private Point3D point;

        private TSettings CustomSettings
        {
            get { return Settings as TSettings; }
        }

        public Experiment()
        {
            B7_78 = new VoltmeterControl();
            B7_78.DataReceived += OnDataReceived;
            appa109N = new Tsu.Voltmeters.Appa.Multimeter109NControl();
            Settings = new TSettings();
            ExperimentCaption = "B7 78/1 + APPA 109N";
        }

        private void OnDataReceived(object sender, DataReceivedEventArgs args)
        {
            var y = CustomSettings.Y1Transform.Call(args.Value);
            point = new Point3D(count * 1e-3 * Settings.MeasurementPeriod, y);
            Tsu.Voltmeters.Appa.DataReceivedEventArgs res = appa109N.ReadValue();
            var z = CustomSettings.Y2Transform.Call(res.Value);
            point.Z = z;
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
            appa109N.Connect();
            try
            {
                B7_78.Setup(CustomSettings.InitCommands.Split(new[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries));                
            }
            catch (VoltmeterException ex)
            {
                MessageBox.Show(ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Stop);
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
            appa109N.Disconnect();
            if (!B7_78.Connected)
            {
                CurrentState = MeasurementState.Disconnected;
            }
        }

        #endregion
    }
}
