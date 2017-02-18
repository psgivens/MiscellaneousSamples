using System;
using System.Collections.Generic;
using System.ComponentModel;
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

namespace SampleExplorer
{
    /// <summary>
    /// Interaction logic for InsightExplorer.xaml
    /// </summary>
    public partial class InsightExplorer : UserControl
    {
        public InsightExplorer()
        {
            InitializeComponent();
            DataContext = new ApplicationViewModel();// new string[] { "Some one", "Just another" };
        }
    }

    public class ApplicationViewModel : INotifyPropertyChanged
    {
        public ApplicationViewModel()
        {
            Application = "Emailer";
            Environment = "JNS";
        }

        private string _application;

        public string Application
        {
            get { return _application; }
            set
            {
                _application = value;
                RaiseChange("Application");
            }
        }

        private string _environment;

        public string Environment
        {
            get { return _environment; }
            set
            {
                _environment = value;
                RaiseChange("Environment");
            }
        }

        private void RaiseChange(string name)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(name));
        }

        public IEnumerable<MenuItemViewModel> Applications
        {
            get
            {
                return from value in new string[] { "Emailer", "Tracker", "Analyzer", "Searcher" }
                       select new ApplicationMenuItemViewModel(this, value);
            }
        }

        public IEnumerable<MenuItemViewModel> Environments
        {
            get
            {
                return from value in new string[] { "Dev", "QA", "Preprod", "MIA", "JNS" }
                       select new EnvironmentMenuItemViewModel(this, value);
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }

    public abstract class MenuItemViewModel : ICommand
    {
        protected ApplicationViewModel Application { get; private set; }
        public string Title { get; private set; }
        public MenuItemViewModel(ApplicationViewModel application, string title)
        {
            this.Application = application;
            this.Title = title;
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public event EventHandler CanExecuteChanged;

        public abstract void Execute(object parameter);
    }

    public class ApplicationMenuItemViewModel : MenuItemViewModel
    {
        public ApplicationMenuItemViewModel(ApplicationViewModel application, string title)
            : base(application, title) { }

        public override void Execute(object parameter)
        {
            Application.Application = Title;
        }
    }
    public class EnvironmentMenuItemViewModel : MenuItemViewModel
    {
        public EnvironmentMenuItemViewModel(ApplicationViewModel application, string title)
            : base(application, title)
        { }

        public override void Execute(object parameter)
        {
            Application.Environment = Title;
        }
    }

}
