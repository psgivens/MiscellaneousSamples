using Microsoft.VisualStudio.TestTools.UnitTesting;
using NumbersOfBabel;
using System;
using TechTalk.SpecFlow;

namespace NumbersTranslators.UnitTests {
    [Binding]
    public class IndonesianLanguageFeatureSteps {
        string _result;

        [When(@"I translate (.*) into (.*)")]
        public void WhenITranslate(int input, string language) {
            INumberTranslator _numberTranslator = null;
            switch (language) {
                case "bahasa Indonesia":
                    _numberTranslator = new NumbersTranslators.IndonesianNumberTranslator();
                    break;
                default:
                    Assert.Fail("No number translator for " + language);
                    break;
            }

            _result = _numberTranslator.TranslateNumber(input);
        }

        [Then(@"the result should be '(.*)'")]
        public void ThenTheResultShouldBe(string output) {
            Assert.AreEqual(output, _result);
        }
    }
}
