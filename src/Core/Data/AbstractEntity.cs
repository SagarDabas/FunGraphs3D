using System;
using System.Windows.Media;
using System.Windows;

namespace FunGraphs3D
{
    public class AbstractEntity : DependencyObject
    {
        public static readonly DependencyProperty TitleProperty = DependencyProperty.Register("Title", typeof(String), typeof(AbstractEntity));
        public static readonly DependencyProperty Label1Property = DependencyProperty.Register("Label1", typeof(String), typeof(AbstractEntity));
        public static readonly DependencyProperty Label2Property = DependencyProperty.Register("Label2", typeof(String), typeof(AbstractEntity));
        public static readonly DependencyProperty Label3Property = DependencyProperty.Register("Label3", typeof(String), typeof(AbstractEntity));
        public static readonly DependencyProperty IdProperty = DependencyProperty.Register("Id", typeof(int), typeof(AbstractEntity));
        public static readonly DependencyProperty TheColorProperty = DependencyProperty.Register("TheColor", typeof(Color), typeof(AbstractEntity));

        public int Id
        {
            get { return (int)base.GetValue(IdProperty); }
            set { base.SetValue(IdProperty, value); }
        }
        public String Title
        {
            get { return (String)base.GetValue(TitleProperty); }
            set { base.SetValue(TitleProperty, value); }
        }
        public String Label1
        {
            get { return (String)base.GetValue(Label1Property); }
            set { base.SetValue(Label1Property, value); }
        }
        public String Label2
        {
            get { return (String)base.GetValue(Label2Property); }
            set { base.SetValue(Label2Property, value); }
        }
        public String Label3
        {
            get { return (String)base.GetValue(Label3Property); }
            set { base.SetValue(Label3Property, value); }
        }
        public Color TheColor
        {
            get { return (Color)base.GetValue(TheColorProperty); }
            set { base.SetValue(TheColorProperty, value); }
        }
    }
}
