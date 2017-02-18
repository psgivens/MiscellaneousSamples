using System;
using System.Collections.Generic;
using TechTalk.SpecFlow;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Algorithms.SpecDefinitions
{
    [Binding]
    public class QuicksortAlgorithmSteps
    {
        private int _max;
        private int _min;
        private int[] items;
                
        [Given(@"that I have an array of random numbers from (.*) to (.*)\.")]
        public void GivenThatIHaveAnArrayOfRandomNumbersFromTo_(int min, int max)
        {
            _min = min;
            _max = max;
            Random random = new Random(20);
            int range = max - min + 1;
            var hashSet = new HashSet<int>();
            while (hashSet.Count < range)
            {
                hashSet.Add(random.Next(min, _max + 1));
            }
            items = hashSet.ToArray();
        }

        [When(@"I run quicksort,")]
        public void WhenIRunQuicksort_()
        {
            Algorithms.SortAlgorithms.Quicksort(items);
        }

        [When(@"I run quicksort with the Task Parallel Library,")]
        public void WhenIRunQuicksortWithTheTaskParallelLibrary()
        {
            Algorithms.SortAlgorithms.QuicksortTpl(items);
        }

        [Then(@"the array will run sequentially\.")]
        public void ThenTheArrayWillRunSequentially_()
        {
            for (int i = _min; i < _max; i++)
            {
                Assert.IsTrue(items[i] == i, string.Format("Item {0} was {1} and should be {0}", i, items[i]));
            }
        }
    }
}
