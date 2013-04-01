using System;
using System.Collections.Generic;
using System.Windows.Media;
using HelixToolkit.Wpf;
using System.Windows.Input;
using System.Windows;
using System.Windows.Media.Media3D;
using System.Collections.ObjectModel;
using System.Windows.Data;

namespace FunGraphs3D
{

    public abstract class AbstractChartRenderer
    {
        public AbstractEntity activeEntity;
        protected HelixViewport3D viewport;
        public ObservableCollection<AbstractEntity> entities;
        protected float xScale = 1;
        protected float yScale = 1;
        protected float zScale = 1;
        private List<TextVisual3D> numbers;
       

        public AbstractChartRenderer()
        {
            entities = new ObservableCollection<AbstractEntity>();
            this.viewport = Utility.viewport.myViewPort2D;
            addNumbers();
        }

        public void setScales(float x, float y, float z = 1)
        {
            if (x > 0)
                xScale = x;
            if (y > 0)
                yScale = y;
            if (y > 0)
                zScale = z;
        }
        

        private void addNumbers()
        {
            if (numbers == null)
            {
                numbers = new List<TextVisual3D>();
                TextVisual3D origin = new TextVisual3D();
                origin.Position = new Point3D(0, 1, 1);
                origin.Foreground = new SolidColorBrush(Colors.Black);
                origin.TextDirection = new Vector3D(0, 1, 0);
                origin.FontSize = 20;
                origin.Height = 1;
                Utility.viewport.blueBox.Children.Add(origin);
                origin.Text = "(0,0,0)";
            }


            for (int i = 10; i <= 80; i = i + 10)
            {
               TextVisual3D textXaxis = new TextVisual3D();
               textXaxis.Text = "(" + i * xScale + ",0,0)";
               textXaxis.Position = new Point3D(0, i, 1);
               textXaxis.Foreground = new SolidColorBrush(Colors.White);
               textXaxis.TextDirection = new Vector3D(0, 1, 0);
               textXaxis.FontSize = 20;
               textXaxis.Height = 1;
               numbers.Add(textXaxis);
                
                if (i <= 40)
                {
                    TextVisual3D textYaxis = new TextVisual3D();
                    textYaxis.Text = "(0," + i*yScale + ",0)";
                    textYaxis.Position = new Point3D(0, 1, i);
                    textYaxis.Foreground = new SolidColorBrush(Colors.White);
                    textYaxis.TextDirection = new Vector3D(0, 1, 0);
                    textYaxis.FontSize = 20;
                    textYaxis.Height = 1;
                    Utility.viewport.blueBox.Children.Add(textYaxis);
                    numbers.Add(textYaxis);
                }

                TextVisual3D textZaxis = new TextVisual3D();
                textZaxis.Text = "(0,0," + i*zScale + ")";
                textZaxis.Foreground = new SolidColorBrush(Colors.White);
                textZaxis.Position = new Point3D(i, 0, 1);
                textZaxis.FontSize = 20;
                textZaxis.TextDirection = new Vector3D(1, 0, 0);
                textZaxis.Height = 1;
                numbers.Add(textZaxis);

                Utility.viewport.blueBox.Children.Add(textXaxis);
                Utility.viewport.blueBox.Children.Add(textZaxis);
            }
        }

        public void updateNumbers()
        {
            if (numbers != null)
            {
                int index = 0;
                for (int i = 10; i <= 80; i = i + 10)
                {
                    numbers[index].Text = "(" + i * xScale + ",0,0)";
                    index++;
                    if (i <= 40)
                    {
                        numbers[index].Text = "(0," + i * yScale + ",0)";
                        index++;
                    }
                    numbers[index].Text = "(0,0," + i * zScale + ")";
                    index++;
                }
            }
        }


        public abstract void addEntity(bool editFlag, String title, Color color, params String[] labels);
        //this method is called when viewport2D is loaded
        public abstract void initViewport(object sender, RoutedEventArgs e);
        public abstract void mouseDown(object sender, MouseButtonEventArgs e);
        public abstract void mouseMove(object sender, MouseEventArgs e);
        public abstract void escDown(object sender, KeyEventArgs e);
        public abstract void delDown(object sender, KeyEventArgs e);
        public abstract void changeActiveEntity(AbstractEntity line);
        public abstract void showCompareResult();
    }
}
