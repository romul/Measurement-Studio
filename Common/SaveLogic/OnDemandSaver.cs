using System;
using System.IO;
using System.Diagnostics;

namespace Common
{
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
