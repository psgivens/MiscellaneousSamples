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
using Parakeet.Sessions;
using Parakeet.Presenters;
using Parakeet.Infrastructure;
using Parakeet.DataModel;
using System.Collections.ObjectModel;


namespace Parakeet
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly ApplicationSession applicationSession;
        private readonly SubjectSession messageManager;

        private MainPresenter mainWindowPresenter;
        private Presenter activeThoughtsPresenter;
        private Presenter staleThoughtsPresenter;

        public MainWindow()
        {
            InitializeComponent();
            applicationSession = ApplicationSession.Start(new DispatcherInvoker(Dispatcher));
            messageManager = applicationSession.MessageManager;

            activeThoughtsPresenter =  new CollectionsPresenter<SubjectSession, ObservableCollection<Message>, Message>(
                    messageManager,
                    manager => manager.ActiveThoughts,
                    manager => manager.FocusedActiveThought);

            staleThoughtsPresenter = new CollectionsPresenter<SubjectSession, ObservableCollection<Message>, Message>(
                    messageManager,
                    manager => manager.StaleThoughts,
                    manager => manager.FocusedStaleThought);

            mainWindowPresenter = new MainPresenter()
            {
                Entry = new ThoughtEntryPresenter(messageManager),
            };

            mainWindowPresenter.SetActiveDisplay(activeThoughtsPresenter);   
            messageManager.PropertyChanged += messageManager_PropertyChanged;
            DataContext = mainWindowPresenter;
        }

        private void messageManager_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "ReviewSession")
                mainWindowPresenter.SetActiveDisplay(
                    messageManager.ReviewSession == null
                    ? activeThoughtsPresenter
                    : (Presenter)new ThoughtReviewPresenter(messageManager.ReviewSession));
        }
    }
}
