using System;
using System.Collections.Generic;
using HelixToolkit.Wpf;
using System.Windows.Media.Media3D;
using System.Windows;


namespace FunGraphs3D
{
    class PieSlice3D : PieSliceVisual3D
    {

        /// <summary>
        /// Identifies the <see cref="InnerRadius"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty VerticalLengthProperty = DependencyProperty.Register(
            "VerticalLength", typeof(double), typeof(PieSlice3D), new UIPropertyMetadata(2.0, GeometryChanged));


        /// <summary>
        /// Gets or sets the outer radius.
        /// </summary>
        /// <value>The outer radius.</value>
        public double VerticalLength
        {
            get
            {
                return (double)this.GetValue(VerticalLengthProperty);
            }

            set
            {
                this.SetValue(VerticalLengthProperty, value);
            }
        }

        protected override MeshGeometry3D Tessellate()
        {
            var pts = new List<Point3D>();
            var pts1 = new List<Point3D>();
            var pts2 = new List<Point3D>();
            var pts3 = new List<Point3D>();
            Vector3D pointZ = new Vector3D(0, 0, this.VerticalLength);
            var right = Vector3D.CrossProduct(this.UpVector, this.Normal);
            var b = new MeshBuilder(false, false);

            Point3D temp = new Point3D();
            Point3D temp1 = new Point3D();

            for (int i = 0; i < this.ThetaDiv; i++)
            {
                double angle = this.StartAngle + (this.EndAngle - this.StartAngle) * i / (this.ThetaDiv - 1);
                double angleRad = angle / 180 * Math.PI;
                Vector3D dir = right * Math.Cos(angleRad) + this.UpVector * Math.Sin(angleRad);
                temp = this.Center + dir * this.InnerRadius;
                temp1 = this.Center + dir * this.OuterRadius;

                //for the base arc
                pts.Add(temp);
                pts.Add(temp1);

                //for inner vertical walls
                pts1.Add(temp);
                pts1.Add(temp + pointZ);

                //for outer vertical walls
                pts2.Add(temp1);
                pts2.Add(temp1 + pointZ);

                //for the raised arc
                pts3.Add(temp + pointZ);
                pts3.Add(temp1 + pointZ);

                //for closing
                if (i == 0 || i == this.ThetaDiv - 1)
                {
                    b.AddQuad(temp, temp1, temp1 + pointZ, temp + pointZ);
                }

            }

            b.AddTriangleStrip(pts);
            b.AddTriangleStrip(pts1);
            b.AddTriangleStrip(pts2);
            b.AddTriangleStrip(pts3);

            return b.ToMesh();
        }
    }
}
