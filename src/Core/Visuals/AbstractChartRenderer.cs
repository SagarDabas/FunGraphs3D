using System;
using System.Collections.Generic;
using System.Windows.Media;
using HelixToolkit.Wpf;
using System.Windows.Input;
using System.Windows;
using System.Windows.Media.Media3D;

namespace FunGraphs3D
{

    public abstract class AbstractChartRenderer
    {

        //Sidebar and viewport are available as static public fields of utility so need of calling setSideBarAndViewport
        //
        protected SideBar sidebar;
        protected HelixViewport3D viewport;
        public List<AbstractEntity> entities;

        public AbstractChartRenderer()
        {
            entities = new List<AbstractEntity>();
        }

        public void setSideBarAndViewport(SideBar sidebar, HelixViewport3D viewport)
        {
            this.sidebar = sidebar;
            this.viewport = viewport;
            addNumbers();
        }

        private void addNumbers()
        {
            TextVisual3D origin = new TextVisual3D();
            origin.Text = "(0,0,0)";
            origin.Position = new Point3D(0, 1, 1);
            origin.Foreground = new SolidColorBrush(Colors.White);
            origin.TextDirection = new Vector3D(0, 1, 0);
            origin.FontSize = 20;
            origin.Height = 1;
            viewport.Children.Add(origin);


            for (int i = 10; i <= 80; i = i + 10)
            {
                
                TextVisual3D textXaxis = new TextVisual3D();
                textXaxis.Text = "(" + i + ",0,0)";
                textXaxis.Position = new Point3D(0,i,1);
                textXaxis.Foreground = new SolidColorBrush(Colors.White);
                textXaxis.TextDirection = new Vector3D(0,1,0);
                textXaxis.FontSize = 20;
                textXaxis.Height = 1;

                if (i <= 40)
                {
                    TextVisual3D textYaxis = new TextVisual3D();
                    textYaxis.Text = "(0," + i + ",0)";
                    textYaxis.Position = new Point3D(0, 1, i);
                    textYaxis.Foreground = new SolidColorBrush(Colors.White);
                    textYaxis.TextDirection = new Vector3D(0, 1, 0);
                    textYaxis.FontSize = 20;
                    textYaxis.Height = 1;

                    viewport.Children.Add(textYaxis);
                }

                TextVisual3D textZaxis = new TextVisual3D();
                textZaxis.Text = "(0,0," + i + ")";
                textZaxis.Foreground = new SolidColorBrush(Colors.White);
                textZaxis.Position = new Point3D(i, 0, 1);
                textZaxis.FontSize = 20;
                textZaxis.TextDirection = new Vector3D(1, 0, 0);
                textZaxis.Height = 1;

                viewport.Children.Add(textXaxis);
                viewport.Children.Add(textZaxis);
            }
        }
        public abstract void addEntity(String title,Color color);
        public abstract void initViewport(object sender, RoutedEventArgs e);
        public abstract void mouseDown(object sender, MouseButtonEventArgs e);
        public abstract void mouseEnter(object sender, MouseEventArgs e);
        public abstract void mouseLeave(object sender, MouseEventArgs e);
        public abstract void mouseMove(object sender, MouseEventArgs e);
        public abstract void escDown(object sender, KeyEventArgs e);
        public abstract void showCompareResult();
    }
}
