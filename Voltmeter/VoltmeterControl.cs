using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Threading;
using System.Globalization;
using Visa32;

namespace Tsu.Voltmeters
{
    [ToolboxBitmap(typeof(VoltmeterControl), "UsbHidBmp.bmp")]
    [CLSCompliant(true)]
    public partial class VoltmeterControl : Component
    {
        System.Windows.Forms.Timer answer_timer;
        public VoltmeterControl()
        {
            InitializeComponent();
            answer_timer = new System.Windows.Forms.Timer();
            answer_timer.Tick += new EventHandler(OnAnswerTimerTick);
            channel = this.FirstChannelNumber;
        }
        private int channel; // номер текущего канала
        private int videfaultRM;	// Resource manager session returned by viOpenDefaultRM(videfaultRM)
        private int vi;			    // Session identifier of devices
        private int errorStatus;		 // VISA function status return code
        private bool connected;  // Used to determine if there is connection with the instrument

        private Queue<double> values = new Queue<double>(1024);
        private Timer read_timer;
       

        [Description("The event that occurs when data is received from the embedded system")]
        [Category("Embedded Event")]
        [DisplayName("OnDataReceived")]
        public event EventHandler<DataReceivedEventArgs> DataReceived;

        public string DeviceDescription
        {
            get { return deviceDescription; }
        }
        private string deviceDescription;
        
        [Description("Специфицированное имя устройства с указанием порта")]
        [DefaultValue("USB0::0x164E::0x0DAD::TW00002992::INSTR")]
        [Category("Embedded Details")]
        public string DeviceName
        {
            get { return deviceName; }
            set { deviceName = value; }
        }
        private string deviceName = "USB0::0x164E::0x0DAD::TW00002992::INSTR";

        [Description("Признак того, что в данный момент проводятся измерения")]
        [DefaultValue("USB0::0x164E::0x0DAD::TW00002992::INSTR")]
        [Category("Double Measurement Details")]
        public bool IsDoubleMeasure
        {
            get { return isMeasure; }
            set { isMeasure = value; }
        }
        private bool isMeasure;

        [Description("Интервал опроса устройства в мс.")]
        [DefaultValue(500)]
        [Category("Embedded Details")]
        public int Interval
        {
            get { return interval; }
            set { 
                interval = value;
                if (read_timer != null)
                {
                    this.StopReading();
                    this.StartReading();
                }
            }
        }
        private int interval = 500;

        public int TimeOfFieldOn
        {
            get { return timeOfFieldOn; }
            set
            {
                timeOfFieldOn = (int)Math.Round(value / 500.0) * 500;
            }
        }
        private int timeOfFieldOn = 2000;

        [Description("Режим работы")]
        [DefaultValue(MeasurementMode.MANUAL)]
        [Category("Measurement Details")]
        public MeasurementMode Mode
        {
            get { return mode; }
            set { mode = value; }
        }
        private MeasurementMode mode;

        [Description("Дополнительные параметры (указываются в команде установки режима), как правило два числа через пробел")]
        [DefaultValue("(none)")]
        [Category("Measurement Details")]
        public string AdditionalParams
        {
            get { return additional_params; }
            set { additional_params = value; }
        }
        private string additional_params;

        public bool Connected
        {
            get { return connected; }
        }

        [Description("Номер первого измерительного канала")]
        [DefaultValue(1)]
        [Category("Double Measurement Details")]
        public int FirstChannelNumber
        {
            get { return firstChannelNumber; }
            set
            {
                firstChannelNumber = value % 16;
            }
        }
        private int firstChannelNumber = 1;

        [Description("Номер второго измерительного канала")]
        [DefaultValue(2)]
        [Category("Double Measurement Details")]
        public int SecondChannelNumber
        {
            get { return secondChannelNumber; }
            set
            {
                secondChannelNumber = value % 16;
            }
        }
        private int secondChannelNumber = 2;

        [Description("Номер промежуточного канала")]
        [DefaultValue(null)]
        [Category("Double Measurement Details")]
        public int? MiddleChannelNumber
        {
            get { return middleChannelNumber; }
            set
            {
                middleChannelNumber = value;
            }
        }
        private int? middleChannelNumber = null;

        public void Connect()
        {
            try
            {
                // If port is open, close it
                if (this.connected)
                    this.errorStatus = NativeMethods.viClose(this.vi);

                // Open the Visa session
                this.errorStatus = NativeMethods.viOpenDefaultRM(out this.videfaultRM);

                // Open communication to the instrument                (addrType.Text).ToUpper() + "::INSTR"
                this.errorStatus = NativeMethods.viOpen(this.videfaultRM, DeviceName, 0, 0, out this.vi);

                // If an error occurs, give a message
                if (this.errorStatus < NativeMethods.VI_SUCCESS)
                {
                    this.connected = false;
                    throw new VoltmeterException("Unable to open device port; check address");
                }

                // Set the termination character to carriage return (i.e., 13);
                // the 3458A uses this character
                this.errorStatus = NativeMethods.viSetAttribute(this.vi, NativeMethods.VI_ATTR_TERMCHAR, 13);

                // Set the flag to terminate when receiving a termination character
                this.errorStatus = NativeMethods.viSetAttribute(this.vi, NativeMethods.VI_ATTR_TERMCHAR_EN, 1);

                // Set timeout in milliseconds; set the timeout for your requirements
                this.errorStatus = NativeMethods.viSetAttribute(this.vi, NativeMethods.VI_ATTR_TMO_VALUE, 2000);

                // Check and make sure the correct instrument is addressed
                this.connected = true;
                this.deviceDescription = SendCmd("*IDN?"); //GetData(); 

                if (Mode != MeasurementMode.MANUAL)
                    this.Setup(Mode, AdditionalParams);

                this.SendCmd("TRIG:DEL 0.01");
            }
            catch (DllNotFoundException ex)
            {
                throw new VoltmeterException("Библиотека visa32.dll не найдена в системном каталоге", ex);
            }
        }

        private void Setup(MeasurementMode mode, string aditionalParams)
        {
            if (!this.Connected) this.Connect();
            //System.Windows.Forms.MessageBox.Show(
            string smode = mode.ToString().Replace('_', ':');
            this.SendCmd("CONF:" + smode + " " + aditionalParams);
        }

        public void Setup(IEnumerable<string> initCommands)
        {
            if (!this.Connected) this.Connect();
            foreach (string cmd in initCommands)
            {
                this.SendCmd(cmd);
            }
        }

        public string SendCmd(string cmd)
        //"""""""""""""""""""""""""""""""""""""""""""""""""""""""""""""""""""""""""""""""""""""
        // This routine will send a command string to the instrument.
        //"""""""""""""""""""""""""""""""""""""""""""""""""""""""""""""""""""""""""""""""""""""
        {
            if (!this.connected) return null;
            string answer = null;
            try
            {
                // Write the command to the instrument (terminated by a linefeed; vbLf is ASCII character 10)
                this.errorStatus = NativeMethods.viPrintf(this.vi, cmd + "\n");

                if (this.errorStatus < NativeMethods.VI_SUCCESS)
                {
                    throw new VoltmeterException("I/O Error! \nError status: " + this.errorStatus.ToString());
                }
            }
            catch (Exception e)
            {
                throw new VoltmeterException("I/O Error (" + e.Message + ") in SendCmd method!", e);                 
            }
            finally
            {
                if (cmd[cmd.Length - 1] == '?')
                    answer = this.GetData();
            }
            return answer;
        }
        /// <summary>
        /// This function reads the string returned by the device
        /// </summary>
        /// <returns>string</returns>
        private string GetData()
        {
            System.Text.StringBuilder msg = new System.Text.StringBuilder(2048);
            try
            {
                // Return the reading
                this.errorStatus = NativeMethods.viScanf(this.vi, "%2048t", msg);

                if (this.errorStatus < NativeMethods.VI_SUCCESS)
                {
                    throw new VoltmeterException("Reading error! \nError status: " + this.errorStatus.ToString());
                }
                return msg.ToString();
            }
            catch (Exception e)
            {
                throw new VoltmeterException("Reading error ("+e.Message+") in GetData method!", e);         
            }
        }
        
        /// <summary>
        /// основная процедура считывания данных по таймеру
        /// </summary>
        /// <param name="state"></param>
        private void ReadProc(object state)
        {
            try
            {              
                if (this.IsDoubleMeasure)
                {                    
                    if (this.MiddleChannelNumber.HasValue)
                    {
                        this.SendCmd("ROUT:CLOS " + this.MiddleChannelNumber);
                        this.SendCmd("READ?");
                    }
                    channel = (channel == this.FirstChannelNumber) ? this.SecondChannelNumber : this.FirstChannelNumber;
                    this.SendCmd("ROUT:CLOS " + channel);
                }
               // SendCmd("INIT");
                double val = Convert.ToDouble(this.SendCmd("READ?"), new CultureInfo("en-US"));
                lock (values)
                {
                    values.Enqueue(val);
                }
            }
            catch
            {
            }
            /*lock (this)
            {
                this.OnDataReceived();
            }*/
        }
        protected virtual void OnDataReceived()
        {
            EventHandler<DataReceivedEventArgs> localHandler = DataReceived;
            double? val = TakeValueOut();
            if ((localHandler != null)&&(val != null))
            {
                localHandler(this, new DataReceivedEventArgs((double)val, channel));
            }
        }

        protected double? TakeValueOut()
        {
            double? res = null;
            lock (values)
            {
                if (values.Count > 0)
                    res = values.Dequeue();
            }
            return res;
        }

        public void StartReading()
        {
            read_timer = new Timer(new TimerCallback(this.ReadProc), null, 0, this.Interval);
            answer_timer.Interval = this.Interval;
            answer_timer.Enabled = true;
        }

        private void OnAnswerTimerTick(object sender, EventArgs e)
        {
            EventHandler<DataReceivedEventArgs> localHandler = DataReceived;
            double? val = TakeValueOut();
            if (localHandler != null)
            {
                while (val != null)
                {
                    localHandler(this, new DataReceivedEventArgs((double)val, channel));
                    val = TakeValueOut();
                }
            }
        }

        public void StopReading()
        {
            answer_timer.Enabled = false;
            read_timer.Dispose();
            read_timer = null;
        }

        public void Disconnect()
        {
            if (this.connected)
            {
                // Close the device session
                this.errorStatus = NativeMethods.viClose(this.vi);

                // Close the session
                this.errorStatus = NativeMethods.viClose(this.videfaultRM);
            }      
        }

        public override string ToString()
        {
            return this.DeviceName;
        }

    }

    public enum MeasurementMode
    {
        MANUAL, VOLT_DC, VOLT_AC, RESISTENCE, FRESISTANCE, PERIOD, CONTINUITY, DIODE, TCOUPLE, TEMPERATURE
    }
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
    //public delegate void DataReceivedEventHandler(object sender, DataReceivedEventArgs args);

    public class VoltmeterException : Exception
    {
        public VoltmeterException() : base() { }
        public VoltmeterException(string msg) : base(msg) { }
        public VoltmeterException(string msg, Exception innerException) : base(msg, innerException) { }
        protected VoltmeterException(System.Runtime.Serialization.SerializationInfo si, System.Runtime.Serialization.StreamingContext sc) : base(si, sc) { }
    }


}