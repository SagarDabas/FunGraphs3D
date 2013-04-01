using System;
using System.Collections.Generic;
using HelixToolkit.Wpf;
using System.Windows.Media.Media3D;
using System.Windows;


namespace FunGraphs3D
{
    public class AreaChart : QuadVisual3D
    {
        //List id used instead of Point3DCollection because Point3DCollection id freezable therefore it if frozen and it cannot be modified.
        //even the clone is frozen.
        //though now i am tempList and tempList is modified therefore Point3DCollection can also be used here.
        private static List<Point3D> points = new List<Point3D>
        {
            new Point3D(0,0,0),
            new Point3D(0,0,5)
        };

        public static readonly DependencyProperty RenderProperty = DependencyProperty.Register(
             "IsRender", typeof(bool), typeof(AreaChart), new UIPropertyMetadata(false, GeometryChanged));

        public bool IsRender
        {
            get
            {
                return (bool)this.GetValue(RenderProperty);
            }

            set
            {
                this.SetValue(RenderProperty, value);
            }
        }

        /// <summary>
        /// Identifies the <see cref="Extrusion"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ExtrusionProperty = DependencyProperty.Register(
            "Extrusion", typeof(double), typeof(AreaChart), new UIPropertyMetadata(0.0, GeometryChanged));

        public static readonly DependencyProperty PointsListProperty = DependencyProperty.Register(
            "PointsList", typeof(IList<Point3D>), typeof(AreaChart),
            new UIPropertyMetadata(points, GeometryChanged));

        private IList<Point3D> tempList;

        /// <summary>
        /// Gets or sets the extrusion.
        /// </summary>
        /// <value>The extrusion.</value>
        public double Extrusion
        {
            get
            {
                return (double)this.GetValue(ExtrusionProperty);
            }

            set
            {
                this.SetValue(ExtrusionProperty, value);
            }
        }

        public IList<Point3D> PointsList
        {
            get
            { return (IList<Point3D>)this.GetValue(PointsListProperty); }

            set
            { this.SetValue(PointsListProperty, value); }
        }

        protected override MeshGeometry3D Tessellate()
        {
            var builder = new MeshBuilder(false, false);
            //calcProjectedPoints();
            extrudeArea(builder);
            //builder.AddQuad(Point1, Point2, point2, point1);
            return builder.ToMesh();
        }

        private Point3D projPoint(Point3D tempPoint)
        {
            Point3D point1 = new Point3D(1,1,0);
            point1.X = point1.X * tempPoint.X;
            point1.Y = point1.Y * tempPoint.Y;
            point1.Z = point1.Z * tempPoint.Z;
            return point1;
        }

 
        private void extrudeArea(MeshBuilder builder)
        {
            if (IsRender)
            {
                //tempList is used instead PointsList because modifying PointsList would invoke the GeometryChanged method
                this.tempList = new List<Point3D>(PointsList);
                Vector3D extrusionAxis = new Vector3D(1, 0, 0);

                if (Extrusion >= 1)
                {
                    for (int i = 0; i < tempList.Count - 1; i++)
                    {
                        builder.AddQuad(tempList[i], tempList[i + 1], projPoint(tempList[i + 1]), projPoint(tempList[i]));
                        builder.AddQuad(tempList[i] + extrusionAxis * Extrusion, tempList[i + 1] + extrusionAxis * Extrusion,
                            projPoint(tempList[i + 1]) + extrusionAxis * Extrusion, projPoint(tempList[i]) + extrusionAxis * Extrusion);
                    }

                    for (int i = 0; i < tempList.Count; i += 2)
                    {
                        tempList.Insert(i + 1, tempList[i] + extrusionAxis * Extrusion);
                    }

                    tempList.Add(projPoint(tempList[tempList.Count - 2]));
                    tempList.Add(tempList[tempList.Count - 1] + extrusionAxis * Extrusion);
                    tempList.Add(projPoint(tempList[0]));
                    tempList.Add(tempList[tempList.Count - 1] + extrusionAxis * Extrusion);
                    tempList.Add(tempList[0]);
                    tempList.Add(tempList[0] + extrusionAxis * Extrusion);
                    builder.AddTriangleStrip(tempList);
                }
                else
                {
                    for (int i = 0; i < tempList.Count - 1; i++)
                    {
                        builder.AddQuad(tempList[i], tempList[i + 1], projPoint(tempList[i + 1]), projPoint(tempList[i]));
                    }
                }
            }
        }
    }
}
