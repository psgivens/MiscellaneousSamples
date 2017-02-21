using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;

namespace Algorithms
{
    [TestClass]
    public class SortAlgorithmUnitTests
    {
        private const int Max = 20 * 1000 * 1000;
        //private const int Max = 10;
        private readonly HashSet<int> _hashSet = new HashSet<int>();
        public SortAlgorithmUnitTests()
        {
            Random random = new Random(20);
            while (_hashSet.Count < Max)
            {
                _hashSet.Add(random.Next(Max));
            }            
        }

        [TestMethod]
        public void QuickSortTpl()
        {
            Console.WriteLine("QuickSortTpl");
                        
            var items = _hashSet.ToArray();

            var stopWatch = new System.Diagnostics.Stopwatch();
            stopWatch.Start();
            Algorithms.SortAlgorithms.QuicksortTpl(items);            
            stopWatch.Stop();
            Console.WriteLine("Quicksort with TPL ran in {0} ms.", stopWatch.ElapsedMilliseconds);

            for (int i = 0; i < Max; i++)
            {
                Assert.IsTrue(items[i] == i, string.Format("Item {0} was {1} and should be {0}", i, items[i]));
            }
        }

        [TestMethod]
        public void QuickSort()
        {
            Console.WriteLine("QuickSort");

            var items =_hashSet.ToArray();

            var stopwatch = new System.Diagnostics.Stopwatch();
            stopwatch.Start();
            Algorithms.SortAlgorithms.Quicksort(items);
            stopwatch.Stop();
            Console.WriteLine("Quicksort ran in {0} ms.", stopwatch.ElapsedMilliseconds);

            for (int i = 0; i < Max; i++)
            {
                Assert.IsTrue(items[i] == i, string.Format("Item {0} was {1} and should be {0}", i, items[i]));
            }
        }

    }
}
