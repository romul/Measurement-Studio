using System;
using System.Collections.Generic;
using System.Xml.Linq;

namespace Common
{
    [Serializable]
    public class MeasurementException : Exception
    {
        public MeasurementException() : base() { }
        public MeasurementException(string msg) : base(msg) { }
        public MeasurementException(string msg, Exception innerException) : base(msg, innerException) { }
        protected MeasurementException(System.Runtime.Serialization.SerializationInfo si, System.Runtime.Serialization.StreamingContext sc) : base(si, sc) { }
    }
}
