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

namespace project
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        GA ga;
        Player enemy;
        Squad[] army;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            enemy = new Player(true, 4);
            Unit humanKnights = new Unit(2, 4, 17, 3, 5, 14, 25, 1.5f);
            army = new Squad[10];
            for (int i = 0; i < army.Length; i++)
            {
                army[i] = new Squad(humanKnights, humanKnights.MaxAmount);
            }

            ga = new GA(enemy,army);

            ga.Go();
        }
    }
}
