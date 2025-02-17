﻿using System;
using System.Drawing;
using System.Text;

namespace Sunny.UI.Demo.Charts
{
    public partial class FLineChart : UITitlePage
    {
        public FLineChart()
        {
            InitializeComponent();
        }

        private void uiSymbolButton1_Click(object sender, EventArgs e)
        {
            timer1.Stop();

            DateTime dt = new DateTime(2020, 10, 4);

            UILineOption option = new UILineOption();
            option.ToolTip.Visible = true;
            option.Title = new UITitle();
            option.Title.Text = "SunnyUI";
            option.Title.SubText = "LineChart";

            option.XAxisType = UIAxisType.DateTime;

            var series = option.AddSeries(new UILineSeries("Line1"));
            series.Add(dt.AddHours(0), 1.2);
            series.Add(dt.AddHours(1), 2.2);
            series.Add(dt.AddHours(2), 3.2);
            series.Add(dt.AddHours(3), 4.2);
            series.Add(dt.AddHours(4), 3.2);
            series.Add(dt.AddHours(5), 2.2);
            series.Symbol = UILinePointSymbol.Square;
            series.SymbolSize = 4;
            series.SymbolLineWidth = 2;
            series.SymbolColor = Color.Red;

            series = option.AddSeries(new UILineSeries("Line2", Color.Lime));
            series.Add(dt.AddHours(3), 3.3);
            series.Add(dt.AddHours(4), 2.3);
            series.Add(dt.AddHours(5), 2.3);
            series.Add(dt.AddHours(6), 1.3);
            series.Add(dt.AddHours(7), 2.3);
            series.Add(dt.AddHours(8), 4.3);
            series.Symbol = UILinePointSymbol.Star;
            series.SymbolSize = 4;
            series.SymbolLineWidth = 2;
            series.SymbolColor = Color.Red;
            series.Smooth = true;

            option.GreaterWarningArea = new UILineWarningArea(3.5);
            option.LessWarningArea = new UILineWarningArea(2.2, Color.Gold);

            option.YAxisScaleLines.Add(new UIScaleLine() { Color = Color.Red, Name = "上限", Value = 3.5 });
            option.YAxisScaleLines.Add(new UIScaleLine() { Color = Color.Gold, Name = "下限", Value = 2.2 });

            option.XAxis.Name = "日期";
            option.YAxis.Name = "数值";
            option.XAxis.AxisLabel.DateTimeFormat = "yyyy-MM-dd HH:mm";
            option.XAxis.AxisLabel.AutoFormat = false;
            option.YAxis.AxisLabel.DecimalCount = 1;
            option.YAxis.AxisLabel.AutoFormat = false;

            option.XAxisScaleLines.Add(new UIScaleLine() { Color = Color.Red, Name = dt.AddHours(3).DateTimeString(), Value = new DateTimeInt64(dt.AddHours(3)) });
            option.XAxisScaleLines.Add(new UIScaleLine() { Color = Color.Red, Name = dt.AddHours(6).DateTimeString(), Value = new DateTimeInt64(dt.AddHours(6)) });

            LineChart.SetOption(option);
        }

        private void uiImageButton1_Click(object sender, EventArgs e)
        {
            LineChart.ChartStyleType = UIChartStyleType.Default;
        }

        private void uiImageButton2_Click(object sender, EventArgs e)
        {
            LineChart.ChartStyleType = UIChartStyleType.Plain;
        }

        private void uiImageButton3_Click(object sender, EventArgs e)
        {
            LineChart.ChartStyleType = UIChartStyleType.Dark;
        }

        private void LineChart_PointValue(object sender, UILineSelectPoint[] points)
        {
            StringBuilder sb = new StringBuilder();
            foreach (var point in points)
            {
                sb.Append(point.Name + ", " + point.Index + ", " + point.X + ", " + point.Y);
            }

            Console.WriteLine(sb.ToString());
        }

        private void uiSymbolButton2_Click(object sender, EventArgs e)
        {
            index = 0;
            UILineOption option = new UILineOption();
            option.ToolTip.Visible = true;
            option.Title = new UITitle();
            option.Title.Text = "SunnyUI";
            option.Title.SubText = "LineChart";
            var series = option.AddSeries(new UILineSeries("Line1"));
            series.Smooth = true;

            option.XAxis.AxisLabel.DecimalCount = 1;
            option.XAxis.AxisLabel.AutoFormat = false;
            option.YAxis.AxisLabel.DecimalCount = 1;
            option.YAxis.AxisLabel.AutoFormat = false;
            LineChart.SetOption(option);
            timer1.Start();
        }

        int index = 0;
        Random random = new Random();

        private void timer1_Tick(object sender, EventArgs e)
        {
            LineChart.Option.AddData("Line1", index, random.NextDouble() * 10);
            index++;

            if (index > 50)
            {
                LineChart.Option.XAxis.Max = index + 20;
                LineChart.Option.XAxis.MaxAuto = false;
                LineChart.Option.XAxis.Min = index - 50;
                LineChart.Option.XAxis.MinAuto = false;
            }

            LineChart.Refresh();
        }
    }
}
