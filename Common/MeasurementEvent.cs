
namespace Common
{
    public enum MeasurementEvents { Start, Stop }

    public class MeasurementEventArgs
    {
        public MeasurementEventArgs(MeasurementEvents mEvent)
        {
            EventType = mEvent;
        }
        public MeasurementEvents EventType { get; private set; }
    }

    internal delegate void MeasurementEventHandler(object sender, MeasurementEventArgs e);
}
