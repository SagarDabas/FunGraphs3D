using System;
using System.Collections.Generic;
using HelixToolkit.Wpf;
using System.Windows.Media.Media3D;
using System.Windows;


namespace FunGraphs3D
{
    class AreaChart : QuadVisual3D
    {
        private Point3D point1;
        private Point3D point2;

        /// <summary>
        /// Identifies the <see cref="OnAxis"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty OnAxisProperty = DependencyProperty.Register(
            "OnAxis", typeof(Vector3D), typeof(AreaChart), new UIPropertyMetadata(new Vector3D(0, 1, 0), GeometryChanged));


        /// <summary>
        /// Identifies the <see cref="Extrusion"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ExtrusionProperty = DependencyProperty.Register(
            "Extrusion", typeof(double), typeof(AreaChart), new UIPropertyMetadata(0.0, GeometryChanged));



        /// <summary>
        /// Gets or sets the On Axis.
        /// </summary>
        /// <value>The On Axis</value>
        public Vector3D OnAxis
        {
            get
            {
                return (Vector3D)this.GetValue(OnAxisProperty);
            }

            set
            {
                this.SetValue(OnAxisProperty, value);
            }
        }

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


        protected override MeshGeometry3D Tessellate()
        {
            var builder = new MeshBuilder(false, false);
            calcProjectedPoints();
            extrudeArea(builder);
            builder.AddQuad(Point1, Point2, point2, point1);
            return builder.ToMesh();
        }

        private void calcProjectedPoints()
        {
            point1 = (Point3D)OnAxis;
            point1.X = point1.X * Point1.X;
            point1.Y = point1.Y * Point1.Y;
            point1.Z = point1.Z * Point1.Z;

            point2 = (Point3D)OnAxis;
            point2.X = point2.X * Point2.X;
            point2.Y = point2.Y * Point2.Y;
            point2.Z = point2.Z * Point2.Z;
        }


        private void extrudeArea(MeshBuilder builder)
        {
            Vector3D extrusionAxis = new Vector3D(1, 1, 1);
            int count = 0;

            if (Point1.X == 0 && Point2.X == 0)
            {
                extrusionAxis = new Vector3D(1, 0, 0);
                count++;
            }
            if (Point1.Y == 0 && Point2.Y == 0)
            {
                extrusionAxis = new Vector3D(0, 1, 0);
                count++;
            }
            if (Point1.Z == 0 && Point2.Z == 0)
            {
                extrusionAxis = new Vector3D(0, 0, 1);
                count++;
            }


            if (count >= 2 && count < 1)
            {
                Console.WriteLine("No Area");
            }
            else
            {
                var list = new List<Point3D>();
                list.Add(Point1);
                list.Add(Point1 + extrusionAxis * Extrusion);
                list.Add(Point2);
                list.Add(Point2 + extrusionAxis * Extrusion);
                list.Add(point2);
                list.Add(point2 + extrusionAxis * Extrusion);
                list.Add(point1);
                list.Add(point1 + extrusionAxis * Extrusion);
                list.Add(Point1);
                list.Add(Point1 + extrusionAxis * Extrusion);
                builder.AddQuad(Point1 + extrusionAxis * Extrusion, Point2 + extrusionAxis * Extrusion, point2 + extrusionAxis * Extrusion, point1 + extrusionAxis * Extrusion);
                builder.AddTriangleStrip(list);
            }

        }


    }
}
