using System.Collections.Generic;
using System.Linq;
using Common;

namespace MeasurementStudio
{
    using System;
    using System.Windows.Forms.DataVisualization.Charting;

    class MSChartProxy : Common.IChart
    {
        private readonly Chart chart;
        private const string unvisibleLegend = "UnvisibleLegend";
        private const string legendName = "MainLegend";
        private readonly List<double> visibleY1 = new List<double>();
        public MSChartProxy(Chart chart)
        {
            this.chart = chart;
            this.chart.Palette = ChartColorPalette.Pastel;
            this.chart.Legends.Clear();
            foreach (var s in this.chart.Series) 
                s.Points.Clear();

            chart.ChartAreas[0].AxisY.Minimum = 0;            
            var legend = new Legend(legendName)
            {
                Alignment = System.Drawing.StringAlignment.Center,
                Docking = Docking.Top,
                Enabled = false
            };
            this.chart.Legends.Add(legend);
            legend = new Legend(unvisibleLegend){ Enabled = false };
            this.chart.Legends.Add(legend);
            this.SetSeriesNames("^1", "^2");
        }

        #region Implementation of IChart

        public void Clear()
        {            
            chart.Series.Clear();
        }

        public string Caption
        {
            get;
            set;
        }

        private static bool ShouldBeShowSeriesInLegend(Series series)
        {
            return (String.IsNullOrEmpty(series.Name) || series.Name[0] == '^');
        }

        public void SetSeriesNames(params string[] seriesNames)
        {
            for (int j = 0; j < seriesNames.Length; j++)
            {
                if (seriesNames[j] == null) continue;
                Series series;
                if (j < chart.Series.Count)
                {
                    series = chart.Series[j];
                }
                else
                {
                    series = new Series
                                 {
                                     ChartType = SeriesChartType.Point,
                                     MarkerSize = 10,
                                     MarkerStyle = MarkerStyle.Circle,
                                     XValueType = ChartValueType.Double,
                                     YValueType = ChartValueType.Double,
                                 };
                    chart.Series.Add(series);
                }
                series.Name = seriesNames[j];
                series.Legend = ShouldBeShowSeriesInLegend(series) ? unvisibleLegend : legendName;
                if (j == 1) series.YAxisType = AxisType.Secondary;
            }
        }


        public void AddPoint(Common.Point3D p)
        {            
            if (AutoScroll && chart.Series[0].Points.Count >= DotsPerFrame)
            {
                visibleY1.RemoveAt(0);
                chart.Series[0].Points.RemoveAt(0);
                chart.ChartAreas[0].AxisY.Minimum = Math.Floor(10 * visibleY1.Min())/10;
                chart.ChartAreas[0].AxisY.Maximum = Math.Ceiling(10 * visibleY1.Max())/10;
            }
            if (p.Z != null && AutoScroll && chart.Series[0].Points.Count >= DotsPerFrame)
            {
                chart.Series[1].Points.RemoveAt(0);
            }                
            chart.Series[0].Points.AddXY(p.X, p.Y);
            chart.ResetAutoValues();
            visibleY1.Add(p.Y);
            if (p.Z != null) chart.Series[1].Points.AddXY(p.X, p.Z); 
        }


        public bool LegendVisible {
            get { return chart.Legends[0].Enabled; }
            set { chart.Legends[0].Enabled = value; }
        }
        public bool AutoScroll { get; set; }
        public int DotsPerFrame { get; set; }
        public PlotModes PlotMode { get; set; }
        public string XAxisLegend { get; set; }

        #endregion
    }
}
