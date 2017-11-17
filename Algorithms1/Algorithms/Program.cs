using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Algorithms
{
    public class Program
    {
        const int SampleSize = 25;

        private static void Experiment()
        {
            //int[] x = new int[] { 4,5,6};
            //Console.WriteLine((int)(1 / 2));
            //Console.ReadLine();
        }
        
        static void Main(string[] args)
        {
            Experiment();
            //RunGraph();
            RunRedBlackTree();
            //RunArrayHeap();
            //RunBTree();
            Console.WriteLine("Press Enter");
            Console.ReadLine();
        }

        private static void RunGraph() {
            Graph graph = new Graph();
            graph.InsertEdge(1, 8, 2);
            graph.InsertEdge(1, 3, 2);
            graph.InsertEdge(7, 8, 2);
            graph.InsertEdge(2, 3, 1);
            graph.InsertEdge(3, 4, 2);
            graph.InsertEdge(3, 7, 2);
            graph.InsertEdge(7, 6, 3);
            graph.InsertEdge(4, 6, 1);
            graph.InsertEdge(4, 5, 1);
            graph.InsertEdge(2, 5, 3);
            graph.Prim();
        }
        private static void RunBTree()
        {

        }

        private static void RunRedBlackTree()
        {
            RedBlackTree tree = new RedBlackTree();
            tree.Insert(42);
            for (int i = 1; i <= 12; i++)
            {
                tree.Insert(i);
            }
            foreach(var value in new int[]{ 3, 7, 8 })
            {
                tree.ToString();
                tree.Delete(value);
            }
        }

        private static void RunLinkedList()
        {
            int[] sampleNumbers = ReadSample(SampleSize, 100);
            LinkedList list = new LinkedList();
            for (int i = 0; i < sampleNumbers.Length; i++)
            {
                list.InsertBegining(sampleNumbers[i]);
            }
            Console.WriteLine(list);
            list.Reverse();
            Console.WriteLine(list);
        }

        private static void RunArrayHeap()
        {            
            int[] sampleNumbers = ReadSample(SampleSize, 100);

            var heap = new ArrayHeap(sampleNumbers, SampleSize);
            while (heap.HasValues())
            {
                //Console.WriteLine(heap);
                int value = heap.Pop();
                Console.Write(value + ", ");
            }
            
            Console.WriteLine("Press any key");
            Console.ReadLine();


            heap = new ArrayHeap(SampleSize);
            for (int i = 0; i < SampleSize; i++)
            {
                heap.Insert(sampleNumbers[i]);
            }

            while (heap.HasValues())
            {
                //Console.WriteLine(heap);
                int value = heap.Pop();
                Console.Write(value + ", ");
            }
        }

        private static int[] CreateSample(int sampleSize, int maxValue)
        {
            Random rand = new Random((int)(DateTime.Now.Ticks % int.MaxValue));
            int[] array = new int[sampleSize];

            for (int i = 0; i < sampleSize; i++)
            {
                bool found = false;
                int nextVal;
                do
                {
                    found = false;
                    nextVal = rand.Next() % maxValue;
                    if (nextVal == 0)
                    {
                        found = true;
                        continue;
                    }
                    for (int j = 0; j < i; j++)
                        if (nextVal == array[i])
                        {
                            found = true;
                            break;
                        }
                }
                while (found);
                array[i] = nextVal;
            }

            using (var file = new StreamWriter(string.Format("{0}_{1}.txt", sampleSize, maxValue)))
            {
                foreach (var value in array)
                    file.WriteLine(value);
            }
            return array;
        }

        private static int[] ReadSample(int sampleSize, int maxValue)
        {
            string fileName = string.Format("{0}_{1}.txt", sampleSize, maxValue);
            if (File.Exists(fileName))
            {
                using (var reader = new StreamReader(fileName))
                {
                    int[] array = new int[SampleSize];
                    string line;
                    int i = 0;
                    while ((line = reader.ReadLine()) != null)
                    {
                        array[i++] = Convert.ToInt32(line);
                    }
                    return array;
                }

            }
            else return CreateSample(sampleSize, maxValue);
        }

        
    }


}
