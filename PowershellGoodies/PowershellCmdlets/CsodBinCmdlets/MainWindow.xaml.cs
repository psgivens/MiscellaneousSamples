using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
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

namespace PowershellCmdlets
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public string UserName
        {
            get { return (string)GetValue(UserNameProperty); }
            set { SetValue(UserNameProperty, value); }
        }

        // Using a DependencyProperty as the backing store for UserName.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty UserNameProperty =
            DependencyProperty.Register("UserName", typeof(string), typeof(MainWindow), new PropertyMetadata(""));

        public string Password { get; set; } = "";
        public System.Net.Cookie Cookie { get; set; }
        public string EncodedCreds { get; private set; }


        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;
        }

        private void pwdBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            Password = ((PasswordBox)sender).Password;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
         

            // TODO: Understand Cookie authentication better. 

            //var url = "https://jira.csod.com/rest/auth/1/session";
            //string json = String.Format("{{ 'username': '{0}', 'password': '{1}' }}", UserName, Password);

            //var httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
            //httpWebRequest.ContentType = "application/json";
            //httpWebRequest.Method = "GET";
            //httpWebRequest.Headers.Add("Authorization", "Basic " + encodedCreds);

            //var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();

            //var cookieString = httpResponse.Headers[HttpResponseHeader.SetCookie];
            //var parts = cookieString.Split(';');
            //string token = string.Empty;
            //string path = string.Empty;
            //foreach (var part in parts)
            //{
            //    var p = part.Split('=');
            //    switch (p[0])
            //    {
            //        case "JSESSIONID":
            //            token = p[1];
            //            break;
            //        case "Path":
            //            path = p[1];
            //            break;
            //    }
            //}
            //Cookie = new System.Net.Cookie("JSESSIONID", token, path);


            Close();
        }

        private void textBox_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                Button_Click(null, null);
            }
        }
    }
}
