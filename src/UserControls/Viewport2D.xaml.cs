using System;
using System.Windows;
using System.Windows.Controls;
using HelixToolkit.Wpf;
using System.Windows.Input;
using System.Windows.Media.Media3D;
using System.Windows.Media;
using System.ComponentModel;


namespace FunGraphs3D
{
    /// <summary>
    /// Interaction logic for Viewport2D.xaml
    /// </summary>
    public partial class Viewport2D : UserControl, INotifyPropertyChanged
    {
        AbstractChartRenderer renderer;
        public Viewport2D()
        {
            InitializeComponent();
            renderer = Utility.chartRenderer;
        }

        private void myViewPort2D_Loaded_1(object sender, RoutedEventArgs e)
        {
            renderer.initViewport(sender, e);
        }

        private void myViewPort2D_MouseDown_1(object sender, MouseButtonEventArgs e)
        {
            IsSidebar = !IsSidebar;
            renderer.mouseDown(sender, e);    
        }

        //It's actually MouseMove
         private void myViewPort2D_MouseEnter_1(object sender, MouseEventArgs e)
        {
            renderer.mouseMove(sender, e);
        }

        private void myViewPort2D_MouseLeave_1(object sender, MouseEventArgs e)
        {
            renderer.mouseLeave(sender, e);
        }
 

        private void myViewPort2D_KeyDown_1(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                renderer.escDown(sender, e);
            }
        }

        private bool isSidebar;
        private bool IsSidebar
        {
            get
            {
                return isSidebar;
            }
            set
            {
                isSidebar = value;
                OnPropertyChanged("IsSidebar");
            }
        }
          public event PropertyChangedEventHandler PropertyChanged;
          public void OnPropertyChanged(string name)
          {
                PropertyChangedEventHandler handler = PropertyChanged;
                handler(null, new PropertyChangedEventArgs(name));
          }

         
        
    }

}
