using System;
using System.Windows.Media.Media3D;
using System.Windows.Media;
using HelixToolkit.Wpf;
using System.Windows.Input;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Data;
using System.Collections.Generic;

namespace FunGraphs3D
{
    public class PieChartRenderer : AbstractChartRenderer
    {
        private PieSlice3D selectedSlice;
        private PieSlice3D pieHovered;
        private PieEntity currentPieEntity;
        private Grid sidebarGrid;
        private Label percentageLabel;
        private Dictionary<PieSlice3D, int> totalSlices;
        private Dictionary<int, PieSlice3D> revTotalSlices;
        private bool isOnSlice;

        public PieChartRenderer()
        {
            totalSlices = new Dictionary<PieSlice3D, int>();
            revTotalSlices = new Dictionary<int, PieSlice3D>();
            sidebarGrid = new Grid();
            viewport.Children.Remove(Utility.viewport.blueBox);
            ColumnDefinition c1 = new ColumnDefinition();
            c1.Width = new GridLength(15, GridUnitType.Star);
            ColumnDefinition c2 = new ColumnDefinition();
            c2.Width = new GridLength(85, GridUnitType.Star);
            sidebarGrid.ColumnDefinitions.Add(c1);
            sidebarGrid.ColumnDefinitions.Add(c2);
            addEntity(false, "Dummy", Colors.ForestGreen, "X-Axis", "Y-Axis");
        }

        public override void changeActiveEntity(AbstractEntity pie)
        {
            currentPieEntity = (PieEntity)pie;
            activeEntity = currentPieEntity;
            Utility.sidebar.setDefaultValues();
        }

        public override void addEntity(bool editFlag, String title, Color color, params String[] labels)
        {
            if (!editFlag)
            {
                currentPieEntity = new PieEntity();
                activeEntity = currentPieEntity;
                entities.Add(currentPieEntity);
                //new allocations for the new entity.
                currentPieEntity.Id = entities.Count;
                addPieSlice();
            }
            //Do this everywhere
            if (currentPieEntity != null)
            {
                currentPieEntity.Title = title;
                currentPieEntity.TheColor = color;
                currentPieEntity.Label1 = labels[0];
                currentPieEntity.Label2 = labels[1];
            }
            if (selectedSlice != null)
            {
                //if a box is selected then donot change its color when edited.
                selectedSlice.SetCurrentValue(PieSlice3D.VerticalLengthProperty, 2.0);
                sidebarGrid.Background = new SolidColorBrush(activeEntity.TheColor);
            }
        }

        private void addPieSlice()
        {
            PieSlice3D tempSlice = new PieSlice3D();
            totalSlices.Add(tempSlice,entities.Count);
            revTotalSlices.Add(entities.Count,tempSlice);
            tempSlice.Center = new Point3D(0,0,0);
            Binding colorBinding = new Binding("TheColor");
            colorBinding.Converter = new ColorToSolidColorBrushValueConverter();
            colorBinding.Source = currentPieEntity;
            BindingOperations.SetBinding(tempSlice, PieSlice3D.FillProperty, colorBinding);
            tempSlice.InnerRadius = 1;
            tempSlice.OuterRadius = 5;
            viewport.Children.Add(tempSlice);

            updateSlices();
        }

        private void updateSlices()
        {
            //update these every time a new Slice is added.
            int div = 360 / entities.Count;
            int count = 1;
            foreach (PieEntity entity in entities)
            {
                revTotalSlices[entity.Id].StartAngle = (count - 1) * div;
                revTotalSlices[entity.Id].EndAngle = (count) * div;
                //currentSlice.VerticalLength = 5;
                revTotalSlices[entity.Id].ThetaDiv = 60 / entities.Count;
                entity.Percentage = (int)(100 / entities.Count);
                count++;
            }
            if (percentageLabel != null)
            {
                percentageLabel.Content = "" + (int)(100 / entities.Count);
            }
        }

        public override void delDown(object sender, KeyEventArgs e)
        {
            if (selectedSlice != null)
            {
                entities.RemoveAt(totalSlices[selectedSlice] - 1);
                revTotalSlices.Remove(totalSlices[selectedSlice]);
                totalSlices.Remove(selectedSlice);
                viewport.Children.Remove(selectedSlice);
                deselect();
                pieHovered = null;
                if (entities.Count > 0)
                {
                    updateSlices();
                    changeActiveEntity(entities[entities.Count - 1]);
                }
            }
        }

        public override void initViewport(object sender, RoutedEventArgs e)
        {
            CameraController controller = viewport.CameraController;
            controller.CameraPosition = new Point3D(0,0,13);
            controller.CameraTarget = new Point3D(0, 0, 0);
            controller.CameraUpDirection = new Vector3D(1,0,0);
        }

        public override void mouseDown(object sender, MouseButtonEventArgs e)
        {
            if (selectedSlice == null || selectedSlice != pieHovered)
            {
                if (selectedSlice != null)
                {
                    deselect();
                }
                if (pieHovered != null)
                {
                    selectSlice();
                }
            }
            else if (selectedSlice == pieHovered)
            {
                deselect();
            }
        }


        private void deselect()
        {
            //if you do pieHovered null in this method then the new hovered slice will not be selected
            //only the selected slice will be deselected.
            selectedSlice.SetCurrentValue(PieSlice3D.VerticalLengthProperty, 2.0);
            selectedSlice = null;
            isOnSlice = false;
            //Remove slider and add listbox
            Utility.sidebarRight.grid.Children.Remove(sidebarGrid);
            Utility.sidebarRight.grid.Children.Add(Utility.sidebarRight.listBox);
        }

        private void selectSlice()
        {
            selectedSlice = pieHovered;
            changeActiveEntity((PieEntity)entities[totalSlices[selectedSlice] - 1]);
            sidebarGrid.Children.Clear();

            //add slider
            Label percentageSign;
            Slider slider;
            slider = new Slider();
            slider.Orientation = Orientation.Vertical;
            slider.Value = (int)((selectedSlice.EndAngle - selectedSlice.StartAngle)/3.6);
            slider.IsSnapToTickEnabled = true;
            slider.Minimum = 0;
            slider.Maximum = 100;
            slider.TickPlacement = System.Windows.Controls.Primitives.TickPlacement.BottomRight;
            slider.TickFrequency = 1;
            sidebarGrid.Background = new SolidColorBrush(activeEntity.TheColor);
            Grid.SetColumn(slider, 0);
            sidebarGrid.Children.Add(slider);

            Grid panel = new Grid();
            panel.Background = new SolidColorBrush(Colors.DarkGray); ;
            RowDefinition r1 = new RowDefinition();
            r1.Height = new GridLength(10, GridUnitType.Star);
            RowDefinition r2 = new RowDefinition();
            r2.Height = new GridLength(90, GridUnitType.Star);
            ColumnDefinition c1 = new ColumnDefinition();
            ColumnDefinition c2 = new ColumnDefinition();
            
            panel.RowDefinitions.Add(r1);
            panel.RowDefinitions.Add(r2);
            panel.ColumnDefinitions.Add(c1);
            panel.ColumnDefinitions.Add(c2);


            Label name = new Label();
            name.FontSize = 20;
            name.Foreground = new SolidColorBrush(Colors.WhiteSmoke);
            Binding nameBinding = new Binding("Title");
            nameBinding.Source = activeEntity;
            BindingOperations.SetBinding(name, Label.ContentProperty, nameBinding);
            Grid.SetColumnSpan(name, 2);

           
            Binding percentageBinding = new Binding("Value");
            percentageBinding.Source = slider;
            slider.ValueChanged += new RoutedPropertyChangedEventHandler<double>(slider_ValueChanged);
            percentageLabel = new Label();
            percentageLabel.FontSize = 35;
            percentageLabel.Foreground = new SolidColorBrush(Colors.WhiteSmoke);
            BindingOperations.SetBinding(percentageLabel, Label.ContentProperty, percentageBinding);
            Grid.SetColumn(percentageLabel,0);
            Grid.SetRow(percentageLabel,1);

            percentageSign = new Label();
            percentageSign.FontSize = 35;
            percentageSign.Content = "%";
            percentageSign.Foreground = new SolidColorBrush(Colors.WhiteSmoke);
            Grid.SetColumn(percentageSign, 1);
            Grid.SetRow(percentageSign, 1);

            panel.Children.Add(name);
            panel.Children.Add(percentageLabel);
            panel.Children.Add(percentageSign);

            Grid.SetColumn(panel,1);
            sidebarGrid.Children.Add(panel);

            //Remove listbox and add slider
            Utility.sidebarRight.grid.Children.Remove(Utility.sidebarRight.listBox);
            Utility.sidebarRight.grid.Children.Add(sidebarGrid);
        }
       
        private void slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            Slider slider = e.OriginalSource as Slider;

            if (slider != null)
            {
                int id = totalSlices[selectedSlice];
                int id2 = id;
                double temp;
                if (id < totalSlices.Count)
                {
                    temp = (int)(selectedSlice.StartAngle  + (e.NewValue / 100) * 360);
                    if (temp > revTotalSlices[id + 1].EndAngle)
                    {
                        temp = (int)revTotalSlices[id + 1].EndAngle;
                        double temp2 = (int)((selectedSlice.EndAngle - selectedSlice.StartAngle) / 3.6);
                        slider.SetCurrentValue(Slider.ValueProperty, temp2);
                    }
                    else if (temp < selectedSlice.StartAngle)
                    {
                        temp = (int)selectedSlice.StartAngle;
                        double temp2 = (int)((selectedSlice.EndAngle - selectedSlice.StartAngle) / 3.6);
                        slider.SetCurrentValue(Slider.ValueProperty, temp2);
                    }
                    selectedSlice.EndAngle = temp;
                    revTotalSlices[id + 1].StartAngle = temp;
                    id2 = id + 1;
                }
                else if (id == totalSlices.Count && id>1)
                {
                    temp = (int)(360 - ((e.NewValue) / 100) * 360);
                    if (temp < revTotalSlices[id - 1].StartAngle)
                    {
                        temp = (int)revTotalSlices[id - 1].StartAngle;
                        double temp2 = (int)((selectedSlice.EndAngle - selectedSlice.StartAngle) / 3.6);
                        slider.SetCurrentValue(Slider.ValueProperty, temp2);
                    }
                    revTotalSlices[id - 1].EndAngle = temp;
                    selectedSlice.StartAngle = temp;
                    id2 = id - 1;
                }
                else if (id == 1)
                {
                    temp = (int)((e.NewValue / 100) * 360);
                    selectedSlice.StartAngle = 0;
                    selectedSlice.EndAngle = temp;
                }
                //update the percentage of the selected Slice's entity.
                ((PieEntity)entities[id -1]).Percentage = (int)((selectedSlice.EndAngle - selectedSlice.StartAngle)/3.6);
                ((PieEntity)entities[id2 - 1]).Percentage = (int)((revTotalSlices[id2].EndAngle - revTotalSlices[id2].StartAngle) / 3.6);

            }
        }

        public override void mouseMove(object sender, MouseEventArgs e)
        {
             var visual = viewport.FindNearestVisual(e.GetPosition(viewport));
             if (visual != null)
             {
                 if (visual.GetType() == typeof(PieSlice3D) && !isOnSlice)
                 {
                     isOnSlice = true;
                     pieHovered = (PieSlice3D)visual;
                     if (selectedSlice == null || selectedSlice != pieHovered)
                     {
                         //by programmatically changing the value, binding is lost, so instead this method is used.
                         pieHovered.SetCurrentValue(PieSlice3D.VerticalLengthProperty, 3.0);
                     }
                 }
                 else if (visual.GetType() != typeof(PieSlice3D) && isOnSlice)
                 {
                     isOnSlice = false;
                     if (selectedSlice != pieHovered)
                     {
                         pieHovered.SetCurrentValue(PieSlice3D.VerticalLengthProperty, 2.0);
                     }
                     pieHovered = null;
                 }
                 else if (visual.GetType() == typeof(PieSlice3D) && isOnSlice)
                 {
                     //if new pieHovered that is visual is not equal to the previous hovered box then deselect the previous
                     //and select the new.
                     if (visual != pieHovered)
                     {
                         if (pieHovered != null && pieHovered != selectedSlice)
                         {
                             //remove previous hovered box
                             pieHovered.SetCurrentValue(PieSlice3D.VerticalLengthProperty, 2.0);
                             pieHovered = null;
                         }

                         pieHovered = (PieSlice3D)visual;
                         if (selectedSlice == null || selectedSlice != pieHovered)
                         {
                             //set the new hovered box
                             pieHovered.SetCurrentValue(PieSlice3D.VerticalLengthProperty, 3.0);
                         }
                     }
                 }
             }
             else
             {
                 if (pieHovered != null && pieHovered != selectedSlice)
                 {
                     //remove previous hovered box
                     int id = totalSlices[pieHovered];
                     pieHovered.SetCurrentValue(PieSlice3D.VerticalLengthProperty, 2.0);
                 }
                 isOnSlice = false;
                 pieHovered = null;
             }
        }
        
        public override void escDown(object sender, KeyEventArgs e){ }

        public override void showCompareResult() { }
    }
}
