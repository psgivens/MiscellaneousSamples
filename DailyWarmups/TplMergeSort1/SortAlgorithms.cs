using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TplMergeSort1
{
    public static class SortAlgorithms
    {
        public static void Mergesort(int[] items)
        {
            Queue<MergeSortNode> queue = new Queue<MergeSortNode>();
            int length = items.Length;
            for (int i = 0; i < length; i++)
            {
                queue.Enqueue(new MergeSortNode(items[i]));
            }

            MergeSortNode current = queue.Dequeue();
            while (queue.Any())
            {
                // merge with current
                var next = queue.Dequeue();
                MergeSortNode head = Merge(current, next);
                queue.Enqueue(head);
                current = queue.Dequeue();
            }

            for (int i = 0; i < length; i++)
            {
                items[i] = current.Value;
                current = current.Next;
            }
        }

        public static void MergesortTpl(int[] items)
        {
            var queue = new Queue<MergeSortNode>();
            int length = items.Length;
            for (int i = 0; i < length; i++)
            {
                queue.Enqueue(new MergeSortNode(items[i]));
            }


            MergeSortNode current = queue.Dequeue();
            
            do
            {
                var tasks = new Queue<Task<MergeSortNode>>();
                while (queue.Any())
                {
                    var next = queue.Dequeue();
                    var head = current;
                    tasks.Enqueue(Task.Factory.StartNew(() =>
                    {
                        // merge with current
                        return Merge(head, next);
                    }));

                    current = queue.Any() ? queue.Dequeue() : null;
                }
                Task.WaitAll(tasks.ToArray());
                if (current != null) queue.Enqueue(current);
                foreach (var task in tasks)
                {
                    queue.Enqueue(task.Result);
                }
                current = queue.Dequeue();
            }
            while (queue.Any());

            for (int i = 0; i < length; i++)
            {
                items[i] = current.Value;
                current = current.Next;
            }
        }

        private static MergeSortNode Merge(MergeSortNode left, MergeSortNode right)
        {
            MergeSortNode head = null;
            MergeSortNode comparisonWinner;
            MergeSortNode comparisonLoser = null;
            MergeSortNode tail = null;
            do
            {
                if (left.Value < right.Value)
                {
                    comparisonWinner = left;
                    comparisonLoser = right;
                }
                else
                {
                    comparisonWinner = right;
                    comparisonLoser = left;
                }

                // We need a linked list head on the first iteration
                if (head == null) head = comparisonWinner;

                // We need to link every successive iteration.
                if (tail != null) tail.Next = comparisonWinner;

                left = comparisonWinner.Next;
                right = comparisonLoser;
                tail = comparisonWinner;

                if (left == null) tail.Next = right;
            } while (left != null);

            return head;
        }

        private static MergeSortNode MergeRecursively(MergeSortNode left, MergeSortNode right)
        {
            MergeSortNode head;
            MergeSortNode next;
            if (left == null)
            {
                //right.Next = null;
                return right;
            }
            else if (left.Value < right.Value)
            {
                head = left;
                next = right;
            }
            else
            {
                head = right;
                next = left;
            }

            next = Merge(head.Next, next);
            head.Next = next;
            return head;
        }

        private class MergeSortNode
        {
            public MergeSortNode Next { get; set; }
            public int Value { get; set; }

            public MergeSortNode(int value)
            {
                Value = value;
            }
        }
    }

}
