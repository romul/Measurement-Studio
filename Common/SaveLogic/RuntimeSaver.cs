using System;
using System.IO;
using System.Diagnostics;

namespace Common
{
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
            Debug.Assert(!tempFileName.IsNullOrEmpty(), "Путь к временному файлу не определён.");
            File.AppendAllText(tempFileName, p + "\n");
        }

        public void SaveAllDataToFile(string destFileName)
        {
            Debug.Assert(!tempFileName.IsNullOrEmpty(), "Путь к временному файлу не определён.");
            File.Copy(tempFileName, destFileName, true);
        }

        public void SmartSave(string FileName, int dt)
        {
            Debug.Assert(!tempFileName.IsNullOrEmpty(), "Путь к временному файлу не определён.");
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
