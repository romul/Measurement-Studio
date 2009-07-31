using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO.Ports;

namespace TSU.Voltmeters.APPA
{
    public partial class Multimeter109nControl : Component
    {
        /// <summary>
        /// COM-порт, который эмулируется поверх USB
        /// </summary>
        private SerialPort com2usb = new SerialPort("COM3");

        /// <summary>
        /// таймер для автоматического считывания данных с мультиметра с заданной периодичностью
        /// </summary>
        private Timer readTimer = new Timer();

        [Description("Максимально допустимая задержка при чтении данных с порта")]
        [DefaultValue(500)]
        [Category("Embedded Details")]
        public int ReadTimeout
        {
            get { return com2usb.ReadTimeout; }
            set { com2usb.ReadTimeout = value; }
        }

        [Description("Максимально допустимая задержка при записи в порт")]
        [DefaultValue(500)]
        [Category("Embedded Details")]
        public int WriteTimeout
        {
            get { return com2usb.WriteTimeout; }
            set { com2usb.WriteTimeout = value; }
        }

        [Description("Единицы измерения")]
        [Category("Embedded Details")]
        public MeasurementMode Mode { get; private set; }

        [Description("Количество измерений в секунду (2 или 20)")]
        [Category("Embedded Details")]
        public MeasurementVelocity MpS {
            get { return mps; }
            private set
            {
                mps = value;
                readTimer.Interval = 1000 / (int)mps;
            }
        }
        private MeasurementVelocity mps;

        [Description("Событие, возникающее после снятия показания с мультиметра")]
        [Category("Embedded Event")]
        [DisplayName("OnDataReceived")]
        public event DataReceivedEventHandler DataReceived;


        public Multimeter109nControl()
        {
            InitializeComponent();
            com2usb.ReadTimeout = 500;
            com2usb.WriteTimeout = 500;
            MpS = MeasurementVelocity.DigitalScale;
            readTimer.Tick += OnReadTimerTick;
        }

        /// <summary>
        /// Подключиться к мультиметру
        /// </summary>
        public void Connect()
        {
            if (!com2usb.IsOpen) com2usb.Open();
        }

        /// <summary>
        /// Отключиться от мультиметра
        /// </summary>
        public void Disconnect()
        {
            if (com2usb.IsOpen) com2usb.Close();
        }


        private void OnReadTimerTick(object sender, EventArgs e)
        {
            ReadValue();
        }

        /// <summary>
        /// Читает текущее значение с мультиметра
        /// </summary>
        public DataReceivedEventArgs ReadValue()
        {
            byte[] buff = {0x55, 0x55, 0, 0, 0xAA}; 
            byte[] out_buff = new byte[19];
            //com2usb.ReceivedBytesThreshold = 19;
            if (!com2usb.IsOpen) com2usb.Open();
            if (com2usb.IsOpen)
            {
                com2usb.Write(buff, 0, 5);
                for (int i = 0; i < out_buff.Length; i++)
                {
                    com2usb.Read(out_buff, i, 1);
                }
            }
            return ParseValue(out_buff);
        }

        /// <summary>
        /// Интерпретирует байты считанные с порта
        /// </summary>
        /// <param name="buffer">массив считанных байт</param>
        private DataReceivedEventArgs ParseValue(byte[] buffer)
        {
            int val = (((buffer[10] << 8) + buffer[9]) << 8) + buffer[8];
            if (val > 0x800000) val -= 0x1000000;
            double res;
            double delta;

            switch (buffer[11])
            {
                case 0x0B: // -20..20 V
                    res = val*1e-3;
                    delta = 1e-3*Math.Ceiling(Math.Abs(val)*6e-4 + 10);
                    this.Mode = MeasurementMode.V;
                    break;
                case 0x0C: // -2..2 V
                    res = val*1e-4;
                    delta = 1e-4*Math.Ceiling(Math.Abs(val)*6e-4 + 10);
                    this.Mode = MeasurementMode.V;
                    break;
                case 0x12: // -200..200 mV
                    res = val*1e-2;
                    delta = 1e-2*Math.Ceiling(Math.Abs(val)*6e-4 + 20);
                    this.Mode = MeasurementMode.mV;
                    break;
                case 0x13: // -20..20 mV
                    res = val*1e-3;
                    delta = 1e-3*Math.Ceiling(Math.Abs(val)*6e-4 + 60);
                    this.Mode = MeasurementMode.mV;
                    break;
                default:
                    throw new NotImplementedException(
                        "Режимы, отличные от DC с пределами = 20 mV, 200 mV, 2 V, 20 V, не поддерживаются");
            }
            this.OnDataReceived(res, delta, this.Mode);
            return new DataReceivedEventArgs(res, delta, this.Mode);
        }

        /// <summary>
        /// Приступить к снятию показаний
        /// </summary>
        public void StartReading()
        {
            readTimer.Enabled = true;
        }

        /// <summary>
        /// Завершить снятие показаний
        /// </summary>
        public void StopReading()
        {
            readTimer.Enabled = false;
        }

        /// <summary>
        /// Сгенерировать событие получения данных с мультиметра
        /// </summary>
        /// <param name="value">измеренное значение</param>
        /// <param name="delta">погрешность</param>
        /// <param name="mode">единицы измерения</param>
        protected virtual void OnDataReceived(double value, double delta, MeasurementMode mode)
        {
            DataReceivedEventHandler localHandler = DataReceived;
            if (localHandler != null)
            {
                localHandler(this, new DataReceivedEventArgs(value, delta, mode));
            }
        }


    }
}
