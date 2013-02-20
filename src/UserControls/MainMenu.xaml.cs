using System;
using System.Windows;
using System.Windows.Media.Animation;
using System.Windows.Controls;

namespace FunGraphs3D
{
    /// <summary>
    /// Interaction logic for MainScreen.xaml
    /// </summary>
    public partial class MainMenu : UserControl
    {
        private bool isCreateButtonClicked = false;
        private Storyboard storyBoard;
        public MainMenu()
        {
            InitializeComponent();
        }
        
       
        private void ArcButton_Click_1(object sender, RoutedEventArgs e)
        {
            if (!isCreateButtonClicked)
            {
                isCreateButtonClicked = true;
                storyBoard = (Storyboard)FindResource("mainMenuAnim1");
                storyBoard.Begin(this);
            }
        }


         private void ArcButton_MouseEnter_1(object sender, System.Windows.Input.MouseEventArgs e)
        {
             //only when createButton is clicked and also it will stop runnin this method again and again if it is already running.
            if (isCreateButtonClicked)
            {
                //Stop the running storyBoard. Already stopped when Opacity is 1.
                if (pieChart.Opacity !=1 )
                    storyBoard.Stop();

                isCreateButtonClicked = false;
                storyBoard = (Storyboard)FindResource("mainMenuAnim2");
                storyBoard.Begin(this);
            }
        }


         private void areaChart_MouseEnter_1(object sender, System.Windows.Input.MouseEventArgs e)
         {
             if (pieChart.Opacity != 0 && pieChart.Opacity != 1)
             {
                 //when opacity is going zero then only start this animation and also it will stop runnin this method again and again if it is already running.
                 if (!isCreateButtonClicked)
                 {
                     isCreateButtonClicked = true;
                     storyBoard.Stop();
                     ((Storyboard)FindResource("mainMenuAnim1")).Begin(this);
                 }
             }
         }

         private void barChart_Click_1(object sender, RoutedEventArgs e)
         {
             Utility.initChartRenderer(Chart.Bar);
         }

         private void areaChart_Click_1(object sender, RoutedEventArgs e)
         {

             Utility.initChartRenderer(Chart.Area);
         }

         private void lineChart_Click_1(object sender, RoutedEventArgs e)
         {

             Utility.initChartRenderer(Chart.Line);
         }

         private void pieChart_Click_1(object sender, RoutedEventArgs e)
         {

             Utility.initChartRenderer(Chart.Pie);
         }
    
    }
}
