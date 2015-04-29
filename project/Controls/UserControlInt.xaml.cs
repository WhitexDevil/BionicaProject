using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace project.Controls
{
    /// <summary>
    /// Interaction logic for UserControlInt.xaml
    /// </summary>
    public partial class UserControlInt : UserControl
    {
        private string prop;




        private int myInt;

        public double MyInt
        {
            get { return myInt; }
            set { myInt = (int)value; }
        }
        
    

        

        public string yProp
        {
            get { return prop; }
            set { prop = value; }
        }

        public UserControlInt()
        {
            InitializeComponent();
        }

        private void Slider1_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (LabelSlider1!=null)
            LabelSlider1.Content = (int)((Slider)sender).Value;
        }
    }
}
