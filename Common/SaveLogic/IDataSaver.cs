namespace Common
{
    internal interface IDataSaver
    {
        /// <summary>
        /// Объект, содержащий все полученные данные
        /// </summary>
        RawData Data { get; }

        /// <summary>
        /// Сохраняет результат единичного измерения
        /// </summary>
        /// <param name="p">результат единичного измерения</param>
        /// <returns></returns>
        void SavePoint(Point3D p);

        /// <summary>
        /// Сохранить все данные в файл
        /// </summary>
        /// <param name="FileName"></param>
        /// <returns></returns>
        void SaveAllDataToFile(string FileName);

        /// <summary>
        ///  Реализует "умное сохранение"
        /// </summary>
        /// <param name="FileName">путь к файлу для сохранения</param>
        /// <param name="dt">временная окрестность экспеимента</param>
        /// <returns></returns>
        void SmartSave(string FileName, int dt);
    }
}