using System;
using System.Windows.Media.Media3D;
using System.Windows.Media;
using HelixToolkit.Wpf;
using System.Collections.Generic;
using System.Windows.Input;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace FunGraphs3D
{
    class LineChartRenderer : AbstractChartRenderer
    {
        private Dictionary<PieSliceVisual3D, RectangleVisual3D> selectedCircles;
        private Dictionary<PieSliceVisual3D, int> totalCircles;
        private Dictionary<int, List<PieSliceVisual3D>> listOfCircles;
        private Dictionary<int, LinesVisual3D> totalLines;
        private TextVisual3D movingPoint;
        private bool isESCkey;
        private bool isOnCircle ;
        private LineEntity currentLineEntity;
        private PieSliceVisual3D circleHovered;
        private LinesVisual3D currentLine;
        private Point3DCollection points;
        private LinesVisual3D movingLines;
        private Point3DCollection movingPoints;
       
        public LineChartRenderer()
        {
            //add a default entity
            selectedCircles = new Dictionary<PieSliceVisual3D, RectangleVisual3D>();
            totalCircles = new Dictionary<PieSliceVisual3D, int>();
            listOfCircles = new Dictionary<int, List<PieSliceVisual3D>>();
            totalLines = new Dictionary<int, LinesVisual3D>();
            movingPoints = new Point3DCollection();
            addEntity(false, "Dummy", Colors.ForestGreen, "X-Axis", "Y-Axis");
        }


        public override void addEntity(bool editFlag, String title, Color color, params String[] labels)
        {
            if (!editFlag)
            {
                if (currentLineEntity == null)
                {
                    addNewEntity();
                }
                else if (currentLineEntity.Points != null && currentLineEntity.Points.Count >= 2)
                {
                    Utility.sidebarRight.listBox.SelectedItem = null;
                    addNewEntity();
                }
            }
            if (currentLineEntity != null)
            {
                currentLineEntity.Title = title;
                currentLineEntity.TheColor = color;
                currentLineEntity.Label1 = labels[0];
                currentLineEntity.Label2 = labels[1];
            }
            //cannot bind the movingLines since they are same for every entity.
            //so they have to be updated every time.
            if (movingLines != null)
            {
                movingLines.Color = color;
            }
        }

        private void addNewEntity()
        {
            currentLineEntity = new LineEntity();
            activeEntity = currentLineEntity;
            entities.Add(currentLineEntity);
            //new allocations for the new entity.
            points = new Point3DCollection();
            currentLineEntity.Points = points;
            movingLines = null;
            currentLine = null;
            currentLineEntity.Id = entities.Count;
        }

        public override void initViewport(object sender, RoutedEventArgs e)
        {
            CameraController controller = viewport.CameraController;
            controller.CameraTarget = new Point3D(0, 40, 20);
        }

        public override void changeActiveEntity(AbstractEntity line)
        {
            currentLineEntity = (LineEntity)line;
            activeEntity = currentLineEntity;
            currentLine = totalLines[currentLineEntity.Id];
            points = currentLineEntity.Points;
            if (movingLines != null)
                movingLines.Color = currentLineEntity.TheColor;
        }

        public override void mouseDown(object sender, MouseButtonEventArgs e)
        {
            var point = viewport.FindNearestPoint(e.GetPosition(viewport));
            if (point.HasValue)
            {
                 //check if the mouse is not on a node, then only insert a new node.
                if (!isOnCircle)
                {
                    //start making moving currentLine.
                    isESCkey = false;

                    //add point to the points collection for line rendering.
                    points.Add(point.Value);
                    
                    //lines need a pair of point to render.
                    if (points.Count % 2 == 0)
                    {
                        points.Add(point.Value);
                    }
                    //add line and circle.
                     addLine();
                     addCircle(point);
                }
                else
                {
                    //if mouse is on a node, then select or deselect that node.
                    selectNode(point);
                }
            }
            
        }

        private void selectNode(Point3D? point)
        {
            if (!selectedCircles.ContainsKey(circleHovered))
            {
                //make the selectedCircle entity as the currect active entity for drawing the info of the selected node.
                //drawNodeInfo checks the currentLineEntity for drawing its points.
                int id = totalCircles[circleHovered];
                selectedCircles.Add(circleHovered,drawNodeInfo(point,id));
            }
            else
            {
                deleteNodeInfo(circleHovered);
            }
            isOnCircle = false;
        }

        private RectangleVisual3D drawNodeInfo(Point3D? point, int id)
        {
            LineEntity tempEntity = (LineEntity)entities[id - 1];
            RectangleVisual3D panelRect = new RectangleVisual3D();
            TextVisual3D entityName = new TextVisual3D();
            TextVisual3D myPoint = new TextVisual3D();
            panelRect.Length = tempEntity.Title.Length + 10;
            panelRect.Width = 7;
            Binding colorBinding = new Binding("TheColor");
            colorBinding.Converter = new ColorToSolidColorBrushValueConverter();
            colorBinding.Source = tempEntity;
            BindingOperations.SetBinding(panelRect, RectangleVisual3D.FillProperty, colorBinding);

            if (point.Value.X < point.Value.Y && point.Value.X < point.Value.Z)
            {
                panelRect.Origin = new Point3D(point.Value.X + 4, point.Value.Y + panelRect.Length/3, point.Value.Z + 1 + panelRect.Width/2 );
                panelRect.Normal = new Vector3D(1,0,0);
                panelRect.LengthDirection = myPoint.TextDirection = entityName.TextDirection  = new Vector3D(0, 1, 0);
                myPoint.UpDirection = entityName.UpDirection = new Vector3D(0, 0, 1);
                myPoint.Position = new Point3D(point.Value.X + 4, point.Value.Y + 2, point.Value.Z + 3);
                entityName.Position = new Point3D(point.Value.X + 4, point.Value.Y + 2, point.Value.Z + 7);

            }
            else if (point.Value.Y < point.Value.X && point.Value.Y < point.Value.Z)
            {
                panelRect.Origin = new Point3D(point.Value.X +panelRect.Length / 3, point.Value.Y + 3.9, point.Value.Z + 1+ panelRect.Width / 2);
                panelRect.Normal = new Vector3D(0, 1, 0);
                panelRect.LengthDirection = entityName.TextDirection =  myPoint.TextDirection = new Vector3D(1, 0, 0);
                entityName.UpDirection = myPoint.UpDirection = new Vector3D(0, 0, 1);
                myPoint.Position = new Point3D(point.Value.X + 2, point.Value.Y + 4, point.Value.Z + 3);
                entityName.Position = new Point3D(point.Value.X + 2, point.Value.Y + 4, point.Value.Z + 7);

            }
            else if (point.Value.Z < point.Value.Y && point.Value.Z < point.Value.X)
            {
                panelRect.Origin = new Point3D(point.Value.X - 1 - panelRect.Width / 2, point.Value.Y + panelRect.Length / 3, point.Value.Z + 3.9);
                panelRect.Normal = new Vector3D(0, 0, 1);
                panelRect.LengthDirection = entityName.TextDirection =  myPoint.TextDirection = new Vector3D(0, 1, 0);
                entityName.UpDirection = myPoint.UpDirection = new Vector3D(-1, 0, 0);
                myPoint.Position = new Point3D(point.Value.X - 3, point.Value.Y + 2, point.Value.Z + 4);
                entityName.Position = new Point3D(point.Value.X - 7, point.Value.Y + 2, point.Value.Z + 4);
            }
            myPoint.Text = "(" + (int)point.Value.Y*xScale + "," + (int)point.Value.Z*yScale + "," + (int)point.Value.X + ")";
            entityName.Text = tempEntity.Title;

            myPoint.Foreground = new SolidColorBrush(Colors.White);
            entityName.Foreground = myPoint.Foreground;

            myPoint.FontSize = entityName.FontSize = 30;
            entityName.FontWeight = FontWeights.Bold;
            myPoint.Height = entityName.Height = 2;

            panelRect.Children.Add(entityName);
            panelRect.Children.Add(myPoint);
            viewport.Children.Add(panelRect);
            return panelRect;
        }

        private void addListOfCircles(PieSliceVisual3D circle)
        {
            List<PieSliceVisual3D> tempList;
            if (!listOfCircles.ContainsKey(currentLineEntity.Id))
            {
                tempList = new List<PieSliceVisual3D>();
                tempList.Add(circle);
                listOfCircles.Add(currentLineEntity.Id, tempList);
            }
            else
            {
               tempList = listOfCircles[currentLineEntity.Id];
               tempList.Add(circle);
            }
        }

        private void addCircle(Point3D? point)
        {
            PieSliceVisual3D circle = new PieSliceVisual3D();
            totalCircles.Add(circle,currentLineEntity.Id);
            addListOfCircles(circle);
            circle.Fill = new SolidColorBrush(Colors.LightSkyBlue);
            circle.ThetaDiv = 30;
            if (point.Value.X < point.Value.Y && point.Value.X < point.Value.Z)
            {
                circle.Normal = new Vector3D(1, 0, 0);
                circle.Center = new Point3D(point.Value.X + 0.1, point.Value.Y, point.Value.Z);
            }
            else if (point.Value.Y < point.Value.X && point.Value.Y < point.Value.Z)
            {
                circle.Normal = new Vector3D(0, 1, 0);
                circle.UpVector = new Vector3D(0, 0, 1);
                circle.Center = new Point3D(point.Value.X, point.Value.Y + 0.1, point.Value.Z);
            }
            else if (point.Value.Z < point.Value.Y && point.Value.Z < point.Value.X)
            {
                circle.Normal = new Vector3D(0, 0, 1);
                circle.Center = new Point3D(point.Value.X, point.Value.Y, point.Value.Z + 0.1);
            }
            circle.InnerRadius = 0;
            circle.OuterRadius = 0.5;
            circle.StartAngle = 0;
            circle.EndAngle = 360;
            viewport.Children.Add(circle);
        }

        private void addLine()
        {
            if (currentLine == null)
            {
                currentLine = new LinesVisual3D();
                totalLines.Add(currentLineEntity.Id, currentLine);
                viewport.Children.Add(currentLine);
                currentLine.Thickness = 3;
                Binding colorBinding = new Binding("TheColor");
                colorBinding.Source = activeEntity;
                BindingOperations.SetBinding(currentLine, LinesVisual3D.ColorProperty, colorBinding);
            }
            currentLine.Points = points;
        }

       
        public override void mouseMove(object sender, MouseEventArgs e)
        {
            var point = viewport.FindNearestPoint(e.GetPosition(viewport));
            var visual = viewport.FindNearestVisual(e.GetPosition(viewport));

            if (point.HasValue && visual.GetType() == typeof(RectangleVisual3D))
            {
                //show the current point value.
                showPoint(point);
            }

            //if escape key is pressed do not show the moving line.
            if (points.Count >= 1 && !isESCkey)
            {
                if (point.HasValue)
                {
                    addMovingLine(point);
                }
            }
            else if(isESCkey)
            {
                //stops making currentLine. user can select a node now.
                onHoverCircle(e);
            }
        }

        private void onHoverCircle(MouseEventArgs e)
        {
            var visual = viewport.FindNearestVisual(e.GetPosition(viewport));
            if (visual != null)
            {
                if (visual.GetType() == typeof(PieSliceVisual3D) && !isOnCircle)
                {
                    circleHovered = (PieSliceVisual3D)visual;
                    isOnCircle = true;
                    if (!selectedCircles.ContainsKey(circleHovered))
                    {
                        circleHovered.OuterRadius = 1;
                        circleHovered.Fill = new SolidColorBrush(Colors.White);
                    }
                }
                else if (visual.GetType() != typeof(PieSliceVisual3D) && isOnCircle)
                {
                    isOnCircle = false;
                    if (!selectedCircles.ContainsKey(circleHovered))
                    {
                        circleHovered.OuterRadius = 0.5;
                        circleHovered.Fill = new SolidColorBrush(Colors.LightSkyBlue);
                    }
                }
            }
        }

        private void addMovingLine(Point3D? point)
        {
            movingPoints.Clear();
            movingPoints.Add(points[points.Count - 1]);
            movingPoints.Add(point.Value);
            if (movingLines == null)
            {
                movingLines = new LinesVisual3D();
                movingLines.Color = activeEntity.TheColor;
                movingLines.Thickness = 3;
                movingLines.Points = movingPoints;
                viewport.Children.Add(movingLines);
            }
        }

        private void showPoint(Point3D? point)
        {
            if (movingPoint == null)
            {
                movingPoint = new TextVisual3D();
                movingPoint.Foreground = new SolidColorBrush(Colors.White);
                movingPoint.FontSize = 20;
                movingPoint.Height = 2;
                viewport.Children.Add(movingPoint);
            }
            if (point.Value.X < point.Value.Y && point.Value.X < point.Value.Z)
            {
                movingPoint.TextDirection = new Vector3D(0, 1, 0);
                movingPoint.UpDirection = new Vector3D(0, 0, 1);
                movingPoint.Position = new Point3D(point.Value.X + 2, point.Value.Y + 2, point.Value.Z + 2);
            }
            else if (point.Value.Y < point.Value.X && point.Value.Y < point.Value.Z)
            {
                movingPoint.TextDirection = new Vector3D(1, 0, 0);
                movingPoint.UpDirection = new Vector3D(0, 0, 1);
                movingPoint.Position = new Point3D(point.Value.X + 2, point.Value.Y + 2, point.Value.Z + 2);
            }
            else if (point.Value.Z < point.Value.Y && point.Value.Z < point.Value.X)
            {
                movingPoint.TextDirection = new Vector3D(0, 1, 0);
                movingPoint.UpDirection = new Vector3D(-1, 0, 0);
                movingPoint.Position = new Point3D(point.Value.X - 2, point.Value.Y + 2, point.Value.Z + 2);
            }
            movingPoint.Text = "(" + Math.Round(point.Value.Y * xScale, 2) + "," + Math.Round(point.Value.Z * yScale, 2) + "," + Math.Round(point.Value.X * zScale, 2) + ")";
        }

        public override void escDown(object sender, KeyEventArgs e)
        {
            isESCkey = true;
            movingPoints.Clear();
        }

        public override void delDown(object sender, KeyEventArgs e)
        {
            List<PieSliceVisual3D> tempList =  listOfCircles[currentLineEntity.Id];
            PieSliceVisual3D lastCircle = tempList[tempList.Count - 1];
            if (selectedCircles.ContainsKey(lastCircle))
            {
                deleteNodeInfo(lastCircle);
                points.RemoveAt(points.Count - 1);
                if(points.Count >= 1)
                points.RemoveAt(points.Count - 1);
                currentLine.Points = points;
                totalCircles.Remove(lastCircle);
                tempList.Remove(lastCircle);
                viewport.Children.Remove(lastCircle);
            }
        }

        private void deleteNodeInfo(PieSliceVisual3D circle)
        {
            RectangleVisual3D rectangle =  selectedCircles[circle];
            selectedCircles.Remove(circle);
            viewport.Children.Remove(rectangle);
        }

        public override void showCompareResult()
        {
            if (currentLineEntity.Points != null && currentLineEntity.Points.Count >= 2)
            {
                LineComparatorPopUp popUp = new LineComparatorPopUp(compareLineEntities(), currentLineEntity, xScale);
                Grid.SetColumnSpan(popUp, 2);
                Utility.grid.Children.Add(popUp);
            }
        }

        //User cannot go back in line chart.
        //Let there be two mode free mode and line chart mode. 
        //Comparison is not available in free mode.
        //user can only work on a single plane in line chart mode.
        //returns the value of y points at positive integer numbers in a list for each entity.

        public Dictionary<LineEntity, Dictionary<float,float>> compareLineEntities()
        {
            Dictionary<LineEntity, Dictionary<float,float>> result = new Dictionary<LineEntity, Dictionary<float,float>>();
            foreach (LineEntity entity in entities)
            {
                Dictionary<float,float> list = new Dictionary<float,float>();
                Point3DCollection points = entity.Points;
                int lastXPoint = (int)(points[points.Count - 1].Y * xScale);
                Point3DCollection.Enumerator enumerator = points.GetEnumerator();
                enumerator.MoveNext();
                Point3D point1 = enumerator.Current;

                point1.X *= zScale;
                point1.Y *= xScale;
                point1.Z *= yScale;

                enumerator.MoveNext();
                Point3D point2 = enumerator.Current;

                point2.X *= zScale;
                point2.Y *= xScale;
                point2.Z *= yScale;

                double slope = (point2.Z - point1.Z) / (point2.Y - point1.Y);
                int x = 0;
                while (x <= lastXPoint)
                {
                    if (x >= point1.Y && x <= point2.Y)
                    {
                        float y = (float)Math.Round(((slope) * (x - point1.Y) + point1.Z), 2);
                        list.Add(x,y);
                        x = (int)(x + 10*xScale);
                    }
                    else if (x > point2.Y)
                    {
                        //Update points and slope.
                        enumerator.MoveNext();
                        point1 = enumerator.Current;
                        point1.X *= zScale;
                        point1.Y *= xScale;
                        point1.Z *= yScale;
                        enumerator.MoveNext();
                        point2 = enumerator.Current;
                        point2.X *= zScale;
                        point2.Y *= xScale;
                        point2.Z *= yScale;
                        slope = (point2.Z - point1.Z) / (point2.Y - point1.Y);
                    }
                    else if (x < point1.Y)
                    {
                        x = (int)(x + 10 * xScale);
                    }
                }
                result.Add(entity, list);
            }
            return result;
        }
    }
}
