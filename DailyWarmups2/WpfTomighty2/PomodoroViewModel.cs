using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace WpfTomighty
{
    public class PomodoroViewModel : ViewModelBase
    {
        private string _clockDisplay;
        private Timer _timer;
        private DateTime _startTime = DateTime.Now;

        public PomodoroViewModel()
        {
            _timer = new Timer(TimerCallback);
            _clockDisplay = TimeSpan.FromMinutes(25).ToString(@"mm\:ss");
            StartClockCommand = new ActionCommand(StartClock);
        }

        public string ClockDisplay
        {
            get { return _clockDisplay; }
            set
            {
                _clockDisplay = value;
                RaisePropertyChanged("ClockDisplay");                
            }
        }

        private void TimerCallback(object state)
        {
            var currentTime = (TimeSpan.FromMinutes(25) - (DateTime.Now - _startTime));
            ClockDisplay = currentTime.ToString(@"mm\:ss");
        }

        public void StartClock()
        {
            _startTime = DateTime.Now;
            _timer.Change(0, 1000);
        }

        public System.Windows.Input.ICommand StartClockCommand { get; private set; }

        private class ActionCommand : System.Windows.Input.ICommand
        {
            private Action _action;
            public ActionCommand(Action action)
            {
                _action = action;
            }

            bool System.Windows.Input.ICommand.CanExecute(object parameter)
            {
                return true;
            }

            public event EventHandler CanExecuteChanged;

            void System.Windows.Input.ICommand.Execute(object parameter)
            {
                _action();
            }
        }


    }
}
