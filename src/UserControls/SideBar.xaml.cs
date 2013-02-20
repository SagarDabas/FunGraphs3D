using System;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.ComponentModel;
using System.Windows;
using System.Windows.Media.Media3D;
using HelixToolkit.Wpf;

namespace FunGraphs3D
{
    /// <summary>
    /// Interaction logic for ChartScreen.xaml
    /// </summary>
    public partial class SideBar : UserControl
    {
        private AbstractChartRenderer renderer;
        private Storyboard storyBoard;
        private ContentControl current;
        public static readonly DependencyProperty EntityNameProperty = DependencyProperty.Register("EntityName", typeof(String), typeof(SideBar));
        public static readonly DependencyProperty xAxisProperty = DependencyProperty.Register("xAxis", typeof(String), typeof(SideBar));
        public static readonly DependencyProperty yAxisProperty = DependencyProperty.Register("yAxis", typeof(String), typeof(SideBar));
        public static readonly DependencyProperty zAxisProperty = DependencyProperty.Register("zAxis", typeof(String), typeof(SideBar));

         public String EntityName
         {
             get { return (String)base.GetValue(EntityNameProperty);}
             set { base.SetValue(EntityNameProperty, value);}
         }
         public String xAxis
         {
             get { return (String)base.GetValue(xAxisProperty);}
             set { base.SetValue(xAxisProperty, value); }
         }
         public String yAxis
         {
             get { return (String)base.GetValue(yAxisProperty); }
             set { base.SetValue(yAxisProperty, value);}
         }

         public String zAxis
         {
             get { return (String)base.GetValue(zAxisProperty);}
             set { base.SetValue(zAxisProperty, value);}
         }
        
        public SideBar()
        {
            InitializeComponent();
            this.renderer = Utility.chartRenderer;
            EntityName = "Dummy";
            xAxis = "X - Axis";
            yAxis = "Y - Axis";
            zAxis = "Z - Axis";
            Utility.viewport.PropertyChanged += delegate(Object o ,PropertyChangedEventArgs e)
            {
                hideCurrent();
            };
            DataContext = this;
        }

        private void Button_Click_1(object sender, System.Windows.RoutedEventArgs e)
        {
            //renderer.showCompareResult();
        }

        private void Button_Click_2(object sender, System.Windows.RoutedEventArgs e)
        {
            hideCurrent();
            current = entityForm;
            storyBoard = (Storyboard)FindResource("popUpShow");
            storyBoard.Begin(entityForm);
        }

        private void Button_Click_3(object sender, System.Windows.RoutedEventArgs e)
        {
           hideCurrent();
           current = labelForm;
           storyBoard = (Storyboard)FindResource("popUpShow");
           storyBoard.Begin(labelForm);
        }

        private void Button_Click_4(object sender, System.Windows.RoutedEventArgs e)
        {
            CameraController controller = Utility.viewport.myViewPort2D.CameraController;
            controller.CameraPosition = new Point3D(48,40,20);
            controller.CameraUpDirection = new Vector3D(0,0,1);
            controller.CameraTarget = new Point3D(0, 40, 20);
        }

        private void Button_Click_5(object sender, System.Windows.RoutedEventArgs e)
        {

        }

        private void Button_Click_6(object sender, System.Windows.RoutedEventArgs e)
        {

        }

        private void Button_Click_7(object sender, System.Windows.RoutedEventArgs e)
        {
            MainMenu mainScreen = new MainMenu();
            App.Current.MainWindow.Content = mainScreen;
        }

        private void Button_Click_8(object sender, System.Windows.RoutedEventArgs e)
        {
            renderer.addEntity(EntityName,colorPicker.SelectedColor);
            hideCurrent();
        }

        private void Button_Click_9(object sender, System.Windows.RoutedEventArgs e)
        {
            hideCurrent();
        }

        private void hideCurrent()
        {
            if (storyBoard != null)
            {
                storyBoard = (Storyboard)FindResource("popUpHide");
                storyBoard.Begin(current);
            }
        }

        private void Button_Click_10(object sender, RoutedEventArgs e)
        {
            Utility.setLabels(xAxis,yAxis,zAxis);
        }


    }
}
