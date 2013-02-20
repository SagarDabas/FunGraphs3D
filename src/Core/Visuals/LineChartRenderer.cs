using System;
using System.Windows.Media.Media3D;
using System.Windows.Media;
using HelixToolkit.Wpf;
using System.Collections.Generic;
using System.Windows.Input;
using System.Windows;
using System.Windows.Controls;

namespace FunGraphs3D
{
    class LineChartRenderer : AbstractChartRenderer
    {
        private LineEntity activeEntity;
        private Dictionary<PieSliceVisual3D, RectangleVisual3D> selectedCircles;
        private TextVisual3D movingPoint;
        private PieSliceVisual3D lastCircle;
        private bool isESCkey;
        private bool isCircleSelected ;
        private PieSliceVisual3D circleSelected;
        private LinesVisual3D lines;
        private Point3DCollection points;
        private LinesVisual3D movingLines;
        private Point3DCollection movingPoints;
        private Dictionary<PieSliceVisual3D, int> totalCircles;

        public LineChartRenderer()
        {
            //add a default entity
            addEntity("Dummy", Colors.Black);
            selectedCircles = new Dictionary<PieSliceVisual3D, RectangleVisual3D>();
            totalCircles = new Dictionary<PieSliceVisual3D, int>();
            points = new Point3DCollection();
            movingPoints = new Point3DCollection();
        }

        public override void addEntity(String title, Color color)
        {
            activeEntity = new LineEntity();
            activeEntity.Id = entities.Count + 1;
            activeEntity.Title = title;
            activeEntity.TheColor = color;
            entities.Add(activeEntity);

            //new allocations for the new entity.
            points = new Point3DCollection();
            movingPoints = new Point3DCollection();
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
                 //check if the mouse is not on a node, then only insert a new node.
                if (!isCircleSelected)
                {
                    //start making moving lines.
                    isESCkey = false;

                    //add point to the points collection for line rendering.
                    points.Add(point.Value);
                    //add to the LineEntity list.
                    activeEntity.Point = point.Value;
                    //lines need a pair of point to render.
                    if (points.Count % 2 == 0)
                    {
                        points.Add(point.Value);
                        //add to the LineEntity list.
                        activeEntity.Point = point.Value;
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
            if (!selectedCircles.ContainsKey(circleSelected))
            {
                //make the selectedCircle entity as the currect active entity for drawing the info of the selected node.
                //drawNodeInfo checks the activeEntity for drawing its points.
                int id;
                totalCircles.TryGetValue(circleSelected, out id);
                activeEntity = (LineEntity)entities[id-1];
                selectedCircles.Add(circleSelected,drawNodeInfo(point));
                //make the last circle's entity as the current active entity.
                totalCircles.TryGetValue(lastCircle, out id);
                activeEntity = (LineEntity)entities[id - 1];
            }
            else
            {
                RectangleVisual3D rectangle;
                selectedCircles.TryGetValue(circleSelected, out rectangle);
                selectedCircles.Remove(circleSelected);
                viewport.Children.Remove(rectangle);
            }
            isCircleSelected = false;
        }

        private RectangleVisual3D drawNodeInfo(Point3D? point)
        {
            RectangleVisual3D panelRect = new RectangleVisual3D();
            RectangleVisual3D colorRect = new RectangleVisual3D();
            TextVisual3D entityName = new TextVisual3D();
            TextVisual3D myPoint = new TextVisual3D();
            panelRect.Length = activeEntity.Title.Length + 10;
            panelRect.Width = 7;
            panelRect.Fill = new SolidColorBrush(activeEntity.TheColor);


            if (point.Value.X < point.Value.Y && point.Value.X < point.Value.Z)
            {
                panelRect.Origin = new Point3D(point.Value.X + 3.9, point.Value.Y + panelRect.Length/3, point.Value.Z + 1 + panelRect.Width/2 );
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
            myPoint.Text = "(" + (int)point.Value.Y + "," + (int)point.Value.Z + "," + (int)point.Value.X + ")";
            entityName.Text = activeEntity.Title;

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


        private void addCircle(Point3D? point)
        {
            PieSliceVisual3D circle = new PieSliceVisual3D();
            lastCircle = circle;
            totalCircles.Add(circle,activeEntity.Id);
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
            lines = new LinesVisual3D();
            lines.Color = activeEntity.TheColor;
            lines.Thickness = 3;
            lines.IsRendering = true;
            lines.Points = points;
            viewport.Children.Add(lines);
        }

       
        public override void mouseMove(object sender, MouseEventArgs e)
        {
            var point = viewport.FindNearestPoint(e.GetPosition(viewport));
            if (point.HasValue)
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
                //stops making lines. user can select a node now.
                selectNode(e);
            }
        }

        private void selectNode(MouseEventArgs e)
        {
            var visual = viewport.FindNearestVisual(e.GetPosition(viewport));
            if (visual != null)
            {
                if (visual.GetType() == typeof(PieSliceVisual3D) && !isCircleSelected)
                {
                    circleSelected = (PieSliceVisual3D)visual;
                    isCircleSelected = true;
                    if (!selectedCircles.ContainsKey(circleSelected))
                    {
                        circleSelected.OuterRadius = 1;
                        circleSelected.Fill = new SolidColorBrush(Colors.White);
                    }
                }
                else if (visual.GetType() != typeof(PieSliceVisual3D) && isCircleSelected)
                {
                    isCircleSelected = false;
                    if (!selectedCircles.ContainsKey(circleSelected))
                    {
                        circleSelected.OuterRadius = 0.5;
                        circleSelected.Fill = new SolidColorBrush(Colors.LightSkyBlue);
                    }
                }
            }
        }

        private void addMovingLine(Point3D? point)
        {
            movingPoints.Clear();
            movingPoints.Add(points[points.Count - 1]);
            movingPoints.Add(point.Value);
            movingLines = new LinesVisual3D();
            movingLines.Color = activeEntity.TheColor;
            movingLines.Thickness = 3;
            movingLines.IsRendering = true;
            movingLines.Points = movingPoints;
            viewport.Children.Add(movingLines);
        }

        private void showPoint(Point3D? point)
        {
            if (movingPoint != null)
            {
                viewport.Children.Remove(movingPoint);
            }
            movingPoint = new TextVisual3D();
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
            movingPoint.Text = "(" + (int)point.Value.Y + "," + (int)point.Value.Z + "," + (int)point.Value.X + ")";
            movingPoint.Foreground = new SolidColorBrush(Colors.White);
            movingPoint.FontSize = 20;
            movingPoint.Height = 2;
            viewport.Children.Add(movingPoint);
        }

        public override void escDown(object sender, KeyEventArgs e)
        {
            isESCkey = true;
            //remove the current moving line.
            //removing line does not work but adding an empty line seems to work.
            movingPoints.Clear();
            movingLines = new LinesVisual3D();
            viewport.Children.Add(movingLines);
        }


        //User cannot go back in line chart.
        //Let there be two mode free mode and line chart mode. 
        //Comparison is not available in free mode.
        //user can only work on a single plane in line chart mode.
        //returns the value of y points at positive integer numbers in a list for each entity.

        public static Dictionary<LineEntity,List<int>> compareLineEntities(List<LineEntity> lineEntities)
        {
            Dictionary<LineEntity, List<int>> result = new Dictionary<LineEntity, List<int>>();
            foreach(LineEntity entity in lineEntities)
            {
                List<int> list = new List<int>();
                Point3DCollection points = entity.Points;
                int lastXPoint = (int)points[points.Count - 1].X;

                Point3DCollection.Enumerator enumerator = points.GetEnumerator();
                enumerator.MoveNext();
                Point3D point1 = enumerator.Current;
                enumerator.MoveNext();
                Point3D point2 = enumerator.Current;
                double slope = (point2.X - point1.X) / (point2.Y - point1.Y);
                int x = 0;
                while( x < lastXPoint)
                {
                    if (x > point1.X && x < point2.X)
                    {
                        int y = (int)((slope) * (x - point1.X));
                        list.Add(y);
                        x++;
                    }
                    else if(x > point2.X)
                    {
                        //Update points and slope.
                        enumerator.MoveNext();
                        point1 = enumerator.Current;
                        enumerator.MoveNext();
                        point2 = enumerator.Current;
                        slope = (point2.X - point1.X) / (point2.Y - point1.Y);
                    }
                    else if(x < point1.X)
                    {
                        x++;
                    }
                }
                result.Add(entity,list);
            }
            return result;
        }

        public override void showCompareResult()
        {
            Console.WriteLine("sssaaasas");
            LineComparatorPopUp popUp = new LineComparatorPopUp(LineComparator.compareLineEntities(entities),activeEntity);
            popUp.LinePopUp.IsOpen = true;
            Grid.SetColumn(popUp, 1);
            Grid.SetRow(popUp, 0);
            Utility.grid.Children.Add(popUp);
        }
        
        public override void mouseEnter(object sender, MouseEventArgs e) { }
        public override void mouseLeave(object sender, MouseEventArgs e) { }
        

    }
}
