using System;
using System.ComponentModel;

namespace Common
{
    [CLSCompliant(true)]
    public abstract class TwoDevicesExperiment : AbstractExperiment
    {         
        /// <summary>
        /// Инициализирует компонент для построения графиков настройками
        /// </summary>
        /// <param name="chart">интерфейсная ссылка на объект компонента для построения графиков</param>
        public override void ApplySettingsToChart(IChart chart)
        {
            var settings = (TwoDevicesSettings) Settings;
            base.ApplySettingsToChart(chart);
            chart.SetSeriesNames(Settings.Series1Legend, settings.Series2Legend);
            chart.PlotMode = settings.PlotMode;
        }

        /// <summary>
        /// Класс настроек, соответсвующий классу эксперимента
        /// </summary>
        public override Type WantedSettingsType
        {
            get { return typeof(TwoDevicesSettings); }
        }
    }

    public abstract class TwoDevicesSettings : AbstractSettings
    {
        [Text]
        [SetByUser(Caption = "Легенда №2", Category = "2-ая зависимость")]
        public virtual string Series2Legend
        {
            get { return series2Legend; }
            set { series2Legend = value; }
        }
        string series2Legend = "Series2";

        [Enum(EnumType = typeof(Transform.Functions))]
        [SetByUser(Caption = "y = f(y)", Category = "2-ая зависимость")]
        public virtual Transform.Functions Y2Transform
        {
            get { return y2Transform; }
            set { y2Transform = value; }
        }
        Transform.Functions y2Transform = Transform.Functions.Nothing;

        [Enum(EnumType = typeof(PlotModes))]
        [SetByUser(Caption = "Режим построения", Category = "2-ая зависимость")]
        public virtual PlotModes PlotMode
        {
            get { return plotMode; }
            set { plotMode = value; }
        }
        PlotModes plotMode = PlotModes.Y1AndY2; 
    }

    public enum PlotModes
    {
        None,
        [Description("y1(x), y2(x)")]
        Y1AndY2,
        [Description("y1(y2)")]
        Y1OfY2,
        [Description("y2(y1)")]
        Y2OfY1,
    }
}
