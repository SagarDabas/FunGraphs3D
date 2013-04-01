using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Media3D;
using System.Windows.Media;
namespace FunGraphs3D
{
    /// <summary>
    /// Interaction logic for LineComparatorPopUp.xaml
    /// </summary>
    public partial class LineComparatorPopUp : UserControl
    {
        private Dictionary<float,float> list;
        private Dictionary<LineEntity,Dictionary<float,float>> data;
        private float scale = 1;
        public LineComparatorPopUp(Dictionary<LineEntity, Dictionary<float,float>> data, LineEntity entity, float scale)
        {
            InitializeComponent();
            this.data = data;
            this.scale = 10 * scale;
            initIndexList();
        }

        private void initIndexList()
        {
            int columnCount = 0;
            foreach (LineEntity lineEntity in data.Keys)
            {
                data.TryGetValue(lineEntity, out list);
                ColumnDefinition column = new ColumnDefinition();
                LinePopUpGrid.ColumnDefinitions.Add(column);
                Grid grid = new Grid();
                grid.Background = new SolidColorBrush(Colors.LightCyan);
                ColumnDefinition column1 = new ColumnDefinition();
                grid.ColumnDefinitions.Add(column1);
                ColumnDefinition column2 = new ColumnDefinition();
                grid.ColumnDefinitions.Add(column2);
                RowDefinition heading = new RowDefinition();
                heading.MaxHeight = 20;
                grid.RowDefinitions.Add(heading);
                Label entityName = new Label();
                entityName.Content = lineEntity.Title;
                entityName.Background = new SolidColorBrush(lineEntity.TheColor);
                Grid.SetColumnSpan(entityName , 2);
                grid.Children.Add(entityName);
                RowDefinition labels = new RowDefinition();
                labels.MaxHeight = 20;
                grid.RowDefinitions.Add(labels);
                Label labelX = new Label();
                labelX.Content = lineEntity.Label1;
                Grid.SetRow(labelX, 1);
                Grid.SetColumn(labelX,0);
                grid.Children.Add(labelX);
                Label labelY = new Label();
                labelY.Content = lineEntity.Label2;
                Grid.SetRow(labelY, 1);
                Grid.SetColumn(labelY,1);
                grid.Children.Add(labelY);
                Dictionary<float,float>.Enumerator enumerator =  list.GetEnumerator();
                for (int i = 0; i < list.Count; i++)
                {
                    enumerator.MoveNext();
                    RowDefinition row = new RowDefinition();
                    row.MaxHeight = 20;
                    grid.RowDefinitions.Add(row);
                    Label indexLabel = new Label();
                    indexLabel.Content = enumerator.Current.Key;
                    Label yLabel = new Label();
                    yLabel.Content = enumerator.Current.Value;
                    Grid.SetColumn(indexLabel, 0);
                    Grid.SetRow(indexLabel, i + 2);
                    Grid.SetColumn(yLabel, 1);
                    Grid.SetRow(yLabel, i + 2);
                    grid.Children.Add(indexLabel);
                    grid.Children.Add(yLabel);
                }
                DockPanel panel = new DockPanel();
                panel.Width = 300;
                panel.Margin = new Thickness(40,60,40,60);
                ScrollViewer scrollviewer = new ScrollViewer() { CanContentScroll = true, VerticalScrollBarVisibility = ScrollBarVisibility.Auto };
                scrollviewer.Content = grid;
                panel.Children.Add(scrollviewer);
                Grid.SetColumn(panel, columnCount);
                LinePopUpGrid.Children.Add(panel);
                columnCount++;
            }
        }

        private void Grid_MouseDown_1(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            Utility.grid.Children.Remove(this);
        }
   }
}
