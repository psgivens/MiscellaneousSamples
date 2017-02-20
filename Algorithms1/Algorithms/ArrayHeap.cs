using System;
using System.Text;

namespace Algorithms
{
    class ArrayHeap 
    {
        public ArrayHeap(int[] values, int length)
        {
            if (length < values.Length)
                throw new InvalidOperationException("Cannot copy values to heap because heap is not large enough.");

            this.length = length;
            array = new int[length];
            Count = values.Length;
            Array.Copy(values, array, length);
            for (int i = length - 1; i >= 0; i--)
            {
                Heapify2(i);
            }
        }

        //[0;1;2;3;4;5;6;7;8]
        public void Heapify2(int i)
        {
            int currentValue = array[i];
            int left = (2 * i) + 1;
            int right = (2 * i) + 2;
            if (right < length)
            {
                // only use left
                int leftValue = array[left];
                int rightValue = array[right];
                bool useLeft = (FavorFirst(leftValue, rightValue));
                if (FavorFirst(useLeft ? leftValue : rightValue, currentValue))
                {
                    Swap(useLeft ? left : right, i);
                    Heapify2(useLeft ? left : right);
                }
            }
            else if (left < length)
            {
                // we are at a leaf
                int leftValue = array[left];
                if (FavorFirst(leftValue, currentValue))
                {
                    Swap(left, i);
                    Heapify2(left);
                }
            }
            else
            {
                // Do nothing. We are at a leaf. 
            }
        }


        public ArrayHeap(int length)
        {
            array = new int[length];
            this.length = length;
        }
        private readonly int length;
        private int[] array;
        public int Count { get; private set; }

        public void Insert(int value)
        {
            if (Count == length)
                throw new ArgumentOutOfRangeException("Attempting to add a value to a full heap.");
            array[Count] = value;
            Heapify(Count++);
        }

        public int Pop()
        {
            int retValue = array[0];
            array[0] = array[--Count];
            if (Count != 0)
            {
                array[Count] = 0;
                HeapifyChildren(0);
            }
            return retValue;
        }

        public bool HasValues()
        {
            return Count > 0;
        }

        public override string ToString()
        {
            StringBuilder builder = new StringBuilder("[");
            for (int i = 0; i < Count;)
            {
                builder.Append(array[i]);
                builder.Append(++i == Count ? ']' : ',');
            }
            return builder.ToString();
        }

        private void HeapifyChildren(int position)
        {
            int t = position * 2;
            int first = t + 1;
            int second = t + 2;
            int parent = array[position];
            if (second > Count - 1) return;

            int fvalue = array[first];
            int svalue = array[second];
            if (FavorFirst(fvalue, svalue))
            {
                if (FavorFirst(parent, fvalue)) return;
                array[position] = fvalue;
                array[first] = parent;
                HeapifyChildren(first);
            }
            else
            {
                if (FavorFirst(parent, svalue)) return;
                array[second] = parent;
                array[position] = svalue;
                HeapifyChildren(second);
            }
        }

        private bool FavorFirst(int first, int second)
        {
            return first < second;
        }

        private void Heapify(int position)
        {
            if (position == 0) return;
            int value = array[position];
            int compPosition = position / 2;
            int compValue = array[compPosition];
            if (FavorFirst(value, compValue))
            {
                Swap(position, compPosition);
                Heapify(compPosition);
            }
        }
        private void Swap(int a, int b)
        {
            int temp = array[a];
            array[a] = array[b];
            array[b] = temp;
        }
    }
}
