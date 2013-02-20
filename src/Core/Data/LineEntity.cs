using System;
using System.Windows.Media.Media3D;


namespace FunGraphs3D
{
    public class LineEntity : AbstractEntity
    {
        private Point3DCollection points;
        private Point3D point;

        public Point3DCollection Points
        {
            get
            {
                return points;
            }
            set
            {
                points = value;
            }
        }

        public Point3D Point
        {
            set
            {
                point = value;
                if (points != null)
                    points.Add(value);
                else
                {
                    points = new Point3DCollection();
                    points.Add(value);
                }
            }
        }

    }
}
