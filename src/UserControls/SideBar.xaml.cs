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
        private Storyboard storyBoard;
        private ContentControl current;
        public static readonly DependencyProperty EntityProperty = DependencyProperty.Register("EntityName", typeof(String), typeof(SideBar));
        public static readonly DependencyProperty XLabelProperty = DependencyProperty.Register("XLabel", typeof(String), typeof(SideBar));
        public static readonly DependencyProperty YLabelProperty = DependencyProperty.Register("YLabel", typeof(String), typeof(SideBar));
        private float xScale = 1;
        private float yScale = 1;
        
         public String EntityName
         {
             get { return (String)base.GetValue(EntityProperty); }
             set { base.SetValue(EntityProperty, value); }
         }
         public String XLabel
         {
             get { return (String)base.GetValue(XLabelProperty); }
             set { base.SetValue(XLabelProperty, value); }
         }
         public String YLabel
         {
             get { return (String)base.GetValue(YLabelProperty); }
             set { base.SetValue(YLabelProperty, value); }
         }

         public float XScale
         {
             get { return xScale; }
             set { xScale = value; }
         }
         public float YScale
         {
             get { return yScale; }
             set { yScale = value; }
         }

        public SideBar()
        {
            InitializeComponent();
            Utility.viewport.PropertyChanged += delegate(Object o ,PropertyChangedEventArgs e)
            {
                hideCurrent();
            };
            DataContext = this;
        }

        public void setDefaultValues()
        {
            EntityName = Utility.chartRenderer.activeEntity.Title;
            XLabel = Utility.chartRenderer.activeEntity.Label1;
            YLabel = Utility.chartRenderer.activeEntity.Label2;
            colorPicker.SelectedColor = Utility.chartRenderer.activeEntity.TheColor;
        }

        private void Button_Click_1(object sender, System.Windows.RoutedEventArgs e)
        {
             Utility.chartRenderer.showCompareResult();
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
            Utility.chartRenderer.initViewport(sender,e);
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
            Utility.chartRenderer.addEntity(false, EntityName, colorPicker.SelectedColor, XLabel, YLabel);
            hideCurrent();
        }

        private void Button_Click_9(object sender, System.Windows.RoutedEventArgs e)
        {
            Utility.chartRenderer.addEntity(true, EntityName, colorPicker.SelectedColor, XLabel, YLabel);
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
            //Change it to 3D -- Utility.chartRenderer.setScales(XScale,YScale,XScale);
            Utility.chartRenderer.setScales(XScale,YScale);
            //overwriting numbers. Rethink about it. Previous numbers are not yet removed.
            Utility.chartRenderer.updateNumbers();
            hideCurrent();
        }


    }
}
