using System;
using System.ComponentModel;
using System.Diagnostics;

namespace Common
{
    using System.Collections.Generic;

    public enum SaveMode
    {
        None, 
        [Description("Сохранять в ран-тайме")]
        SaveInRuntime,
        [Description("Сохранять по запросу")]
        SaveOnDemand,
        [Description("Не сохранять")]
        DontSave
    }
    public enum ConnectStatus { Successful, OneDeviceNotConnected, AllDevicesNotConnected }
    public enum MeasurementState { Connected, Ready, Started, Stopped, Disconnected }

    /* 
    public static class Extender
    {
        public static string ToString(this Enum o, int dummy)
        {
            var member = o.GetType().GetMember(o.ToString());
            if (member != null)
            {
                var attr = Attribute.GetCustomAttribute(member[0], typeof(DescriptionAttribute)) as
                           DescriptionAttribute;

                return attr != null ? attr.Description : o.ToString();
            }
            return o.ToString();
        }
    }
    */
    public static class Transform 
    {
        public enum Functions
        {
            None,
            [Description("f(y) = y")]
            Nothing,
            [Description("f(y) = CuprumConstantant(mV -> C)")]
            CuprumConstantantFromMiliVoltToCelcium,
            [Description("f(y) = 1000*y")]
            MultiplyByThousand,
            [Description("f(y) = sqrt(y)")]
            Sqrt,
            [Description("f(y) = ln(y)")]
            Log,
            [Description("f(y) = sin(y)")]
            Sin,
            [Description("f(y) = cos(y)")]
            Cos,
                        
        }

        private static readonly Dictionary<Functions, Func<double, double>> tfDictonary =
                new Dictionary<Functions, Func<double, double>>
                    {
                        {Functions.Nothing, x => x},
                        {Functions.MultiplyByThousand, x => 1000*x},
                        {Functions.Sqrt, Math.Sqrt},
                        {Functions.Log, Math.Log},
                        {Functions.Sin, Math.Sin},
                        {Functions.Cos, Math.Cos},
                        {Functions.CuprumConstantantFromMiliVoltToCelcium, CuprumConstantantFromMiliVoltToCelcium}
                    };


        public static double Call(this Functions func, double xValue)
        {
            Debug.Assert(tfDictonary.ContainsKey(func));
            return tfDictonary[func](xValue);
        }

        private static double CuprumConstantantFromMiliVoltToCelcium(double x)
        {
            return 25000*x;
        }
    }


    #region Deprecated
    /*
    public interface IExperiment
    {
        /// <summary>
        /// Рабочая поверхность для всех типов экспериментов, 
        /// позволяет отображать до двух графиков с разными масштабами по оси ординат
        /// </summary>
        Chart Chart { get; }

        /// <summary>
        /// Класс настроек, соответсвующий классу эксперимента
        /// </summary>
        Type WantedSettingsType { get; }

        MeasurementState CurrentState { get; }

        /// <summary>
        /// Пробует применить настройки, по факту - необходимо проверить, 
        /// что класс настроек соответствует классу эксперимента
        /// </summary>
        /// <param name="settings">настройки</param>
        /// <returns>класс настроек соответствует классу эксперимента</returns>
        bool ApplySettings(AbstractSettings settings);

        /// <summary>
        /// Пробует подключить все необходимые устройства
        /// </summary>
        /// <returns>удалось подключить всё необходимое</returns>
        bool Connect();

        /// <summary>
        /// Начинает считывание сигналов с устройств
        /// </summary>
        void StartMeasurement();

        /// <summary>
        /// Останавливает считывание сигналов с устройств
        /// </summary>
        void StopMeasurement();

        /// <summary>
        /// отсоединяет все устройства
        /// </summary>
        void Disconnect();

        /// <summary>
        /// Переопределяем метод ToString
        /// </summary>
        /// <returns>описание эксперимента на основе настроек</returns>
        string ToString();
    } */
    #endregion
}
