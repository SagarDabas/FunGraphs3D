using HelixToolkit.Wpf;
using System.Windows.Input;
using System.Windows;
using System.Collections.Generic;
using System.Windows.Data;
using System;
using System.Windows.Media.Media3D;
using System.Windows.Media;

namespace FunGraphs3D
{
    public class AreaChartRenderer : AbstractChartRenderer
    {
        private AreaChart movingAreaC;
        private AreaEntity currentAreaEntity;
        private Dictionary<int , AreaChart> totalAreaC;
        private double chartXOffset = 0.001;
        private bool isESCkey;
        private bool isComplete;
        private Dictionary<int, double> previousExtrusion;

        public AreaChartRenderer()
        {
            totalAreaC = new Dictionary<int, AreaChart>();
            previousExtrusion = new Dictionary<int, double>();
            //never call overridable method in the constructor of the base class.
            addEntity(false, "Dummy", Colors.ForestGreen, "X-Axis", "Y-Axis");
        }

        public override void changeActiveEntity(AbstractEntity areaEntity)
        {
            currentAreaEntity = (AreaEntity)areaEntity;
            activeEntity = currentAreaEntity;
            movingAreaC = totalAreaC[currentAreaEntity.Id];
        }

        public override void delDown(object sender, KeyEventArgs e)
        {

        }

        public override void addEntity(bool editFlag, String title, Color color, params String[] labels)
        {
            if (!editFlag)
            {
                currentAreaEntity = new AreaEntity();
                activeEntity = currentAreaEntity;
                entities.Add(currentAreaEntity);
                //new allocations for the new entity.
                currentAreaEntity.Id = entities.Count;
                previousExtrusion.Add(currentAreaEntity.Id, currentAreaEntity.Ext);
                movingAreaC = null;
            }
            if (currentAreaEntity != null)
            {
                currentAreaEntity.Title = title;
                currentAreaEntity.TheColor = color;
                currentAreaEntity.Label1 = labels[0];
                currentAreaEntity.Label2 = labels[1];
            }
        }

        public override void initViewport(object sender, RoutedEventArgs e)
        {
            CameraController controller = viewport.CameraController;
            controller.CameraTarget = new Point3D(0, 40, 20);
        }
        public override void mouseDown(object sender, MouseButtonEventArgs e)
        {
            var point = viewport.FindNearestPoint(e.GetPosition(viewport));
            if (point.HasValue)
            {
                if (movingAreaC == null)
                {
                    isComplete = false;
                    isESCkey = false;
                    addNewAreaC(e, point.Value);
                }
                else
                {
                    if (!isESCkey)
                    {
                        if (!isComplete)
                        {
                            //only add the point if it is greater than the previous value.
                            //the last point in the list is modified using the mouseMove event i.e. point pressed is already in the list.
                            //but this point is the last hence it is temporary(since the mouseMove event changes the last point in the list)
                            //therefore to make it permanent add it again so that now it the second last and the last is the same point.
                            //and the last point just added can now be used as temporary point.
                            if (point.Value.Y > movingAreaC.PointsList[movingAreaC.PointsList.Count - 2].Y)
                                movingAreaC.PointsList.Add(movingAreaC.PointsList[movingAreaC.PointsList.Count - 1]);
                        }

                    }
                    else
                    {
                        //if extrusion mode is on (i.e. isESCkey is true) then if mouse is pressed then stop updating chart by setting isComplete=true
                        //and update the previousExtrusion by new updated value.
                        //is the user press the mouse again and isComplete is true then set isComlete to false and stop the extrusion mode.
                        if (!isComplete)
                        {
                            previousExtrusion[currentAreaEntity.Id] = currentAreaEntity.Ext;
                            isComplete = true;
                        }
                        else
                        {
                            isComplete = false;
                            isESCkey = false;
                            //this point is added again as it was removed when isESCkey was set true.
                            movingAreaC.PointsList.Add(movingAreaC.PointsList[movingAreaC.PointsList.Count - 1]);
                        }
                    }
                }
            }
        }

        private void addNewAreaC(MouseButtonEventArgs e, Point3D point)
        {
            chartXOffset += 0.002;
            movingAreaC = new AreaChart();
            totalAreaC.Add(currentAreaEntity.Id, movingAreaC);
            Binding colorBinding = new Binding("TheColor");
            colorBinding.Converter = new ColorToSolidColorBrushValueConverter();
            colorBinding.ConverterParameter = 1.0;
            colorBinding.Source = currentAreaEntity;
            BindingOperations.SetBinding(movingAreaC, AreaChart.FillProperty, colorBinding);
            Binding extrusionBinding = new Binding("Ext");
            extrusionBinding.Source = currentAreaEntity;
            BindingOperations.SetBinding(movingAreaC,AreaChart.ExtrusionProperty,extrusionBinding);
            movingAreaC.PointsList = new Point3DCollection();
            movingAreaC.PointsList.Add(new Point3D(chartXOffset, point.Y, point.Z));
            movingAreaC.PointsList.Add(new Point3D(chartXOffset, point.Y, point.Z));
            movingAreaC.IsRender = true;
            viewport.Children.Add(movingAreaC);
        }
       
        public override void mouseMove(object sender, MouseEventArgs e)
        {
            if (!isComplete)
            {
                var point = viewport.FindNearestPoint(e.GetPosition(viewport));
                if (movingAreaC != null && point.HasValue)
                {
                    //if isESCkey is false then add the points in the chartArea.
                    if (!isESCkey)
                    {
                        //only if it is greater than the previous value
                        if (point.Value.X < point.Value.Y && point.Value.X < point.Value.Z && point.Value.Y > movingAreaC.PointsList[movingAreaC.PointsList.Count - 2].Y)
                        {
                            movingAreaC.PointsList[movingAreaC.PointsList.Count - 1] = new Point3D(chartXOffset, point.Value.Y, point.Value.Z);
                        }
                    }
                    //if isESCkey is true then extrude the chartArea
                    else
                    {
                        if (point.Value.Z < point.Value.X && point.Value.Z < point.Value.Y)
                        {
                            currentAreaEntity.Ext = point.Value.X;
                        }
                    }
                }
            }
        }


        public override void escDown(object sender, KeyEventArgs e)
        {
            if (!isComplete)
            {
                if (!isESCkey)
                {
                    isESCkey = true;
                    if (movingAreaC.PointsList.Count == 2)
                    {
                        viewport.Children.Remove(movingAreaC);
                        totalAreaC.Remove(currentAreaEntity.Id);
                        movingAreaC = null;
                    }
                    else
                    {
                        movingAreaC.PointsList.RemoveAt(movingAreaC.PointsList.Count - 1);
                    }
                }
                else
                {
                    isComplete = true;
                    currentAreaEntity.Ext = previousExtrusion[currentAreaEntity.Id];
                    //this point is added again as it was removed when isESCkey was set true.
                    movingAreaC.PointsList.Add(movingAreaC.PointsList[movingAreaC.PointsList.Count - 1]);
                }
            }
        }

        public override void showCompareResult(){}
    }
}
