using System;
using System.Windows;
using System.Threading;
using System.Windows.Media.Animation;

namespace FunGraphs3D
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        //VirtualMouse mouse;
        public Thread newThread;
        public MainWindow()
        {
            InitializeComponent();
        }

        private void HelixViewport3D_Loaded_1(object sender, RoutedEventArgs e)
        {
            //mouse = new VirtualMouse();
            //newThread = new Thread(new ThreadStart(mouse.initFingerTracking));
            //newThread.IsBackground = true;
            //newThread.Start();
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            Application.Current.Shutdown();
        }

        private void Window_Closing_1(object sender, System.ComponentModel.CancelEventArgs e)
        {
            //mouse.SetFlag();
            //Application.Current.Shutdown();

        }
    }
}
