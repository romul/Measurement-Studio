using System;
using System.Collections.Generic;
using System.Xml.Linq;

namespace Common
{
    public abstract class AbstractExperiment //: IExperiment
    {
        /// <summary>
        /// Настройки, соответсвующие эксперименту
        /// </summary>
        protected AbstractSettings Settings { get; set; }
        public bool ShouldBeSaved { get { return Settings.SaveMode != SaveMode.DontSave; } }

        protected abstract ConnectStatus TryConnect();

        internal event MeasurementEventHandler MeasurementEvent;

        public void RaiseMeasurementEvent(MeasurementEventArgs e)
        {
            MeasurementEventHandler handler = MeasurementEvent;
            // Event will be null if there are no subscribers
            if (handler != null)
            {
                handler(this, e);
            }
        }

        /// <summary>
        /// Краткое описание конфигурации устройств, взаимодействие с которыми описывает данный класс
        /// </summary>
        protected string ExperimentCaption { get; set; }

        /// <summary>
        /// Загружены ли настройки
        /// </summary>
        public bool IsSettingsLoaded { get; private set; }

        /// <summary>
        /// С помощью пользователя создаёт новый набор настроек на основе значений по умолчанию
        /// </summary>
        public void SetSettingsByUser()
        {
            if (Settings != null) Settings.CreateForm(null);
        }

        /// <summary>
        /// Редактирует файл настреок с помощью пользователя
        /// </summary>
        /// <param name="fileName">полный путь к файлу настроек</param>
        public void EditSettingsByUser(string fileName)
        {
            LoadSettings(fileName);
            if (Settings != null) Settings.CreateForm(fileName);
        }

        public void InitChart(IChart chart)
        {            
            ApplySettingsToChart(chart);
            Chart = chart;                        
        }
        /// <summary>
        /// Инициализирует компонент для построения графиков настройками
        /// </summary>
        /// <param name="chart">интерфейсная ссылка на объект компонента для построения графиков</param>
        public virtual void ApplySettingsToChart(IChart chart)
        {
            chart.SetSeriesNames(Settings.Series1Legend);
            chart.AutoScroll = Settings.AutoScroll;
            chart.LegendVisible = Settings.LegendVisible;
            chart.DotsPerFrame = Settings.DotsPerFrame;
            chart.Caption = Settings.ChartCaption; 
            chart.PlotMode = PlotModes.None;
            chart.XAxisLegend = Settings.XAxisLegend;
        }

        public void SaveAllDataToFile(string destFileName)
        {
            Data.SaveToFile(destFileName);
            Data.SmartSave(destFileName+".smart.dat", 1);
        }

        #region Implementation of IExperiment

        /// <summary>
        /// Рабочая поверхность для всех типов экспериментов, 
        /// позволяет отображать до двух графиков с разными масштабами по оси ординат
        /// </summary>
        public IChart Chart
        { get; protected set; }

        /// <summary>
        /// Класс настроек, соответсвующий классу эксперимента
        /// </summary>
        public virtual Type WantedSettingsType
        {
            get { return typeof(AbstractSettings); }
        }

        /// <summary>
        /// Текущее состояние подключения к устройствам
        /// </summary>
        public MeasurementState CurrentState
        {
            get { return currentState; }
            protected set { currentState = value; }
        }

        /// <summary>
        /// Объект, сохраняющий результаты эксперимента
        /// </summary>
        public RawData Data
        {
            get; protected set;
        }

        private MeasurementState currentState = MeasurementState.Disconnected;


        /// <summary>
        /// Возвращает путь к директории с настройками для данного режима
        /// </summary>
        public string GetSettingsPath()
        {
            return Settings.SettingsPath;
        }

        /// <summary>
        /// Пробует загрузить настройки
        /// </summary>
        /// <param name="fileName">полный путь к файлу настроек</param>
        /// <returns></returns>
        public void LoadSettings(string fileName)
        {            
            var xmlSettings = XElement.Load(fileName);
            var attr = xmlSettings.Attribute(XName.Get("type"));
            if (attr==null || attr.Value != WantedSettingsType.FullName) IsSettingsLoaded = false;
            LoadSettings(xmlSettings);
        }

        /// <summary>
        /// Парсит XElement в поисках значений для настроек ;-)
        /// </summary>
        /// <param name="xmlSettings">XElement</param>
        /// <returns></returns>
        protected virtual void LoadSettings(XElement xmlSettings)
        {
            Settings.ForEachSetByUserFieldsGroup(null, 
                (prop, name) =>
                    {
                        var elem = xmlSettings.Element(XName.Get(prop.Name));
                        if (elem != null)
                        {
                            object value = null;
                            if (prop.PropertyType == typeof (int))
                            {
                                int v;
                                if (Int32.TryParse(elem.Value, out v))
                                {
                                    value = v;
                                }
                            }
                            if (prop.PropertyType.BaseType == typeof(Enum))
                            {
                                value = Enum.Parse(prop.PropertyType, elem.Value);
                            }
                            if (prop.PropertyType == typeof(string))
                            {
                                value = elem.Value;
                            }
                            if (prop.PropertyType == typeof(bool))
                            {
                                value = Boolean.Parse(elem.Value);
                            }
                            if (value != null)
                                prop.SetValue(Settings, value, null);
                        } 
                        else
                        {
                            IsSettingsLoaded = false;
                            return;
                        }
                    }
                );
            IsSettingsLoaded = true;
        }

        /// <summary>
        /// Пробует подключить все необходимые устройства
        /// </summary>
        /// <returns>статус подключения</returns>
        public bool Connect()
        {
            if (CurrentState == MeasurementState.Connected) return true;
            ConnectStatus status = TryConnect();
            if (status == ConnectStatus.Successful)
            {
                CurrentState = IsSettingsLoaded ? MeasurementState.Ready : MeasurementState.Connected;
                return true;
            }
            Disconnect();
            return false;
        }

        /// <summary>
        /// Начинает считывание сигналов с устройств
        /// </summary>
        public virtual void StartMeasurement()
        {
            if (CurrentState != MeasurementState.Ready)
            {
                throw new ApplicationException("Перед началом измерений устройства надо подключить и настроить!");
            }
            Data = new RawData(Settings);
            MeasurementEvent += Data.ProcessMeasurementEvent;
        }

        /// <summary>
        /// Останавливает считывание сигналов с устройств
        /// </summary>
        public abstract void StopMeasurement();


        /// <summary>
        /// отсоединяет все устройства
        /// </summary>
        public abstract void Disconnect();

        #endregion

        public override string ToString()
        {
            return ExperimentCaption;
        }

        public string SettingsToString()
        {
            return Settings.ToString();
        }


    }
}
