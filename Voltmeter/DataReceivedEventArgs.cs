using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Threading;
using System.Globalization;
using Visa32;

namespace Tsu.Voltmeters
{
    public class DataReceivedEventArgs : EventArgs
    {
        public double Value { get; private set; }

        public int Channel { get; private set; }

        public DataReceivedEventArgs(double data, int channel)
        {
            this.Value = data;
            this.Channel = channel;
        }
    }
}
