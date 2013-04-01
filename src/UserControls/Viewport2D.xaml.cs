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
        public Viewport2D()
        {
            InitializeComponent();
        }

        private void myViewPort2D_Loaded_1(object sender, RoutedEventArgs e)
        {
            Utility.chartRenderer.initViewport(sender, e);
        }

        private void myViewPort2D_MouseDown_1(object sender, MouseButtonEventArgs e)
        {
            IsSidebar = !IsSidebar;
            Utility.chartRenderer.mouseDown(sender, e);    
        }

        //It's actually MouseMove
         private void myViewPort2D_MouseEnter_1(object sender, MouseEventArgs e)
        {
            Utility.chartRenderer.mouseMove(sender, e);
        }

        private void myViewPort2D_KeyDown_1(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                Utility.chartRenderer.escDown(sender, e);
            }
            else if (e.Key == Key.Delete)
            {
                Utility.chartRenderer.delDown(sender, e);
            }
        }

        private bool isSidebar;
        public bool IsSidebar
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
