using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using TechTalk.SpecFlow;

namespace MyWpfCalculator2.Specs.StepDefinitions
{
    [Binding]
    public class CalculatorSpecificationSteps
    {
        private readonly CalculatorViewModel _calculator = new CalculatorViewModel();
        private string _displayValue;

        [Given(@"I have pressed the key '(.)'")]
        public void GivenIHavePressedTheKey(char p0)
        {
            _calculator.KeyPressed.Execute(p0);
        }

        [When(@"I look at the display")]
        public void WhenILookAtTheDisplay()
        {
            _displayValue = _calculator.DisplayValue;
        }

        [Then(@"I see the string ""(.*)""")]
        public void ThenISeeTheString(string p0)
        {
            Assert.IsTrue(_displayValue == p0, string.Format("Display value shows {0}, but we expected {1}", _displayValue, p0));
        }
    }
}
