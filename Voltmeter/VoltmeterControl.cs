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
        private int channel;     // номер текущего канала
        private int videfaultRM; // Resource manager session returned by viOpenDefaultRM(videfaultRM)
        private int vi;			 // Session identifier of devices
        private int errorStatus; // VISA function status return code
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
                if (connected)
                    errorStatus = NativeMethods.viClose(vi);

                // Open the Visa session
                errorStatus = NativeMethods.viOpenDefaultRM(out videfaultRM);

                // Open communication to the instrument           
                errorStatus = NativeMethods.viOpen(videfaultRM, DeviceName, 0, 0, out vi);

                // If an error occurs, give a message
                if (errorStatus < NativeMethods.VI_SUCCESS)
                {
                    connected = false;
                    throw new VoltmeterNotFoundException();
                }

                // Set the termination character to carriage return (i.e., 13);
                // the 3458A uses this character
                errorStatus = NativeMethods.viSetAttribute(vi, NativeMethods.VI_ATTR_TERMCHAR, 13);

                // Set the flag to terminate when receiving a termination character
                errorStatus = NativeMethods.viSetAttribute(vi, NativeMethods.VI_ATTR_TERMCHAR_EN, 1);

                // Set timeout in milliseconds; set the timeout for your requirements
                errorStatus = NativeMethods.viSetAttribute(vi, NativeMethods.VI_ATTR_TMO_VALUE, 2000);


                // If an error occurs, give a message
                if (errorStatus < NativeMethods.VI_SUCCESS)
                {
                    connected = false;
                    throw new VoltmeterInvalidSettingsException();                    
                }

                // Check and make sure the correct instrument is addressed
                connected = true;
                deviceDescription = SendCmd("*IDN?"); //GetData(); 

                if (Mode != MeasurementMode.MANUAL)
                    Setup(Mode, AdditionalParams);

                SendCmd("TRIG:DEL 0.01");
            }
            catch (DllNotFoundException ex)
            {
                throw new VoltmeterException("Библиотека visa32.dll не найдена в системном каталоге", ex);
            }
        }

        private void Setup(MeasurementMode mode, string aditionalParams)
        {
            if (!Connected) Connect();
            string smode = mode.ToString().Replace('_', ':');
            SendCmd("CONF:" + smode + " " + aditionalParams);
        }

        public void Setup(IEnumerable<string> initCommands)
        {
            if (!Connected) Connect();
            foreach (string cmd in initCommands)
            {
                SendCmd(cmd);
            }
        }

        /// <summary>
        /// This routine will send a command string to the instrument.
        /// </summary>
        /// <param name="cmd">command string</param>
        /// <returns>result string</returns>
        public string SendCmd(string cmd)
        {
            if (!connected) return null;
            string answer = null;
            try
            {
                // Write the command to the instrument (terminated by a linefeed; vbLf is ASCII character 10)
                errorStatus = NativeMethods.viPrintf(vi, cmd + "\n");

                if (errorStatus < NativeMethods.VI_SUCCESS)
                {
                    throw new VoltmeterException("I/O Error! \nError status: " + errorStatus.ToString());
                }
            }
            catch (VoltmeterException ex)
            {
                throw new VoltmeterException("I/O Error (" + ex.Message + ") in SendCmd method!", ex);                 
            }
            finally
            {
                if (cmd[cmd.Length - 1] == '?')
                    answer = GetData();
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
                errorStatus = NativeMethods.viScanf(vi, "%2048t", msg);

                if (errorStatus < NativeMethods.VI_SUCCESS)
                {
                    throw new VoltmeterException("Reading error! \nError status: " + errorStatus.ToString());
                }
                return msg.ToString();
            }
            catch (VoltmeterException ex)
            {
                throw new VoltmeterException("Reading error ("+ex.Message+") in GetData method!", ex);         
            }
        }
        
        /// <summary>
        /// основная процедура считывания данных по таймеру
        /// </summary>
        /// <param name="state"></param>
        private void ReadProc(object state)
        {
            if (IsDoubleMeasure)
            {
                if (MiddleChannelNumber.HasValue)
                {
                    SendCmd("ROUT:CLOS " + this.MiddleChannelNumber);
                    SendCmd("READ?");
                }
                channel = (channel == FirstChannelNumber) ? SecondChannelNumber : FirstChannelNumber;
                SendCmd("ROUT:CLOS " + channel);
            }
            // SendCmd("INIT");
            double val = Convert.ToDouble(SendCmd("READ?"), new CultureInfo("en-US"));
            lock (values)
            {
                values.Enqueue(val);
            }
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
            read_timer = new Timer(new TimerCallback(ReadProc), null, 0, Interval);
            answer_timer.Interval = Interval;
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
            StopReading();
            if (connected)
            {
                // Close the device session
                errorStatus = NativeMethods.viClose(vi);
                // Close the session
                errorStatus = NativeMethods.viClose(videfaultRM);
            }      
        }

        public override string ToString()
        {
            return DeviceName;
        }

    }
}