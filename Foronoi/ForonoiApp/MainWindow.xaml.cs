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

namespace ForonoiApp {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {

        private Random rand = new Random(DateTime.Now.Millisecond);
        private const int Size = 5;
        public MainWindow() {
            InitializeComponent();

            for (var i=0; i<20; i++) {
                drawingCanvas.Children.Add(RandomElipse(400, 400));
            }
        }

        private Ellipse RandomElipse(int width, int height) {
            var left = rand.Next(width-Size);
            var top = rand.Next(height-Size);
            var brush = new SolidColorBrush(Color.FromArgb(0xff, 0, 0, 0xff));
            var ellipse = new Ellipse {
                Width = Size,
                Height = Size,
                Stroke = brush,
                Fill = brush
            };
            Canvas.SetTop(ellipse, top);
            Canvas.SetLeft(ellipse, left);
            Console.WriteLine("({0},{1})", left, top);
            return ellipse;
        }
    }
}
