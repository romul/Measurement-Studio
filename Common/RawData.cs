using System;
using System.Collections.Generic;

namespace Common
{
    public sealed class RawData : IEnumerable<Point3D>
    {
        #region Fields

        public int TimeInterval { get; private set; }
        private int startPoint;
        private List<MeasPoint> measPoints;
        private IList<Point3D> allPoints;
        int currentNum;
        readonly IDataSaver dataSaver;

        #endregion

        #region Contructors

        public RawData(AbstractSettings settings)
        {
            TimeInterval = settings.MeasurementPeriod;
            dataSaver = CreateDataSaver(settings.SaveMode);
            measPoints = new List<MeasPoint>();
            allPoints = new List<Point3D>();
        }

        #endregion

        #region Методы, отвечающие за стратегию сохранения

        IDataSaver CreateDataSaver(SaveMode mode)
        {
            switch (mode)
            {
                case SaveMode.DontSave:
                    return new NothingSaver(this);
                case SaveMode.SaveOnDemand:
                    return new OnDemandSaver(this);
                case SaveMode.SaveInRuntime:
                    return new RuntimeSaver(this);
                default:
                    throw new ArgumentException("Неизвестный режим сохранения!");
            }
        }

        public void SavePoint(Point3D point)
        {
            dataSaver.SavePoint(point);
        }

        public void SaveToFile(string destFileName)
        {
            dataSaver.SaveAllDataToFile(destFileName);
        }

        public void SmartSave(string destFileName, int dt)
        {
            dataSaver.SmartSave(destFileName, dt);
        }

        #endregion

        public void AddPoint(Point3D point)
        {
            currentNum++;
            allPoints.Add(point);
        }

        public void ProcessMeasurementEvent(object sender, MeasurementEventArgs e)
        {
            switch (e.EventType)
            {
                case MeasurementEvents.Start:
                    startPoint = currentNum;
                    break;
                case MeasurementEvents.Stop:
                    measPoints.Add(new MeasPoint(startPoint, currentNum));
                    break;
            }
        }


        #region IEnumerable<Point3D> Members

        public IEnumerator<Point3D> GetEnumerator()
        {
            foreach (Point3D p in allPoints)
            {
                yield return p;
            }
        }

        #endregion

        #region IEnumerable Members

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion

        #region Custom Enumerators

        public IEnumerable<MeasPoint> MeasPoints
        {
            get
            {
                foreach (MeasPoint point in measPoints)
                {
                    yield return point;
                }
            }
        }

        #endregion

        public Point3D this[int index]
        {
            get { return allPoints[index]; }
        }

        public int Count { get { return allPoints.Count; } }

    }
}
