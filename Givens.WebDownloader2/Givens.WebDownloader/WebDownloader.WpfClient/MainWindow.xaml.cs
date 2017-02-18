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
using WebDownloader.ClientSessions;

namespace WebDownloader.WpfClient
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly ApplicationSession session = new ApplicationSession();
        public MainWindow()
        {
            InitializeComponent();

            DataContext = session;
        }

        private void AddUrlButton_Click(object sender, RoutedEventArgs e)
        {
            session.AddUrls(NewAddress.Text);
        }

        private void DownloadButton_Click(object sender, RoutedEventArgs e)
        {
            session.StartDownloading();
        }

    }
}
