using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace FunGraphs3D
{
    class ArcButton : Button
    {

            //why these properties are read only.
        public static readonly  DependencyProperty StartAngleProperty;
        public static readonly DependencyProperty EndAngleProperty;
        public static readonly DependencyProperty RotateAngleProperty;
        public static readonly DependencyProperty ArcThicknessProperty;
        public static readonly DependencyProperty StrokeThicknessProperty;
        public static readonly DependencyProperty StrokeProperty;




        static ArcButton()
        {
            StartAngleProperty = DependencyProperty.Register("StartAngle", typeof(double), typeof(ArcButton));
            EndAngleProperty = DependencyProperty.Register("EndAngle", typeof(double), typeof(ArcButton),new PropertyMetadata(0.0,OnEndAnglePropertyChanged));
            RotateAngleProperty = DependencyProperty.Register("RotateAngle", typeof(double), typeof(ArcButton));
            ArcThicknessProperty = DependencyProperty.Register("ArcThickness", typeof(double), typeof(ArcButton));
            StrokeThicknessProperty = DependencyProperty.Register("StrokeThickness", typeof(double), typeof(ArcButton));
            StrokeProperty = DependencyProperty.Register("Stroke", typeof(Brush), typeof(ArcButton));

        }

        private static void OnEndAnglePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var vm = (ArcButton)d;
            if (vm.EndAngle == 90 || vm.EndAngle == 360)
                vm.RotateAngle = vm.EndAngle - 45;
            else
                if (vm.EndAngle == 270)
                    vm.RotateAngle = vm.EndAngle + 315;
                else
                    vm.RotateAngle = vm.EndAngle + 135;
        }

      
        public double StartAngle
        {
            get { return (double)base.GetValue(StartAngleProperty); }
            set { base.SetValue(StartAngleProperty, value); }
        }

        public double EndAngle
        {
            get { return (double)base.GetValue(EndAngleProperty); }
            set 
            { 
                base.SetValue(EndAngleProperty, value);
            }
        }

        public double RotateAngle
        {
            get { return (double)base.GetValue(RotateAngleProperty); }
            set { base.SetValue(RotateAngleProperty, value); }

        }
        public double ArcThickness
        {
            get { return (double)base.GetValue(ArcThicknessProperty); }
            set { base.SetValue(ArcThicknessProperty, value); }

        }
        public double StrokeThickness
        {
            get { return (double)base.GetValue(ArcThicknessProperty); }
            set { base.SetValue(ArcThicknessProperty, value); }

        }
        public Brush Stroke
        {
            get { return (Brush)base.GetValue(ArcThicknessProperty); }
            set { base.SetValue(ArcThicknessProperty, value); }

        }
    }
}
