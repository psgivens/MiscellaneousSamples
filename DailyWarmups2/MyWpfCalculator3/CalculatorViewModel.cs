using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyWpfCalculator3
{
    public class CalculatorViewModel
    {
        #region Fields
        private string _displayValue = "0";
        private bool _isEditing;
        private CalculationNode _head;
        #endregion

        internal void EnteryKey(char character)
        {
            switch (character)
            {
                case '1':
                case '2':
                case '3':
                case '4':
                case '5':
                case '6':
                case '7':
                case '8':
                case '9':
                case '0':
                    if (_isEditing && DisplayValue != "0")
                    {
                        DisplayValue += character.ToString();
                    }
                    else
                    {
                        _isEditing = true;
                        DisplayValue = character.ToString();
                    }
                    break;
                case '+': EnterOperator(Operator.Add); break;
                case '-': EnterOperator(Operator.Subtract); break;
                case 'x':
                case '*': EnterOperator(Operator.Multiply); break;
                case '/': EnterOperator(Operator.Divide); break;
                case '=':
                    int newValue = Convert.ToInt32(DisplayValue);
                    var node = Insert(_head, newValue, Operator.None);
                    var calculationValue = Calculate(node);
                    DisplayValue = calculationValue.ToString();
                    _head = null;
                    _isEditing = false;
                    break;
                default:
                    break;
            }
        }

        private void EnterOperator(Operator @operator)
        {
            int newValue = Convert.ToInt32(DisplayValue);
            _head = Insert(_head, newValue, @operator);
            _isEditing = false;
        }

        private int Calculate(CalculationNode node)
        {
            if (node == null) return 0;

            var numeric = node.Value as NumericValue;
            if (numeric != null) return numeric.Value;

            if (node.Right == null) return Calculate(node.Left);

            var @operator = ((OperatorValue)node.Value).Value;
            var left = Calculate(node.Left);
            var right = Calculate(node.Right);
            switch (@operator)
            {
                case Operator.Add: return left + right;
                case Operator.Subtract: return left - right;
                case Operator.Multiply: return left * right;
                case Operator.Divide: return Convert.ToInt32(left / right);
                case Operator.None:
                default: return 0;
            }
        }

        private CalculationNode Insert(CalculationNode node, int value, Operator @operator)
        {
            if (node == null)
            {
                return (@operator == Operator.None)
                    ? new CalculationNode(value)
                    : new CalculationNode(@operator)
                    {
                        Left = new CalculationNode(value)
                    };
            }
            else if (node.Value is NumericValue)
            {
                if (@operator == Operator.None)
                {
                    return new CalculationNode(value);
                }
                return new CalculationNode(@operator)
                {
                    Left = node
                };
            }
            else
            {
                if (@operator == Operator.None)
                {
                    if (node.Right == null)
                    {
                        node.Right = new CalculationNode(value);
                    }
                    else
                    {
                        Insert(node.Right, value, @operator);
                    }
                    return node;
                }

                var headOperator = ((OperatorValue)node.Value).Value;
                switch (headOperator)
                {
                    case Operator.Divide:
                    case Operator.Multiply:
                        node.Right = new CalculationNode(value);
                        return new CalculationNode(@operator)
                        {
                            Left = node
                        };
                    case Operator.Add:
                    case Operator.Subtract:
                        if (node.Right == null)
                        {
                            switch (@operator)
                            {
                                case Operator.Add:
                                case Operator.Subtract:
                                    node.Right = new CalculationNode(value);
                                    return new CalculationNode(@operator)
                                    {
                                        Left = node
                                    };
                                case Operator.Divide:
                                case Operator.Multiply:
                                    node.Right = new CalculationNode(@operator)
                                    {
                                        Left = new CalculationNode(value)
                                    };
                                    return node;
                                case Operator.None:
                                default:
                                    return node;
                            }
                        }
                        else
                        {
                            node.Right = Insert(node.Right, value, @operator);
                            return node;
                        }
                    case Operator.None:
                    default:
                        return node;
                }
            }
        }
        
        public string DisplayValue
        {
            get { return _displayValue; }
            set { _displayValue = value; }
        }

        #region Nested types
        private enum Operator
        {
            None,
            Add,
            Subtract,
            Divide,
            Multiply
        }

        private class CalculationNode
        {
            public CalculationNode Left { get; set; }
            public CalculationNode Right { get; set; }

            public CalculationValue Value { get; private set; }

            public CalculationNode(int value)
            {
                Value = new NumericValue(value);
            }

            public CalculationNode(Operator value)
            {
                Value = new OperatorValue(value);
            }
        }

        private abstract class CalculationValue { }

        private class OperatorValue : CalculationValue
        {
            public Operator Value { get; private set; }
            public OperatorValue(Operator value) { Value = value; }
        }

        private class NumericValue : CalculationValue
        {
            public int Value { get; private set; }
            public NumericValue(int value) { Value = value; }
        }
        #endregion
    }
}
