using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TplQuickSort1
{
    class Program
    {
        private static readonly Random _random = new Random(20);

        static void Main(string[] args)
        {            
            HashSet<int> itemsHash = new HashSet<int>();
            while(itemsHash.Count < 10)
            {
                itemsHash.Add(_random.Next(10));
            }

            int[] items = itemsHash.ToArray();

            Sort(items, 0, 9);
        }

        private static void Sort(int[] items, int min, int max)
        {
            if (min >= max) return;

            int index = _random.Next(min, max);
            int item = items[index];

            items[index] = items[max];
            items[max] = item;

            int storeIndex = min;

            for (int i = min; i < max; i++)
            {
                int comp = items[i];
                if (comp <= item)
                {
                    items[i] = items[storeIndex];
                    items[storeIndex++] = comp;
                }
            }

            int temp = items[storeIndex];
            items[storeIndex] = items[max];
            items[max] = temp;

            Sort(items, min, storeIndex - 1);
            Sort(items, storeIndex + 1, max);
        }
    }
}
