using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace MyWpfCalculator2
{
    public class CalculatorViewModel : ViewModelBase
    {
        private string _displayValue;
        private readonly KeyPressedCommand _keyPressed;
        private bool _isValue;
        private int? _tempValue;
        private Operator? _operator;

        private enum Operator
        {
            Add,
            Multiply,
            Subtract,
            Divide
        }

        public CalculatorViewModel()
        {
            _keyPressed = new KeyPressedCommand(this);
            _displayValue = "0";
        }

        public string DisplayValue
        {
            get { return _displayValue; }
            set
            {
                _displayValue = value;
                RaisePropertyChanged("DisplayValue");
            }
        }

        private void PressKey(char key)
        {
            int value;
            if (Int32.TryParse(key.ToString(), out value))
            {
                InsertNumber(value);
            }
            else
            {
                switch (key)
                {
                    case '+':
                        ApplyOperator(Operator.Add);
                        break;
                    case '*':
                        ApplyOperator(Operator.Multiply);
                        break;
                    case '/':
                        ApplyOperator(Operator.Divide);
                        break;
                    case '-':
                        ApplyOperator(Operator.Subtract);
                        break;
                    case '=':
                        if (!_operator.HasValue)
                        {
                            _tempValue = null;
                            _isValue = false;
                            return;
                        }
                        if (!_tempValue.HasValue) return;

                        switch (_operator.Value)
                        {
                            case Operator.Add:
                                ApplyResult(_tempValue.Value + Convert.ToInt32(DisplayValue));
                                break;
                            case Operator.Multiply:
                                ApplyResult(_tempValue.Value * Convert.ToInt32(DisplayValue));
                                break;
                            case Operator.Subtract:
                                ApplyResult(_tempValue.Value - Convert.ToInt32(DisplayValue));
                                break;
                            case Operator.Divide:
                                ApplyResult(Convert.ToInt32(_tempValue.Value / Convert.ToInt32(DisplayValue)));
                                break;
                            default:
                                break;
                        }
                        break;
                    default:
                        break;
                }
            }
        }

        private void ApplyResult(int result)
        {
            _tempValue = result;
            DisplayValue = _tempValue.ToString();
            _operator = null;
            _isValue = false;            
        }

        private void ApplyOperator(Operator @operator)
        {
            _tempValue = Convert.ToInt32(DisplayValue);
            _isValue = false;
            _operator = @operator;
        }

        private void InsertNumber(int value)
        {
            if (_isValue)
            {
                DisplayValue += value;
            }
            else
            {
                if (value == 0) return;

                DisplayValue = value.ToString();
                _isValue = true;
            }
        }

        public ICommand KeyPressed
        {
            get { return _keyPressed; }
        }

        private class KeyPressedCommand : ICommand
        {
            public readonly CalculatorViewModel _viewModel;
            public KeyPressedCommand(CalculatorViewModel viewModel)
            {
                _viewModel = viewModel;
            }

            bool ICommand.CanExecute(object parameter)
            {
                return true;
            }

            public event EventHandler CanExecuteChanged;

            void ICommand.Execute(object parameter)
            {
                _viewModel.PressKey(Convert.ToChar(parameter));
            }
        }
    }
}
