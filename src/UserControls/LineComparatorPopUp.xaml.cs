using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace FunGraphs3D
{
    /// <summary>
    /// Interaction logic for LineComparatorPopUp.xaml
    /// </summary>
    public partial class LineComparatorPopUp : UserControl
    {
        private List<int> list;
        public List<int> entityData{get; set;}
        private Dictionary<LineEntity,List<int>> data;
        public LineComparatorPopUp(Dictionary<LineEntity, List<int>> data, LineEntity entity)
        {
            InitializeComponent();
            this.data = data;
            data.TryGetValue(entity,out list);
            entityData = list;
            DataContext = this;
        }



    }
}
