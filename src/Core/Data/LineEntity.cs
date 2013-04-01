using System;
using System.Windows.Media.Media3D;


namespace FunGraphs3D
{
    public class LineEntity : AbstractEntity
    {
        private Point3DCollection points;
       
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

    }
}
