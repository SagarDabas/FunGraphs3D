using System;
using System.Windows.Media;


namespace FunGraphs3D
{
    public class AbstractEntity
    {
        private String title;
        private Color color;
        private int id;

        public int Id
        {
            get { return id; }
            set { id = value;}
        }
        public String Title
        {
            get
            {
                return title;
            }
            set
            {
                if (value.Length <= 20)
                    title = value;
            }
        }

        public Color TheColor
        {
            get
            {
                return color;
            }
            set
            {
                color = value;
            }
        }
    }
}
