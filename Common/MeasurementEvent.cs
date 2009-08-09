using System;

namespace Common
{
    public enum MeasurementEvents { Start, Stop }

    public class MeasurementEventArgs : EventArgs
    {
        public MeasurementEventArgs(MeasurementEvents mEvent)
        {
            EventType = mEvent;
        }
        public MeasurementEvents EventType { get; private set; }
    }
}
