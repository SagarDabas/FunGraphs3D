using System;
using System.Windows.Media.Media3D;
using System.Windows.Media;
using HelixToolkit.Wpf;
using System.Windows.Input;
using System.Windows;
using System.Collections.Generic;
using System.Windows.Data;

namespace FunGraphs3D
{
    public class BarChartRenderer: AbstractChartRenderer
    {
        private BoxVisual3D movingBox;
        private BoxVisual3D selectedBox;
        private BarEntity currentBoxEntity;
        private BoxVisual3D boxHovered;
        private bool isOnBox;
        private bool isESCkey;
        private Dictionary<BoxVisual3D, int> totalBoxes;
        private QuadWithLines leftBox;
        private QuadWithLines rightBox;
        private QuadWithLines frontBox;
        private QuadWithLines topBox;
        private QuadWithLines selectedQuad;
        private QuadWithLines quadHovered;
        private bool isOnQuad;
        private billBoard panel;

        public BarChartRenderer()
        {
            totalBoxes = new Dictionary<BoxVisual3D, int>();
            addEntity(false, "Dummy", Colors.ForestGreen, "X-Axis", "Y-Axis");
        }

       public override void changeActiveEntity(AbstractEntity box)
       {
           currentBoxEntity = (BarEntity)box;
           activeEntity = currentBoxEntity;
       }
       public override void delDown(object sender, KeyEventArgs e)
       {
           if (selectedBox != null)
           {
               totalBoxes.Remove(selectedBox);
               viewport.Children.Remove(selectedBox);
               removeRectangles();
               if (panel.panelRect != null)
               {
                   viewport.Children.Remove(panel.panelRect);
               }
               selectedQuad = null;
               quadHovered = null;
               selectedBox = null;
           }
       }

       public override void addEntity(bool editFlag, String title, Color color, params String[] labels)
       { 
           if (!editFlag)
            {
                currentBoxEntity = new BarEntity();
                activeEntity = currentBoxEntity;
                entities.Add(currentBoxEntity);
                //new allocations for the new entity.
                currentBoxEntity.Id = entities.Count;
            }
           if (currentBoxEntity != null)
           {
               currentBoxEntity.Title = title;
               currentBoxEntity.TheColor = color;
               currentBoxEntity.Label1 = labels[0];
               currentBoxEntity.Label2 = labels[1];
           }
            if (selectedBox != null)
            {
                //if a box is selected then donot change its color when edited.
                selectedBox.SetCurrentValue(BoxVisual3D.FillProperty, new SolidColorBrush(Colors.White));
            }
       }

        public override void initViewport(object sender, RoutedEventArgs e)
        {
            CameraController controller = viewport.CameraController;
            controller.CameraTarget = new Point3D(0, 40, 20);
        }
        
        public override void mouseDown(object sender, MouseButtonEventArgs e)
        {
            //check if the mouse is not on a node, then only insert a new node.
            if (!isOnBox && !isOnQuad)
            {
                //start making movingBox.
                isESCkey = false;
                //deselect the selected Box
                deselectBox(selectedBox);
                //add Box.
                movingBox = null;
            }
            else if (selectedQuad != null && selectedQuad.Transform != null)
            {
                updateBox();
            }
            else
            {
                //if mouse is on a node, then select or deselect that node.
                selectNode();
            }
        }


        private void updateBox()
        {
            TranslateTransform3D transform = (TranslateTransform3D)selectedQuad.Transform;
            double centerX = transform.OffsetX / 2;
            double centerY = transform.OffsetY / 2;
            double centerZ = transform.OffsetZ / 2;
            selectedBox.Length = selectedBox.Length + transform.OffsetX;
            selectedBox.Width = selectedBox.Width + transform.OffsetY;
            selectedBox.Height = selectedBox.Height + transform.OffsetZ;
            selectedBox.Center = new Point3D(selectedBox.Center.X + centerX, selectedBox.Center.Y + centerY, selectedBox.Center.Z + centerZ);
            removeRectangles();
            addRectangles();
            selectedQuad = null;
        }

        //when selected create four rectangles around the box along the directions in which they can be dragged or moved.
        //select the particular rectangle by clicking on it, then move it in just one direction by using mouseMove.
        //to deselect rect press escape.
        //to deselect a box click anywhere except the box
        private void selectNode()
        {
            if (!isOnQuad)
            {
                if (selectedBox == null || selectedBox != boxHovered)
                {
                    if (selectedBox != null)
                    {
                        deselectBox(selectedBox);
                    }
                    selectedBox = boxHovered;
                    addRectangles();
                }
                else
                {
                    deselectBox(selectedBox);
                }
                isOnBox = false;
            }
            else
            {
                //quad or box is hovered only if none of the quad is selected.
                selectedQuad = quadHovered;
            }
        }

        private class QuadWithLines : QuadVisual3D
        {
            private LinesVisual3D line;
            private Point3DCollection points;
            private TranslateTransform3D transform;
            BarChartRenderer renderer;
            //Unlike Java C# does not have implicit reference to the  outer class.
            public  QuadWithLines(BarChartRenderer renderer)
            {
                this.renderer = renderer;
                line = new LinesVisual3D();
                points = new Point3DCollection();
                line.Points = points;
                line.Thickness = 2;
                BarEntity temp = (BarEntity)renderer.entities[renderer.totalBoxes[renderer.selectedBox] - 1];
                Binding colorBinding = new Binding("TheColor");
                colorBinding.Converter = new ColorToSolidColorBrushValueConverter();
                colorBinding.Source = temp;
                BindingOperations.SetBinding(this, BoxVisual3D.FillProperty, colorBinding);
                renderer.viewport.Children.Add(line);
            }

            public void removeLine()
            {
                renderer.viewport.Children.Remove(line);
            }
            public void setLinePoints()
            {
                points.Add(this.Point1);
                points.Add(this.Point1);
                points.Add(this.Point2);
                points.Add(this.Point2);
                points.Add(this.Point3);
                points.Add(this.Point3);
                points.Add(this.Point4);
                points.Add(this.Point4);
            }
            public void updateLine(TranslateTransform3D transform)
            {
                renderer.drawBillBoard(transform);
                this.transform = transform;
                points[1] = getPoint(this.Point1);
                points[3] = getPoint(this.Point2);
                points[5] = getPoint(this.Point3);
                points[7] = getPoint(this.Point4);
            }

            private Point3D getPoint(Point3D point)
            {
                return new Point3D(point.X + transform.OffsetX, point.Y + transform.OffsetY, point.Z + transform.OffsetZ);
            }
        }

        private void addRectangles()
        {
            drawBillBoard(null);
            leftBox = new QuadWithLines(this);
            rightBox = new QuadWithLines(this);
            frontBox = new QuadWithLines(this);
            topBox = new QuadWithLines(this);
            
            Point3D point = selectedBox.Center;
            if ((point.X - (selectedBox.Length / 2)) == 0)
            {
                rightBox.Point1 = new Point3D(point.X + selectedBox.Length / 2, point.Y + selectedBox.Width / 2 + 0.1, point.Z + selectedBox.Height / 2);
                rightBox.Point2 = new Point3D(point.X - selectedBox.Length / 2, point.Y + selectedBox.Width / 2 + 0.1, point.Z + selectedBox.Height / 2);
                rightBox.Point3 = new Point3D(point.X - selectedBox.Length / 2, point.Y + selectedBox.Width / 2 + 0.1, point.Z - selectedBox.Height / 2);
                rightBox.Point4 = new Point3D(point.X + selectedBox.Length / 2, point.Y + selectedBox.Width / 2 + 0.1, point.Z - selectedBox.Height / 2);
                                
                leftBox.Point1 = new Point3D(point.X + selectedBox.Length / 2, point.Y - selectedBox.Width / 2 - 0.1, point.Z + selectedBox.Height / 2);
                leftBox.Point2 = new Point3D(point.X - selectedBox.Length / 2, point.Y - selectedBox.Width / 2 - 0.1, point.Z + selectedBox.Height / 2);
                leftBox.Point3 = new Point3D(point.X - selectedBox.Length / 2, point.Y - selectedBox.Width / 2 - 0.1, point.Z - selectedBox.Height / 2);
                leftBox.Point4 = new Point3D(point.X + selectedBox.Length / 2, point.Y - selectedBox.Width / 2 - 0.1, point.Z - selectedBox.Height / 2);
                
                frontBox.Point1 = new Point3D(point.X + selectedBox.Length / 2 + 0.1, point.Y - selectedBox.Width / 2, point.Z + selectedBox.Height / 2);
                frontBox.Point2 = new Point3D(point.X + selectedBox.Length / 2 + 0.1, point.Y + selectedBox.Width / 2, point.Z + selectedBox.Height / 2);
                frontBox.Point3 = new Point3D(point.X + selectedBox.Length / 2 + 0.1, point.Y + selectedBox.Width / 2, point.Z - selectedBox.Height / 2);
                frontBox.Point4 = new Point3D(point.X + selectedBox.Length / 2 + 0.1, point.Y - selectedBox.Width / 2, point.Z - selectedBox.Height / 2);
               
            }
            else if ((point.Y - (selectedBox.Width / 2) == 0))
            {
                rightBox.Point1 = new Point3D(point.X - selectedBox.Length / 2 - 0.1, point.Y - selectedBox.Width / 2, point.Z + selectedBox.Height / 2);
                rightBox.Point2 = new Point3D(point.X - selectedBox.Length / 2 - 0.1, point.Y + selectedBox.Width / 2, point.Z + selectedBox.Height / 2);
                rightBox.Point3 = new Point3D(point.X - selectedBox.Length / 2 - 0.1, point.Y + selectedBox.Width / 2, point.Z - selectedBox.Height / 2);
                rightBox.Point4 = new Point3D(point.X - selectedBox.Length / 2 - 0.1, point.Y - selectedBox.Width / 2, point.Z - selectedBox.Height / 2);

                frontBox.Point1 = new Point3D(point.X + selectedBox.Length / 2, point.Y + selectedBox.Width / 2 + 0.1, point.Z + selectedBox.Height / 2);
                frontBox.Point2 = new Point3D(point.X - selectedBox.Length / 2, point.Y + selectedBox.Width / 2 + 0.1, point.Z + selectedBox.Height / 2);
                frontBox.Point3 = new Point3D(point.X - selectedBox.Length / 2, point.Y + selectedBox.Width / 2 + 0.1, point.Z - selectedBox.Height / 2);
                frontBox.Point4 = new Point3D(point.X + selectedBox.Length / 2, point.Y + selectedBox.Width / 2 + 0.1, point.Z - selectedBox.Height / 2);

                leftBox.Point1 = new Point3D(point.X + selectedBox.Length / 2 + 0.1, point.Y - selectedBox.Width / 2, point.Z + selectedBox.Height / 2);
                leftBox.Point2 = new Point3D(point.X + selectedBox.Length / 2 + 0.1, point.Y + selectedBox.Width / 2, point.Z + selectedBox.Height / 2);
                leftBox.Point3 = new Point3D(point.X + selectedBox.Length / 2 + 0.1, point.Y + selectedBox.Width / 2, point.Z - selectedBox.Height / 2);
                leftBox.Point4 = new Point3D(point.X + selectedBox.Length / 2 + 0.1, point.Y - selectedBox.Width / 2, point.Z - selectedBox.Height / 2);
            }


            topBox.Point1 = new Point3D(point.X - selectedBox.Length / 2, point.Y - selectedBox.Width / 2, point.Z + selectedBox.Height / 2 + 0.1);
            topBox.Point2 = new Point3D(point.X - selectedBox.Length / 2, point.Y + selectedBox.Width / 2, point.Z + selectedBox.Height / 2 + 0.1);
            topBox.Point3 = new Point3D(point.X + selectedBox.Length / 2, point.Y + selectedBox.Width / 2, point.Z + selectedBox.Height / 2 + 0.1);
            topBox.Point4 = new Point3D(point.X + selectedBox.Length / 2, point.Y - selectedBox.Width / 2, point.Z + selectedBox.Height / 2 + 0.1);
            
           
            leftBox.setLinePoints();
            rightBox.setLinePoints();
            topBox.setLinePoints();
            frontBox.setLinePoints();
            viewport.Children.Add(leftBox);
            viewport.Children.Add(rightBox);
            viewport.Children.Add(topBox);
            viewport.Children.Add(frontBox);
        }

        private void deselectBox(BoxVisual3D tempBox)
        {
            if (tempBox != null)
            {
                int id = totalBoxes[tempBox];
                tempBox.SetCurrentValue(BoxVisual3D.FillProperty, new SolidColorBrush(entities[id - 1].TheColor));
                if (tempBox == selectedBox)
                {
                    removeRectangles();
                    if (panel.panelRect != null)
                    {
                        viewport.Children.Remove(panel.panelRect);
                    }
                    selectedBox = null;
                    panel.panelRect = null;
                }
                tempBox = null;
            }
        }


        private void removeRectangles()
        {
            leftBox.removeLine();
            rightBox.removeLine();
            topBox.removeLine();
            frontBox.removeLine();
            viewport.Children.Remove(leftBox);
            viewport.Children.Remove(rightBox);
            viewport.Children.Remove(topBox);
            viewport.Children.Remove(frontBox);
        }

        public override void mouseMove(object sender, MouseEventArgs e)
        {
            if (!isESCkey)
            {
                if (movingBox == null)
                {
                    movingBox = new BoxVisual3D();
                    totalBoxes.Add(movingBox,currentBoxEntity.Id);
                    //since binding cannot be modified so a new binding is created each time.
                    //and it is commonsense , you stupid
                    Binding colorBinding = new Binding("TheColor");
                    colorBinding.Converter = new ColorToSolidColorBrushValueConverter();
                    colorBinding.Source = currentBoxEntity;
                    BindingOperations.SetBinding(movingBox, BoxVisual3D.FillProperty, colorBinding);
                    movingBox.Width = 2;
                    movingBox.Height = 10;
                    movingBox.Length = 2;
                    viewport.Children.Add(movingBox);
                }
                else
                {
                    var point = viewport.FindNearestPoint(e.GetPosition(viewport));
                    var visual = viewport.FindNearestVisual(e.GetPosition(viewport));

                    if (visual!=null && point.HasValue && visual.GetType() == typeof(RectangleVisual3D))
                    {
                        if (point.Value.X < point.Value.Y && point.Value.X < point.Value.Z)
                        {
                            movingBox.Center = new Point3D((int)movingBox.Length / 2, even((int)point.Value.Y) + 1, (int)movingBox.Height / 2);
                        }
                        else if (point.Value.Y < point.Value.X && point.Value.Y < point.Value.Z)
                        {

                            movingBox.Center = new Point3D(even((int)point.Value.X) + 1, (int)movingBox.Length / 2, (int)movingBox.Height / 2);
                        }
                    }
                }
            }
            else if (selectedQuad == null)
            {
                var visual = viewport.FindNearestVisual(e.GetPosition(viewport));
                if (visual != null)
                {
                    if (visual.GetType() == typeof(BoxVisual3D) && !isOnBox)
                    {
                        isOnBox = true;
                        boxHovered = (BoxVisual3D)visual;
                        if (selectedBox == null || selectedBox != boxHovered)
                        {
                            //by programmatically changing the value, binding is lost, so instead this method is used.
                            boxHovered.SetCurrentValue(BoxVisual3D.FillProperty, new SolidColorBrush(Colors.White));
                        }
                    }
                    else if (visual.GetType() != typeof(BoxVisual3D) && visual.GetType() != typeof(QuadVisual3D) && isOnBox)
                    {
                        isOnBox = false;
                        if (selectedBox == null || selectedBox != boxHovered)
                        {
                            deselectBox(boxHovered);
                            boxHovered = null;
                        }
                    }
                    else if (visual.GetType() == typeof(BoxVisual3D)  && isOnBox)
                    {
                        //if new boxhovered that is visual is not equal to the previous hovered box then deselect the previous
                        //and select the new.
                        if (visual != boxHovered)
                        {
                            if (selectedBox == null || selectedBox != boxHovered)
                            {
                                //remove previous hovered box
                                deselectBox(boxHovered);
                                //set the new hovered box
                                boxHovered = (BoxVisual3D)visual;
                                boxHovered.SetCurrentValue(BoxVisual3D.FillProperty, new SolidColorBrush(Colors.White));
                            }
                        }
                    }
                    else if (visual.GetType() == typeof(QuadWithLines) && !isOnQuad)
                    {
                        isOnQuad = true;
                        quadHovered = (QuadWithLines)visual;
                        quadHovered.SetCurrentValue(QuadVisual3D.FillProperty, new SolidColorBrush(Colors.White));
                    }
                    else if (visual.GetType() != typeof(QuadWithLines) && isOnQuad)
                    {
                        isOnQuad = false;
                        int id = totalBoxes[selectedBox];
                        quadHovered.SetCurrentValue(BoxVisual3D.FillProperty, new SolidColorBrush(entities[id - 1].TheColor));
                        quadHovered = null;
                    }
                    else if (visual.GetType() == typeof(QuadWithLines) && isOnQuad)
                    {
                        if (visual != quadHovered)
                        {
                            //remove the previous hovered quad
                            int id = totalBoxes[selectedBox];
                            quadHovered.SetCurrentValue(BoxVisual3D.FillProperty, new SolidColorBrush(entities[id - 1].TheColor));
                            //set the new hovered quad
                            quadHovered = (QuadWithLines)visual;
                            quadHovered.SetCurrentValue(QuadVisual3D.FillProperty, new SolidColorBrush(Colors.White));
                        }
                    }
                }
            }
            else
            {
                var point = viewport.FindNearestPoint(e.GetPosition(viewport));
                var visual = viewport.FindNearestVisual(e.GetPosition(viewport));
                TranslateTransform3D transform = new TranslateTransform3D();
                if (visual != null && point.HasValue && visual.GetType() == typeof(RectangleVisual3D))
                {
                    selectedQuad.Transform = transform;
                    if ((selectedBox.Center.X - (selectedBox.Length/2))==0)
                    {
                        if (selectedQuad == rightBox)
                            transform.OffsetY = even((int)point.Value.Y) - (int)selectedQuad.Point1.Y;
                        else if (selectedQuad == frontBox)
                            transform.OffsetX = even((int)point.Value.X) - (int)selectedQuad.Point1.X;
                        else if (selectedQuad == topBox)
                            transform.OffsetZ = even((int)point.Value.Z) - (int)selectedQuad.Point1.Z;
                    }
                    else if ((selectedBox.Center.Y - (selectedBox.Width / 2) == 0))
                    {
                        if (selectedQuad == leftBox)
                            transform.OffsetX = even((int)point.Value.X) - (int)selectedQuad.Point1.X;
                        else if (selectedQuad == frontBox)
                            transform.OffsetY = even((int)point.Value.Y) - (int)selectedQuad.Point1.Y;
                        else if (selectedQuad == topBox)
                            transform.OffsetZ = even((int)point.Value.Z) - (int)selectedQuad.Point1.Z;
                    }
                    selectedQuad.updateLine(transform);
                }
            }
        }


        private struct billBoard
        {
            public RectangleVisual3D panelRect;
            public TextVisual3D entityName;
            public TextVisual3D length;
            public TextVisual3D width;
            public TextVisual3D height;
        }
        private void drawBillBoard(TranslateTransform3D transform)
        {
           
            if (panel.panelRect == null)
            {
                panel.panelRect = new RectangleVisual3D();
                panel.entityName = new TextVisual3D();
                panel.length = new TextVisual3D();
                panel.width = new TextVisual3D();
                panel.height = new TextVisual3D();

                panel.panelRect.Width = 8;
                panel.panelRect.Length = currentBoxEntity.Title.Length + 5;
                panel.panelRect.Normal = new Vector3D(1, 0, 0);

                Binding colorBinding = new Binding("TheColor");
                colorBinding.Converter = new ColorToSolidColorBrushValueConverter();
                colorBinding.Source = entities[totalBoxes[selectedBox] - 1];
                BindingOperations.SetBinding(panel.panelRect, RectangleVisual3D.FillProperty, colorBinding);
                panel.panelRect.LengthDirection = panel.length.TextDirection = panel.height.TextDirection = panel.width.TextDirection = panel.entityName.TextDirection = new Vector3D(0, 1, 0);
                panel.length.UpDirection = panel.height.UpDirection = panel.width.UpDirection = panel.entityName.UpDirection = new Vector3D(0, 0, 1);
                panel.entityName.Text = currentBoxEntity.Title;
                panel.entityName.FontSize = panel.length.FontSize = panel.height.FontSize = panel.width.FontSize = 30;
                panel.entityName.FontWeight = FontWeights.Bold;
                panel.length.Height = panel.entityName.Height = panel.width.Height = panel.height.Height = 2; 
                panel.panelRect.Children.Add(panel.entityName);
                panel.panelRect.Children.Add(panel.length);
                panel.panelRect.Children.Add(panel.width);
                panel.panelRect.Children.Add(panel.height);
                viewport.Children.Add(panel.panelRect);
            }
            if (selectedQuad == null)
            {
                panel.panelRect.Origin = new Point3D(selectedBox.Center.X + selectedBox.Length / 2 + 1, selectedBox.Center.Y, selectedBox.Height + 10);
                panel.length.Position = new Point3D(selectedBox.Center.X + selectedBox.Length / 2 + 1.1, selectedBox.Center.Y - panel.panelRect.Length / 2 + 4, selectedBox.Height + 11);
                panel.width.Position = new Point3D(selectedBox.Center.X + selectedBox.Length / 2 + 1.1, selectedBox.Center.Y - panel.panelRect.Length / 2 + 4, selectedBox.Height + 9);
                panel.height.Position = new Point3D(selectedBox.Center.X + selectedBox.Length / 2 + 1.1, selectedBox.Center.Y - panel.panelRect.Length / 2 + 4, selectedBox.Height + 7);
                panel.entityName.Position = new Point3D(selectedBox.Center.X + selectedBox.Length / 2 + 1.1, selectedBox.Center.Y - panel.panelRect.Length / 2 + 4, selectedBox.Height + 13);
                panel.length.Text = "Length : " +(selectedBox.Length);
                panel.width.Text = "Width : "+(selectedBox.Width);
                panel.height.Text = "Height : "+(selectedBox.Height);
            }
            else if (transform != null)
            {
                panel.panelRect.Origin = new Point3D(selectedBox.Center.X + selectedBox.Length / 2 + transform.OffsetX / 2 + 1,
                    selectedBox.Center.Y + transform.OffsetY / 2,
                    selectedBox.Height + 10 + transform.OffsetZ / 2);
                panel.length.Position = new Point3D(selectedBox.Center.X + selectedBox.Length / 2 + 1.1 + transform.OffsetX / 2,
                    selectedBox.Center.Y + transform.OffsetY / 2 - panel.panelRect.Length/2 + 4,
                    selectedBox.Height + 11 + transform.OffsetZ / 2);
                panel.width.Position = new Point3D(selectedBox.Center.X + selectedBox.Length / 2 + 1.1 + transform.OffsetX / 2,
                    selectedBox.Center.Y + transform.OffsetY / 2 - panel.panelRect.Length / 2 + 4,
                    selectedBox.Height + 9 + transform.OffsetZ / 2);
                panel.height.Position = new Point3D(selectedBox.Center.X + selectedBox.Length / 2 + 1.1 + transform.OffsetX / 2,
                    selectedBox.Center.Y + transform.OffsetY / 2 - panel.panelRect.Length / 2 + 4,
                    selectedBox.Height + 7 + transform.OffsetZ / 2);
                panel.entityName.Position = new Point3D(selectedBox.Center.X + selectedBox.Length / 2 + 1.1 + transform.OffsetX / 2,
                    selectedBox.Center.Y + transform.OffsetY / 2 - panel.panelRect.Length / 2 + 4,
                    selectedBox.Height + 13 + transform.OffsetZ / 2);
                panel.length.Text = "Length : " + ((selectedBox.Length) + transform.OffsetX);
                panel.width.Text = "Width : " + ((selectedBox.Width) + transform.OffsetY);
                panel.height.Text = "Height : " + ((selectedBox.Height) + transform.OffsetZ);
            }
        }

        private int even(int x)
        {
            if (x % 2 != 0)
                return x + 1;
            else
                return x;
        }

        public override void escDown(object sender, KeyEventArgs e)
        {
            if (selectedQuad == null)
            {
                isESCkey = true;
                viewport.Children.Remove(movingBox);
            }
            else
            {
                //send it back to its original place
                int id = totalBoxes[selectedBox];
                selectedQuad.SetCurrentValue(BoxVisual3D.FillProperty, new SolidColorBrush(entities[id - 1].TheColor));
                TranslateTransform3D transform = ((TranslateTransform3D)selectedQuad.Transform);
                transform.OffsetX = 0;
                transform.OffsetY = 0;
                transform.OffsetZ = 0;
                selectedQuad.updateLine(transform);
                selectedQuad = null;
            }

        }

        public override void showCompareResult()
        {
            throw new NotImplementedException();
        }     
    }
}
