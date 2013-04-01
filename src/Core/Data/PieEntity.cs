using System.Windows;

namespace FunGraphs3D
{
    class PieEntity : AbstractEntity
    {
        public static readonly DependencyProperty PercentageProperty = DependencyProperty.Register("Percentage", typeof(int), typeof(PieEntity), new PropertyMetadata(100));
        public int Percentage
        {
            get { return (int)base.GetValue(PercentageProperty); }
            set { base.SetValue(PercentageProperty, value); }
        }       
    }
}
