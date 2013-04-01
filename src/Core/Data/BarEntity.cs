using HelixToolkit.Wpf;
using System.Collections.Generic;


namespace FunGraphs3D
{
    public class BarEntity : AbstractEntity
    {
        private List<BoxVisual3D> boxes;

        public List<BoxVisual3D> Boxes
        {
            get
            {
                return boxes;
            }
            set
            {
                boxes = value;
            }
        }

    }
}
