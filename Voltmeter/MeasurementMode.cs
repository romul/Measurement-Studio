using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Threading;
using System.Globalization;
using Visa32;

namespace Tsu.Voltmeters
{
    public enum MeasurementMode
    {
        MANUAL, VOLT_DC, VOLT_AC, RESISTENCE, FRESISTANCE, PERIOD, CONTINUITY, DIODE, TCOUPLE, TEMPERATURE
    }
}
