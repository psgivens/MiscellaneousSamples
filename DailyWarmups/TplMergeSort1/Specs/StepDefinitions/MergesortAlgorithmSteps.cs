using System;
using System.Collections.Generic;
using TechTalk.SpecFlow;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TplMergeSort1.Specs.StepDefinitions
{
    [Binding]
    public class MergesortAlgorithmSteps
    {
        private int[] _items;
        private int _min;
        private int _max;

        [Given(@"that I have an array of random numbers from (.*) to (.*)\.")]
        public void GivenThatIHaveAnArrayOfRandomNumbersFromTo_(int min, int max)
        {
            var random = new Random(32);
            int range = max - (min - 1);
            HashSet<int> set = new HashSet<int>();
            while (set.Count < range)
            {
                set.Add(random.Next(min, max + 1));
            }
            _items = set.ToArray();
        }

        [When(@"I run Mergesort,")]
        public void WhenIRunMergesort()
        {
            SortAlgorithms.Mergesort(_items);
        }

        [When(@"I run Mergesort with the Task Parallel Library,")]
        public void WhenIRunMergesortWithTheTaskParallelLibrary()        
        {
            //Assert.Fail("Not implemented");

            SortAlgorithms.MergesortTpl(_items);
        }

        [Then(@"the array will run sequentially\.")]
        public void ThenTheArrayWillRunSequentially_()
        {
            int expected = _min;
            for (int i = 0; i < _items.Length; i++, expected++)
            {
                Assert.IsTrue(_items[i] == expected, string.Format("Item {0} was expected to be {1}, but was {1}.", i, expected, _items[i]));
            }
        }
    }
}
