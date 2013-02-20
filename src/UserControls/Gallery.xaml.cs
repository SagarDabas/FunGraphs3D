using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace FunGraphs3D
{
    /// <summary>
    /// Interaction logic for Gallery.xaml
    /// </summary>
    public partial class Gallery : UserControl
    {
        //For now it is just number. You should represent it as a collection.
        private int galleryData;

        public Gallery()
        {
            InitializeComponent();
        }
       
        //reads the information about the saved charts in the log.
        //log represents the info about all charts like the file location (user directory), name, thumbnail location (application directory).
        private void readData()
        {
            //just for now.
            galleryData = 10;
        }

        private void showList()
        {
            for(int i = 0; i < galleryData ; i ++)
            {

            }
        }
        


    }
}
