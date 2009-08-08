using System.Drawing;
using Common;
using ZedGraph;

namespace MeasurementStudio
{
    class ZedGraphProxy : IChart
    {
        private readonly ZedGraphControl Chart;
        private readonly GraphPane myPane;
        private readonly PointPairList y1List;
        private readonly PointPairList y2List;
        readonly MasterPane master;

        public ZedGraphProxy(ZedGraphControl chart)
        {
            Chart = chart;
            
            master = chart.MasterPane;
            master.Fill = new Fill(Color.White, Color.FromArgb(220, 220, 255), 45.0f);
            master.PaneList.Clear();

            master.Title.IsVisible = true;
            //master.Title.Text = "";

            master.Margin.All = 10;
            master.InnerPaneGap = 0;
           
            myPane = new GraphPane(new Rectangle(25, 25, chart.Width - 50, chart.Height - 50), "", "", "");

           // myPane.Title.Text = "";
            myPane.XAxis.Title.Text = "Time, s";
            myPane.Y2Axis.IsVisible = true;
            myPane.Fill.IsVisible = false;
            myPane.Chart.Fill = new Fill(Color.White, Color.LightYellow, 45.0F);
            myPane.Margin.All = 0;
            myPane.Margin.Top = 20;

            master.Add(myPane);

            y1List = new PointPairList();
            y2List = new PointPairList();

            var myCurve = myPane.AddCurve("", y1List, Color.Red, SymbolType.Circle);
            myCurve.IsVisible = false;
            myCurve = myPane.AddCurve("", y2List, Color.Green, SymbolType.Circle);
            myCurve.IsVisible = false;
            myCurve.IsY2Axis = true;
            chart.IsSynchronizeYAxes = true;           
            chart.Invalidate();
        }

        #region Implementation of IChart

        public void Clear()
        {
            //myPane.CurveList.Clear();
            myPane.CurveList.ForEach(curve => curve.Clear());
        }

        public string Caption 
        {
            get { return master.Title.Text; }
            set { master.Title.Text = value; } 
        }


        public void SetSeriesNames(params string[] seriesNames)
        {
            for (int j = 0; j < seriesNames.Length; j++)
            {
                CurveItem curve = myPane.CurveList[j];
                if (seriesNames[j] == null) continue;
                curve.Label = new Label(seriesNames[j], new FontSpec());
                curve.IsVisible = true;
                if (j == 0) myPane.YAxis.Title.Text = seriesNames[0] ?? "";
                if (j == 1) myPane.Y2Axis.Title.Text = seriesNames[1] ?? ""; 
            }         
            
        }

        public void AddPoint(Point3D point)
        {
            if (AutoScroll && y1List.Count >= DotsPerFrame)
            {
                y1List.RemoveAt(0);
            }
            if (point.Z != null)
            {
                if (AutoScroll && y2List.Count >= DotsPerFrame)
                {
                    y2List.RemoveAt(0);
                }
                var z = (double) point.Z;
                switch (PlotMode)
                {
                    case PlotModes.Y1AndY2:
                        myPane.CurveList[0].AddPoint(point.X, point.Y);
                        myPane.CurveList[1].AddPoint(point.X, z);
                        break;
                    case PlotModes.Y1OfY2:
                        myPane.CurveList[0].AddPoint(z, point.Y);
                        break;
                    case PlotModes.Y2OfY1:
                        myPane.CurveList[0].AddPoint(point.Y, z);
                        break;
                }
                
            } 
            else
            {
               myPane.CurveList[0].AddPoint(point.X, point.Y); 
            }
            Chart.AxisChange();
            Chart.Refresh();
        }

        public bool LegendVisible
        {
            get { return myPane.Legend.IsVisible; }
            set { myPane.Legend.IsVisible = value; }
        }
        public string XAxisLegend
        {
            get { return myPane.XAxis.Title.Text; }
            set { myPane.XAxis.Title.Text = value; }
        }

        public bool AutoScroll { get; set; }
        public int DotsPerFrame { get; set; }
        public PlotModes PlotMode { get; set; }
        

        #endregion
    }
}
