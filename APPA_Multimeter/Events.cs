using System;

namespace Tsu.Voltmeters.Appa
{
    public class DataReceivedEventArgs : EventArgs
    {
        public double Value { get; private set; }

        public double Delta { get; private set; }

        public MeasurementMode Mode { get; private set; }

        public DataReceivedEventArgs(double value, double delta, MeasurementMode mode)
        {
            this.Value = value;
            this.Delta = delta;
            this.Mode = mode;
        }
    }
}
