using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Threading;
using System.Globalization;
using Visa32;

namespace Tsu.Voltmeters
{
    [Serializable]
    public class VoltmeterException : Exception
    {
        public VoltmeterException() : base() { }
        public VoltmeterException(string msg) : base(msg) { }
        public VoltmeterException(string msg, Exception innerException) : base(msg, innerException) { }
        protected VoltmeterException(System.Runtime.Serialization.SerializationInfo si, System.Runtime.Serialization.StreamingContext sc) : base(si, sc) { }
    }

    [Serializable]
    public class VoltmeterNotFoundException : VoltmeterException
    {
        public VoltmeterNotFoundException() : base("Вольтметр B7 78/1 не найден\nПроверьте указанный номер порта.") { }
        public VoltmeterNotFoundException(string msg) : base(msg) { }
        public VoltmeterNotFoundException(string msg, Exception innerException) : base(msg, innerException) { }
        protected VoltmeterNotFoundException(System.Runtime.Serialization.SerializationInfo si, System.Runtime.Serialization.StreamingContext sc) : base(si, sc) { }
    }

    [Serializable]
    public class VoltmeterInvalidSettingsException : VoltmeterException
    {
        public VoltmeterInvalidSettingsException() : base("Ошибка настройки вольтметра B7 78/1.") { }
        public VoltmeterInvalidSettingsException(string msg) : base(msg) { }
        public VoltmeterInvalidSettingsException(string msg, Exception innerException) : base(msg, innerException) { }
        protected VoltmeterInvalidSettingsException(System.Runtime.Serialization.SerializationInfo si, System.Runtime.Serialization.StreamingContext sc) : base(si, sc) { }
    }
}
