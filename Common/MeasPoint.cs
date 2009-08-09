using System;

namespace Common
{
    public struct MeasPoint
    {
        int start, stop;
        public int Start { get { return start; } }
        public int Stop { get { return stop; } }

        public MeasPoint(int start, int stop)
        {
            this.start = start;
            this.stop = stop;
        }

        public override string ToString()
        {
            return String.Format("{0}\t{1}", Start, Stop);
        }
    }
}
