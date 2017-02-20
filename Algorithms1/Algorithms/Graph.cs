using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Algorithms
{
    public class GraphTree {
        public int [] Parents { get; set; }
        public int [] PWeights { get; set; }
    }
    public class Graph
    {
        public const int MaxV = 1000;
        private class EdgeNode
        {
            public EdgeNode(int value, int weight) {
                Value = value;
                Weight = weight;
            }
            public int Value { get; private set; }
            public EdgeNode Next { get; set; }
            public int Weight { get; private set; }
        }
        private EdgeNode[] _edges = new EdgeNode[MaxV + 1];
        private int nedges;
        public void InsertEdge(int x, int y, int weight)
        {
            InsertEdge(x, y, weight, false);
        }
        private void InsertEdge(int x, int y, int weight, bool directed)
        {
            _edges[x] = new EdgeNode(y, weight)
            {
                Next = _edges[x]
            };
            if (directed)
            {
                nedges++;
            }
            else
            {
                InsertEdge(y, x, weight, true);
            }
        }

        public void PrintGraph()
        {
            for (int i = 0; i < _edges.Length; i++)
            {
                if (_edges[i] == null) continue;
                Console.Write("{0}: ", i);
                for (EdgeNode edge = _edges[i]; edge != null; edge = edge.Next)
                    Console.Write("{0}->{1} ", i, edge.Value);
                Console.WriteLine();
            }
        }

        public abstract class GraphSearch
        {
            public abstract void ProcessVertexEarly(int vertex);
            public abstract void ProcessVertexLate(int vertex);
            public abstract void ProcessEdge(int x, int y);
            public abstract void OnProcessingFinished();
            protected abstract void Process(Graph graph, int startVertex);
        }

        public void Prim() {
            GraphTree tree = new GraphTree();
            bool[] inTree = new bool[MaxV + 1];
            int[] weights = new int[MaxV + 1];
            int[] parent = new int[MaxV + 1];

            for (int j = 0; j <= 10; j++) weights[j] = int.MaxValue;

            int i = 1;            
            while (!inTree[i]) {
                inTree[i] = true;

                // Find lowest weight for edges of newly processed vertex. 
                for(EdgeNode edge = _edges[i]; edge != null; edge = edge.Next) {
                    int value = edge.Value;
                    int weight = edge.Weight;
                    if (!inTree[value] && weights[value] > weight) {
                        weights[value] = weight;
                        parent[value] = i;
                    }                        
                }

                // Choose the lowest non-included edge
                int lowest = int.MaxValue;
                for (int j = 0; j <= 10; j++) {
                    if (!inTree[j] && weights[j] < lowest) {
                        lowest = weights[j];
                        i = j;
                    }
                }
            }
        }

        public abstract class DFS : GraphSearch
        {
            protected override void Process(Graph graph, int startVertex)
            {
                throw new NotImplementedException();
            }
        }

        public abstract class BFS : GraphSearch
        {
            private bool[] _discovered;
            private bool[] _processed;
            private Queue<int> _processingQueue;
            private int[] _parent;
            public BFS(int size)
            {
                _discovered = new bool[size];
                _processed = new bool[size];
                _parent = new int[size];
            }
            protected override sealed void Process(Graph graph, int startVertex)
            {
                _discovered[startVertex] = true;
                _processingQueue.Enqueue(startVertex);

                while (_processingQueue.Count > 0)
                {
                    int vertex = _processingQueue.Dequeue();
                    ProcessVertexEarly(vertex);
                    for (EdgeNode edge = graph._edges[vertex]; edge != null; edge = edge.Next)
                    {
                        int endVertex = edge.Value;
                        if (!_processed[endVertex])
                        {
                            ProcessEdge(vertex, endVertex);
                        }
                        if (!_discovered[endVertex])
                        {
                            _discovered[endVertex] = true;
                            _processingQueue.Enqueue(endVertex);
                            _parent[endVertex] = vertex;
                        }
                    }
                    ProcessVertexLate(vertex);
                }
            }

        }

        public class CountingBFS : BFS
        {
            private int _edgeCount;
            private int _vertexCount;

            public CountingBFS(int size) : base(size) { }

            public override void OnProcessingFinished()
            {
                Console.WriteLine("There are {0} vertices and {1} edges.", _vertexCount, _edgeCount);
            }

            public override void ProcessEdge(int x, int y)
            {
                _edgeCount++;
            }

            public override void ProcessVertexEarly(int vertex)
            {
                _vertexCount++;
            }

            public override void ProcessVertexLate(int vertex)
            {
            }
        }
    }


}
