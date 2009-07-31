using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

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

    class NothingSaver : IDataSaver
    {
        public NothingSaver(RawData data)
        {
            Data = data;
        }

        #region IDataSaver Members

        public RawData Data { get; private set; }
        public void SavePoint(Point3D p){}
        public void SaveAllDataToFile(string FileName){}
        public void SmartSave(string FileName, int dt){}

        #endregion
    }

    class OnDemandSaver : IDataSaver
    {
        public OnDemandSaver(RawData data)
        {
            Data = data;
        }

        #region IDataSaver Members

        public RawData Data { get; private set; }

        public void SavePoint(Point3D p)
        {
            Data.AddPoint(p);
        }

        public void SaveAllDataToFile(string FileName)
        {
            using (var stream = File.AppendText(FileName))
            {
                foreach (var point in Data)
                {
                    stream.WriteLine(point.ToString());
                }
            }       
        }

        public void SmartSave(string FileName, int dt)
        {
            using (var stream = File.AppendText(FileName))
            {
                var dn = 1000*dt/Data.TimeInterval;
                foreach (var point in Data.MeasPoints)
                {
                    var start = point.Start - dn;
                    if (start < 0) start = 0;
                    var stop = point.Stop + dn;
                    if (stop > Data.Count) stop = Data.Count;
                    for (var num = start; num <= stop; num++)
                        stream.WriteLine(Data[num].ToString());
                }
            }            
        }

        #endregion
    }

    class RuntimeSaver : IDataSaver
    {
        readonly string tempFileName = Path.GetTempFileName();

        public RuntimeSaver(RawData data)
        {
            Data = data;
        }

        #region IDataSaver Members

        public RawData Data { get; private set; }

        public void SavePoint(Point3D p)
        {
            Data.AddPoint(p);
            if (tempFileName.IsNullOrEmpty()) throw new AccessViolationException("Не удалось получить доступ к временному файлу");
            File.AppendAllText(tempFileName, p + "\n");
        }

        public void SaveAllDataToFile(string destFileName)
        {
            if (tempFileName.IsNullOrEmpty()) throw new AccessViolationException("Не удалось получить доступ к временному файлу");
            File.Copy(tempFileName, destFileName, true);
        }

        public void SmartSave(string FileName, int dt)
        {
            using (var stream = File.AppendText(FileName))
            {
                var dn = 1000 * dt / Data.TimeInterval;
                foreach (var point in Data.MeasPoints)
                {
                    var start = point.Start - dn;
                    if (start < 0) start = 0;
                    var stop = point.Stop + dn;
                    if (stop > Data.Count) stop = Data.Count;
                    for (var num = start; num <= stop; num++)
                        stream.WriteLine(Data[num].ToString());
                }
            }
        }

        #endregion
    }
}
