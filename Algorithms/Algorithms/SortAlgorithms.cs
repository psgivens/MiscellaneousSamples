using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Algorithms
{
    public static class SortAlgorithms
    {
        private static readonly Random _random = new Random(21);

        public static void QuicksortTpl(int[] items)
        {
            Task task = QuicksortTplCore(items, 0, items.Length - 1);
            task.Wait();
        }

        private static Task QuicksortTplCore(int[] items, int min, int max)
        {
            return (max <= min)
                ? Task.Delay(0)
                : Task.Factory.StartNew(() =>
                    {
                        int newPivotIndex = _random.Next(min, max + 1);
                        int pivot = items[newPivotIndex];

                        // Move pivot to last place
                        items[newPivotIndex] = items[max];
                        items[max] = pivot;

                        newPivotIndex = min;

                        for (int index = min; index < max; index++)
                        {
                            var current = items[index];
                            if (current < pivot)
                            {
                                items[index] = items[newPivotIndex];
                                items[newPivotIndex++] = current;
                            }
                        }

                        items[max] = items[newPivotIndex];
                        items[newPivotIndex] = pivot;

                        QuicksortTplCore(items, min, newPivotIndex - 1);
                        QuicksortTplCore(items, newPivotIndex + 1, max);
                    }, TaskCreationOptions.AttachedToParent);
        }

        public static void Quicksort(int[] items)
        {
            QuicksortCore(items, 0, items.Length - 1);
        }

        private static void QuicksortCore(int[] items, int min, int max)
        {
            if (max <= min) return;

            int newPivotIndex = _random.Next(min, max + 1);
            int pivot = items[newPivotIndex];

            // Move pivot to last place
            items[newPivotIndex] = items[max];
            items[max] = pivot;

            newPivotIndex = min;

            for (int index = min; index < max; index++)
            {
                var current = items[index];
                if (current < pivot)
                {
                    items[index] = items[newPivotIndex];
                    items[newPivotIndex++] = current;
                }
            }

            items[max] = items[newPivotIndex];
            items[newPivotIndex] = pivot;

            QuicksortCore(items, min, newPivotIndex - 1);
            QuicksortCore(items, newPivotIndex + 1, max);
        }

    }
}
