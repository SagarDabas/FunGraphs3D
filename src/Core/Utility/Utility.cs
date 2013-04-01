using System;
using System.Windows;
using System.Windows.Media.Animation;
using System.Windows.Controls;


namespace FunGraphs3D
{
    public enum Chart { Line, Area, Pie, Bar }

    class Utility
    {
        public static AbstractChartRenderer chartRenderer = null;
        public static SideBar sidebar;
        public static Viewport2D viewport;
        public static SidebarRight sidebarRight;
        public static Grid grid;


        private static void initChartWindow()
        {
            //Initialiaze viewport and sidebar
            viewport = new Viewport2D();
            sidebar = new SideBar();
            sidebarRight = new SidebarRight();
            //Dock them
            grid = new Grid();
            ColumnDefinition c1 = new ColumnDefinition();
            c1.Width = new GridLength(12, GridUnitType.Star);
            ColumnDefinition c2 = new ColumnDefinition();
            c2.Width = new GridLength(76, GridUnitType.Star);
            ColumnDefinition c3 = new ColumnDefinition();
            c3.Width = new GridLength(12, GridUnitType.Star);
            grid.ColumnDefinitions.Add(c1);
            grid.ColumnDefinitions.Add(c2);
            grid.ColumnDefinitions.Add(c3);

            Grid.SetColumn(sidebar, 0);
            Grid.SetRow(sidebar, 0);
            Grid.SetColumn(viewport, 1);
            Grid.SetRow(viewport, 0);
            Grid.SetColumn(sidebarRight, 2);


            grid.Children.Add(viewport);
            grid.Children.Add(sidebar);
            grid.Children.Add(sidebarRight);

            //set them as the content
            App.Current.MainWindow.Content = grid;
        }


        public static AbstractChartRenderer initChartRenderer(Chart chart)
        {
            initChartWindow();
                switch (chart)
                {
                    case Chart.Line:
                        chartRenderer = new LineChartRenderer();
                        break;
                    case Chart.Area: chartRenderer = new AreaChartRenderer();
                        break;
                    case Chart.Pie: chartRenderer = new PieChartRenderer();
                        break;
                    case Chart.Bar: chartRenderer = new BarChartRenderer();
                        break;
                    default: chartRenderer = new LineChartRenderer();
                        break;
                }
            sidebarRight.Entities = Utility.chartRenderer.entities;
            sidebar.setDefaultValues();
            return chartRenderer;
        }

        


        public static void rotate()
        {

        }


        public static void switchCam()
        {

        }



        public static void save()
        {

        }

        public static void delete()
        {

        }

        public static void load()
        {

        }
    }
}
