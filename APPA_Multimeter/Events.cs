using System;

namespace TSU.Voltmeters.APPA
{
    public class DataReceivedEventArgs : EventArgs
    {
        public readonly double Value;

        public readonly double Delta;

        public readonly MeasurementMode Mode;

        public DataReceivedEventArgs(double value, double delta, MeasurementMode mode)
        {
            this.Value = value;
            this.Delta = delta;
            this.Mode = mode;
        }
    }

    public delegate void DataReceivedEventHandler(object sender, DataReceivedEventArgs args);
}
