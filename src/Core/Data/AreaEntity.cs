using HelixToolkit.Wpf;
using System.Collections.Generic;
using System;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using System.Windows;

namespace FunGraphs3D
{
    class AreaEntity : AbstractEntity
    {
        private List<AreaChart> areaCharts;
        public static readonly DependencyProperty ExtProperty = DependencyProperty.Register("Ext", typeof(double), typeof(AreaEntity));
        public double Ext
        {
            get { return (double)base.GetValue(ExtProperty); }
            set { base.SetValue(ExtProperty, value); }
        }

        public List<AreaChart> AreaCharts
        {
            get
            {
                return areaCharts;
            }
            set
            {
                areaCharts = value;
            }
        }


    }
}
