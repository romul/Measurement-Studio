namespace Common
{
    class NothingSaver : IDataSaver
    {
        public NothingSaver(RawData data)
        {
            Data = data;
        }

        #region IDataSaver Members

        public RawData Data { get; private set; }
        public void SavePoint(Point3D p) { }
        public void SaveAllDataToFile(string FileName) { }
        public void SmartSave(string FileName, int dt) { }

        #endregion
    }
}
