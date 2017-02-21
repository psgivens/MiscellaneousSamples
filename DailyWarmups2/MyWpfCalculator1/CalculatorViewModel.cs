using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace MyWpfCalculator1
{
    public class CalculatorViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public CalculatorViewModel()
        {
            _buttonPress = new ButtonPressCommand(this);
        }
        
        public void RaisePropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        private string _displayValue;
        public string DisplayValue
        {
            get { return _displayValue; }
            private set 
            { 
                _displayValue = value;
                RaisePropertyChanged("DisplayValue");
            }
        }

        internal void PressButton(string buttonValue)
        {
            DisplayValue += buttonValue;
        }

        public class ButtonPressCommand : ICommand
        {
            private readonly CalculatorViewModel _calculator;
            public ButtonPressCommand(CalculatorViewModel calculator)
            {
                _calculator = calculator;
                if (CanExecuteChanged != null)
                {
                    CanExecuteChanged(this, EventArgs.Empty);
                }
            }

            public bool CanExecute(object parameter)
            {
                return true;
            }

            public event EventHandler CanExecuteChanged;

            public void Execute(object parameter)
            {
                _calculator.PressButton(parameter.ToString());
            }
        }

        private ButtonPressCommand _buttonPress;        
        public ICommand ButtonPress
        {
            get { return _buttonPress; }
        }
    }
        
}
