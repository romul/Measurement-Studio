namespace TSU.Voltmeters.APPA
{
    /// <summary>
    /// Поддерживаемые компонентом варианты режимов измерения
    /// </summary>
    public enum MeasurementMode
    {
        V, mV
    }

    /// <summary>
    /// Скорость измерения(зависит от шкалы)
    /// </summary>
    public enum MeasurementVelocity
    {
        /// <summary>
        /// Цифровая шкала -> 2 изм/с
        /// </summary>
        DigitalScale = 2,
        /// <summary>
        /// Линейная шкала -> 20 изм/с
        /// </summary>
        LinearScale = 20  
    }
}