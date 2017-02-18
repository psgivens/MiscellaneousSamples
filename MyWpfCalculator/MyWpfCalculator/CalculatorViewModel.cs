using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace MyWpfCalculator {
    public class CalculatorViewModel : ViewModelBase {
        #region Fields
        private string _displayValue = "0";
        private string _equationDisplay = "";
        private bool _isEditing;
        private CalculationNode _head;
        #endregion

        private readonly ParameterCommand<char> _enterDigit;
        private readonly ParameterCommand<Operator> _enterOperator;
        private readonly ParameterCommand<Control> _enterControl;

        public CalculatorViewModel() {
            _enterDigit = new ParameterCommand<char>(Convert.ToChar, EnterDigit, _ => true);
            _enterOperator = new ParameterCommand<Operator>(_ => (Operator)Enum.Parse(typeof(Operator), _.ToString()),
                EnterOperator, IsOperatorAvailable);
            _enterControl = new ParameterCommand<Control>(_ => (Control)Enum.Parse(typeof(Control), _.ToString()),
                EnterControl, IsControlAvailable);
            _displayValue = "0";
        }

        public string DisplayValue {
            get { return _displayValue; }
            set {
                _displayValue = value;
                RaisePropertyChanged("DisplayValue");
            }
        }

        public string EquationDisplay {
            get { return _equationDisplay; }
            set {
                _equationDisplay = value;
                RaisePropertyChanged("EquationDisplay");
            }
        }

        public ICommand EnterDigitCommand {
            get { return _enterDigit; }
        }

        public ICommand EnterOperatorCommand {
            get { return _enterOperator; }
        }

        public ICommand EnterControlCommand {
            get { return _enterControl; }
        }

        public bool IsEditing {
            get { return _isEditing; }
            set {
                _isEditing = value;
                _enterControl.TriggerEvent();
                _enterOperator.TriggerEvent();
            }
        }

        private void EnterDigit(char digit) {
            Debug.WriteLine("Digit " + digit.ToString());
            if (IsEditing && DisplayValue != "0") {
                DisplayValue += digit.ToString();
            }
            else {
                if (_head == null) EquationDisplay = "";
                IsEditing = true;
                DisplayValue = digit.ToString();
            }
        }

        private void EnterOperator(Operator @operator) {
            switch (@operator) {
                case Operator.Add:
                case Operator.Multiply:
                case Operator.Subtract:
                case Operator.Divide:
                    Debug.WriteLine("Operator " + @operator.ToString());
                    int newValue = Convert.ToInt32(DisplayValue);
                    _head = Insert(_head, newValue, @operator);
                    AppendOperatorToEquation(@operator, newValue);
                    IsEditing = false;
                    break;
                case Operator.Reciprocal:
                    throw new NotImplementedException("Reciprocal");
                    break;
                case Operator.Percent:
                    throw new NotImplementedException("Percent");
                    break;
                case Operator.Invert:
                    throw new NotImplementedException("Invert");
                    break;
                case Operator.Negate:
                    throw new NotImplementedException("Negate");
                    break;
                case Operator.Radical:
                    throw new NotImplementedException("Radical");
                    break;
                default:
                    break;
            }
        }

        private void AppendOperatorToEquation(Operator @operator, int value) {
            char o = '\0';
            switch (@operator) {
                case Operator.Add:
                    o = '+';
                    break;
                case Operator.Multiply:
                    o = '*';
                    break;
                case Operator.Subtract:
                    o = '-';
                    break;
                case Operator.Divide:
                    o = '/';
                    break;
                default:
                    break;
            }
            EquationDisplay += string.Format("{0}{1}", value, o);
        }

        private void EnterControl(Control control) {
            Debug.WriteLine("Control " + control.ToString());
            switch (control) {
                case Control.Clear:
                    DisplayValue = "0";
                    _head = null;
                    IsEditing = false;
                    break;
                case Control.ClearE:
                    DisplayValue = "0";
                    IsEditing = false;
                    break;
                case Control.Back:
                    var displayValue = DisplayValue;
                    if (displayValue.Length > 1) {
                        DisplayValue = displayValue.Substring(0, displayValue.Length - 1);
                    }
                    else {
                        DisplayValue = "0";
                        IsEditing = false;
                    }
                    break;
                case Control.Calculate:
                    int newValue = Convert.ToInt32(DisplayValue);
                    var node = InsertFinal(_head, newValue);
                    var calculationValue = Calculate(node);
                    DisplayValue = calculationValue.ToString();
                    _head = null;
                    EquationDisplay += string.Format("{0}=", newValue);
                    IsEditing = false;
                    break;
                default:
                    break;
            }
        }

        private bool IsOperatorAvailable(Operator @operator) {
            switch (@operator) {
                case Operator.Add:
                    break;
                case Operator.Multiply:
                    break;
                case Operator.Subtract:
                    break;
                case Operator.Divide:
                    break;
                case Operator.Reciprocal:
                    break;
                case Operator.Percent:
                    break;
                case Operator.Invert:
                    break;
                case Operator.Negate:
                    break;
                case Operator.Radical:
                    break;
                default:
                    break;
            }
            return _isEditing;
        }

        private bool IsControlAvailable(Control control) {
            switch (control) {
                case Control.Clear:
                    break;
                case Control.Back:
                    break;
                case Control.Calculate:
                    break;
                case Control.ClearE:
                    break;
                default:
                    break;
            }
            return IsEditing;
        }

        private int Calculate(CalculationNode node) {
            if (node == null) return 0;

            var numeric = node.Value as NumericValue;
            if (numeric != null) return numeric.Value;

            if (node.Right == null) return Calculate(node.Left);

            var @operator = ((OperatorValue)node.Value).Value;
            var left = Calculate(node.Left);
            var right = Calculate(node.Right);
            switch (@operator) {
                case Operator.Add: return left + right;
                case Operator.Subtract: return left - right;
                case Operator.Multiply: return left * right;
                case Operator.Divide: return Convert.ToInt32(left / right);
                default: return 0;
            }
        }

        private CalculationNode InsertFinal(CalculationNode node, int value) {
            if (node == null) {
                return new CalculationNode(value);
            }
            else if (node.Value is NumericValue) {
                return new CalculationNode(value);
            }
            else if (node.Right == null) {
                node.Right = new CalculationNode(value);
            }
            else {
                InsertFinal(node.Right, value);
            }
            return node;
        }

        private CalculationNode Insert(CalculationNode node, int value, Operator @operator) {
            if (node == null) {
                return new CalculationNode(@operator) {
                    Left = new CalculationNode(value)
                };
            }
            else if (node.Value is NumericValue) {
                return new CalculationNode(@operator) {
                    Left = node
                };
            }
            else {
                var headOperator = ((OperatorValue)node.Value).Value;
                switch (headOperator) {
                    case Operator.Divide:
                    case Operator.Multiply:
                        node.Right = new CalculationNode(value);
                        return new CalculationNode(@operator) {
                            Left = node
                        };
                    case Operator.Add:
                    case Operator.Subtract:
                        if (node.Right == null) {
                            switch (@operator) {
                                case Operator.Add:
                                case Operator.Subtract:
                                    node.Right = new CalculationNode(value);
                                    return new CalculationNode(@operator) {
                                        Left = node
                                    };
                                case Operator.Divide:
                                case Operator.Multiply:
                                    node.Right = new CalculationNode(@operator) {
                                        Left = new CalculationNode(value)
                                    };
                                    return node;
                                default:
                                    return node;
                            }
                        }
                        else {
                            node.Right = Insert(node.Right, value, @operator);
                            return node;
                        }
                    default:
                        return node;
                }
            }
        }

        #region Nested Types
        private class ParameterCommand<T> : ICommand {
            private readonly Func<object, T> _convert;
            private readonly Action<T> _execute;
            private readonly Predicate<T> _canExecute;

            public ParameterCommand(Func<object, T> convert, Action<T> execute, Predicate<T> canExecute) {
                _convert = convert;
                _execute = execute;
                _canExecute = canExecute;
            }

            bool ICommand.CanExecute(object parameter) {
                return _canExecute(_convert(parameter));
            }

            internal void TriggerEvent() {
                if (CanExecuteChanged != null) {
                    CanExecuteChanged(this, EventArgs.Empty);
                }
            }
            public event EventHandler CanExecuteChanged;

            void ICommand.Execute(object parameter) {
                _execute(_convert(parameter));
            }
        }

        private class CalculationNode {
            public CalculationNode Left { get; set; }
            public CalculationNode Right { get; set; }

            public CalculationValue Value { get; private set; }

            public CalculationNode(int value) {
                Value = new NumericValue(value);
            }

            public CalculationNode(Operator value) {
                Value = new OperatorValue(value);
            }
        }

        private abstract class CalculationValue { }

        private class OperatorValue : CalculationValue {
            public Operator Value { get; private set; }
            public OperatorValue(Operator value) { Value = value; }
        }

        private class NumericValue : CalculationValue {
            public int Value { get; private set; }
            public NumericValue(int value) { Value = value; }
        }
        #endregion
    } 
}
