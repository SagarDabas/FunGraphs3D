using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Collections.ObjectModel;


namespace FunGraphs3D
{
    /// <summary>
    /// Interaction logic for SidebarRight.xaml
    /// </summary>
    public partial class SidebarRight : UserControl
    {
        //for(LineEntity entity in entities) // looks like implicit type casting -- Google it.
        private ObservableCollection<AbstractEntity> entities;
        
        public ObservableCollection<AbstractEntity> Entities
        {
            get
            {
                return entities;
            }
            set
            {
                entities = value;
            }
        }
       
        public SidebarRight()
        {
            InitializeComponent();
            DataContext = this;
        }

        private void ListBox_SelectionChanged_1(object sender, SelectionChangedEventArgs e)
        {
            if (listBox.SelectedItem != null)
            {
                Utility.chartRenderer.changeActiveEntity((AbstractEntity)listBox.SelectedItem);
                Utility.sidebar.setDefaultValues();
            }
        }
    }
}
