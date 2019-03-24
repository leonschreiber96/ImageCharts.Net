﻿using ImageCharts.Net.ChartProperties;
using ImageCharts.Net.Enums;
using ImageCharts.Net.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ImageCharts.Net.Charts
{
    public class BarChart : Chart
    {
        public BarChartStyle BarChartStyle { get; set; }

        public BarChart() : base() { }

        public BarChart(ChartData chartData, BarChartStyle lineChartStyle) : base(chartData)
        {
            this.BarChartStyle = BarChartStyle;
        }

        protected override Dictionary<ChartProperty, string> GetChartProperties()
        {
            var chartProperties = base.GetChartProperties();

            // Add colors
            if (this.ChartData.DataSeries.Any(x => x.Fill != null))
            {
                var colors = this.ChartData.DataSeries.Select(x => x.Fill).ToList();

                var colorStrings = new List<string>();
                var gradientStrings = new List<string>();

                foreach (var fill in colors)
                {
                    if (fill is SingleColorFill singleColorFill)
                    {
                        colorStrings.Add(singleColorFill.Color.GetHexString());
                    }
                    else if (fill is MultiColorFill multiColorFill)
                    {
                        colorStrings.Add($"{string.Join("|", multiColorFill.Colors.Select(x => x.Color.GetHexString()))}");
                    }

                    else if (fill is GradientFill gradientFill)
                    {
                        var dataSeriesIndex = colors.IndexOf(fill);

                        gradientStrings.Add($"b{dataSeriesIndex},lg,{gradientFill.Angle}," +
                            $"{string.Join(",", gradientFill.GradientColors.Select(x => $"{x.Color.GetHexString()},{x.CenterPoint}"))}");
                    }
                }

                if (colorStrings.Any())
                {
                    chartProperties.Add(ChartProperty.DataFill, string.Empty);

                    chartProperties[ChartProperty.DataFill] = string.Join(",", colorStrings);
                }

                if (gradientStrings.Any())
                {
                    if (!chartProperties.ContainsKey(ChartProperty.Fill))
                    {
                        chartProperties.Add(ChartProperty.Fill, string.Join("|", gradientStrings));
                    }
                    else
                    {
                        chartProperties[ChartProperty.Fill] += $"|{string.Join("|", gradientStrings)}";
                    }
                }
            }

            return chartProperties;
        }

        protected override string GetChartTypeSpecifier()
        {
            switch (this.BarChartStyle)
            {
                case BarChartStyle.GroupedVertically:
                    return "bvg";
                case BarChartStyle.GroupedHorizontally:
                    return "bhg";
                case BarChartStyle.StackedVertically:
                    return "bvs";
                case BarChartStyle.StackedHorizontally:
                    return "bhs";
                default:
                    throw new InvalidOperationException($"{this.BarChartStyle} is not a valid bar chart style.");
            }
        }
    }
}