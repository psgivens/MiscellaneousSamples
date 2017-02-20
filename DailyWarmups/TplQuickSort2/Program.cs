using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TplQuickSort2
{
    class Program
    {
        // Using a constant seed ensures predictable randomness.
        private static readonly Random _random = new Random(20);
        private const int Range = 100000;

        static void Main(string[] args)
        {
            var set = new HashSet<int>();
            while(set.Count < Range)
            {
                set.Add(_random.Next(Range));
            }

            int[] items = set.ToArray();
            var task = Sort(items, 0, Range - 1);
            task.Wait();

            for (int i = 0; i < Range; i++)
            {
                Debug.Assert(i == items[i], string.Format("Item {0} expected to be {0}, but was {1}", i, items[i]));
            }
        }

        private static Task Sort(int[] items, int min, int max)
        {
            if (max <= min) return Task.Delay(0);

            return Task.Factory.StartNew(() =>
                {

                    int index = _random.Next(min, max + 1);
                    int pivot = items[index];

                    items[index] = items[max];
                    items[max] = pivot;

                    index = min;

                    for (int i = min; i < max; i++)
                    {
                        int value = items[i];
                        if (value < pivot)
                        {
                            items[i] = items[index];
                            items[index] = value;
                            index++;
                        }
                    }

                    items[max] = items[index];
                    items[index] = pivot;

                    Sort(items, min, index - 1);
                    Sort(items, index + 1, max);
                },
 
                // AttachedToParent causes the parent Task to wait for all recursive
                // calls to Sort to finish. 
                TaskCreationOptions.AttachedToParent);
        }
    }
}
