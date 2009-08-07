using System;

namespace Common
{
    public struct Point3D
    {
        public double X, Y;
        public double? Z;

        public Point3D(double x, double y)
        {
            X = x;
            Y = y;            
            Z = null;
            wasInitialized = true;
        }

        public Point3D(double x, double y, double z):this(x,y)
        {
            Z = z;
        }

        public bool WasInitialized { get { return wasInitialized; } }
        bool wasInitialized;

        public override string ToString()
        {
            return String.Format("{0:F3}\t{1:F3}\t{2:F3}", X, Y, Z);
        }
    }

    public struct MeasPoint
    {
        public int Start, Stop;

        public MeasPoint(int start, int stop)
        {
            Start = start;
            Stop = stop;
        }

        public override string ToString()
        {
            return String.Format("{0}\t{1}", Start, Stop);
        }
    }
}
