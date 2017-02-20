using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using TechTalk.SpecFlow;

namespace MyWpfCalculator3.Specs.StepDefinitions
{
    [Binding]
    public class CalculatorSpecificationSteps
    {
        private readonly CalculatorViewModel _calculator = new CalculatorViewModel();
        
        [Given(@"I have entered the keys ""(.*)""")]
        public void GivenIHaveEnteredTheKeys(string keySequence)
        {
            foreach(char key in keySequence)
            {
                _calculator.EnteryKey(key);
            }
        }

        [When(@"I enter the key ""(.)""")]
        public void WhenIEnterTheKey(char character)
        {
            _calculator.EnteryKey(@character);
        }

        [Then(@"the display value will be ""(.*)""")]
        public void ThenTheDisplayValueWillBe(string expectedDisplay)
        {
            Assert.AreEqual(_calculator.DisplayValue, expectedDisplay, string.Format("Display is expected to show {0}, but shows {1}", _calculator.DisplayValue, expectedDisplay));
        }

    }
}
