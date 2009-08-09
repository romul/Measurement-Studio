using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Common;

namespace MeasurementStudio
{
    [Serializable]
    public class InvalidExperimentAssemblyException : Exception
    {
        public InvalidExperimentAssemblyException() : base() { }
        public InvalidExperimentAssemblyException(string msg) : base(msg) { }
        public InvalidExperimentAssemblyException(string msg, Exception innerException) : base(msg, innerException) { }
        protected InvalidExperimentAssemblyException(System.Runtime.Serialization.SerializationInfo si, System.Runtime.Serialization.StreamingContext sc) : base(si, sc) { }
    }
}
