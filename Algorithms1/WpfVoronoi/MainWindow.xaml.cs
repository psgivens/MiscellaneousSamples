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

namespace WpfVoronoi
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            RedBlackTree rbTree = new RedBlackTree();
            rbTree.Insert(42);
            for (int i = 1; i < 12; i++) {
                rbTree.Insert(i);
                var s = rbTree.ToString();
                Console.WriteLine(s);
            }
            Console.ReadLine();
        }
    }
}
