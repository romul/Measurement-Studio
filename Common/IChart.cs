namespace Common
{
    public interface IChart
    {        
        void Clear();
        void SetSeriesNames(params string[] seriesNames);
        void AddPoint(Point3D p);

        string Caption { get; set; }
        string XAxisLegend { get; set; }
        bool LegendVisible { get; set; }
        bool AutoScroll { get; set; }
        int DotsPerFrame { get; set; }
        PlotModes PlotMode { get; set; }
    }
}
