using System;
using System.Windows.Media.Media3D;
using System.Collections.Generic;


namespace FunGraphs3D
{
    class LineComparator
    {


        //User cannot go back in line chart.
        //Let there be two mode free mode and line chart mode. 
        //Comparison is not available in free mode.
        //user can only work on a single plane in line chart mode.
        //returns the value of y points at positive integer numbers in a list for each entity.

        public static Dictionary<LineEntity, List<int>> compareLineEntities(List<AbstractEntity> lineEntities)
        {
            Dictionary<LineEntity, List<int>> result = new Dictionary<LineEntity, List<int>>();
            foreach (LineEntity entity in lineEntities)
            {
                List<int> list = new List<int>();
                Point3DCollection points = entity.Points;
                int lastXPoint = (int)points[points.Count - 1].Y;

                Point3DCollection.Enumerator enumerator = points.GetEnumerator();
                enumerator.MoveNext();
                Point3D point1 = enumerator.Current;
                enumerator.MoveNext();
                Point3D point2 = enumerator.Current;
                double slope = (point2.Z - point1.Z) / (point2.Y - point1.Y);
                int x = 0;
                while (x < lastXPoint)
                {
                    if (x > point1.Y && x < point2.Y)
                    {
                        int y = (int)Math.Round(((slope) * (x - point1.Y) + point1.Z));
                        list.Add(y);
                        Console.WriteLine(x - point1.Y);
                        x++;
                    }
                    else if (x > point2.Y)
                    {
                        //Update points and slope.
                        enumerator.MoveNext();
                        point1 = enumerator.Current;
                        enumerator.MoveNext();
                        point2 = enumerator.Current;
                        slope = (point2.Z - point1.Z) / (point2.Y - point1.Y);
                    }
                    else if (x < point1.Y)
                    {
                        x++;
                    }
                }
                result.Add(entity, list);
            }
            return result;
        }



    }
}
