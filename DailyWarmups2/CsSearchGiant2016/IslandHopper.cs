using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CsSearchGiatn2016 {
    public class IslandHopper {
        public struct Island {
            public int Up, Down, Left, Right;
        }
        private Island[] _islands;
        //private ulong _survivingBridges = ulong.MaxValue;
        private readonly int _rows;
        private readonly int _columns;
        private readonly int _maxIslands;
        private readonly int _maxBridges;
        private readonly int _permutations;

        public IslandHopper(int rows, int columns) {
            Console.WriteLine("Hopper {0} {1}", rows, columns);

            _rows = rows;
            _columns = columns;
            _maxIslands = rows * columns;
            _maxBridges = 2 * columns * rows + columns - rows;
            _permutations = 1 << _maxBridges;
            _islands = new Island[_maxIslands];
            for (int i = 0; i < _islands.Length; i++) {
                _islands[i] = CreateIsland(i);
            }
            List<ulong> traversals = GetPaths();
            Console.WriteLine("Matches: {0}", traversals.Count);
            foreach (var val in traversals) {
                Console.WriteLine("Value: " + Convert.ToString((long)val, 2).PadLeft(14, '0'));
            }

            List<ulong> considered = new List<ulong>();
            ulong permutations = 0;
            foreach (var val in traversals) {
                int bitCount = CountBits(val);
                int unusedCount = _maxBridges - bitCount;
                if (considered.Count > 0) {
                    ulong agg = (1UL << _maxBridges) - 1;
                    foreach (var c in considered) {
                        agg &= ~(val | c);
                    }
                    int commonCount = CountBits(agg);
                    permutations += (1UL << unusedCount) - (1UL << commonCount);
                } else {
                    permutations += (1UL << unusedCount);
                }
                considered.Add(val);                
            }
            
            Console.Write("Found {0} permutations", permutations);




            //ulong usedBits = 0UL;
            //ulong permutations = 0;
            //foreach(var val in traversals) {
            //    int bitCount = CountBits(val);
            //    int unusedCount = _maxBridges - bitCount;

            //    // Experiment with this
            //    // ulong commonBits = ~(usedBits | val) & ((1UL << _maxBridges) - 1);

            //    ulong commonBits = usedBits & val;
            //    usedBits |= val;
            //    int commonCount = CountBits(commonBits);

            //    Console.WriteLine("UsedBits: " + Convert.ToString((long)usedBits, 2).PadLeft(14, '0'));
            //    checked {
            //        if (commonCount < unusedCount) {
            //            // Works for 5 bridges, but not 13. 
            //            permutations += Math.Max((1UL << unusedCount) - (commonCount == 0 ? 0UL : 1UL << commonCount), 0UL);
            //        }
            //    }
            //}
            //Console.Write("Found {0} permutations", permutations);




            //var unique = new Dictionary<ulong, int>();
            //for(int i=0; i<traversals.Count; i++) {
            //    ulong path = traversals[i];
            //    ulong mask = ~path;
            //    for(int j=0; j< i; j++) {
            //        ulong val = traversals[j];
            //        ulong common = val & mask;

            //    }
            //}

        }
        public int CountBits(ulong field) {
            int count = 0;
            while(field > 0UL) {
                if (field % 2 == 1) { count++; }
                field >>= 1;
            }
            return count;
        }

        public Island CreateIsland(int i) {
            int row = i / _columns;
            int column = i % _columns;

            return new Island {
                Down = ((row + 1) * (2 * _columns - 1)) + column,
                Up = (row * (2 * _columns - 1)) + column,
                Right = column == _columns - 1 ? -1
                    : (row * (2 * _columns - 1)) + _columns + column,
                Left = column == 0 ? -1
                    : (row * (2 * _columns - 1)) + _columns + column - 1
            };
        }

        public List<int> GetIslands(int bridge) {
            var islands = new List<int>();
            int row = bridge / (_columns + _rows);
            int column = bridge % (_columns + _rows);
            bool isVertical = column < _columns;
            int first, second;
            if (isVertical) {
                first = row == 0 ? first = -1
                    : (row - 1) * _columns + column;
                second = Math.Min(row * _columns + column, _maxIslands);
            }
            else {
                first = row * _columns + (column % _columns);
                second = first + 1;
            }
            islands.Add(first);
            islands.Add(second);
            return islands;
        }

        public List<ulong> GetPaths() {
            var paths = new List<ulong>();
            for (int i = 0; i < _columns; i++) {
                TraversPath(-1, paths, 0UL, i);
            }
            return paths;
        }

        public void TraversPath(int originalIsland, List<ulong> paths, ulong currentPath, int bridge) {
            if (bridge < 0) {
                return;
            }
            ulong bridgeBit = 01UL << bridge;
            if ((bridgeBit & currentPath) == bridgeBit) { return; }
            currentPath |= bridgeBit;

            var islandIndices = GetIslands(bridge);
            foreach (var i in islandIndices) {
                if (i < 0 || i == originalIsland) { continue; }
                if (i == _maxIslands) {
                    paths.Add(currentPath);
                    return;
                }
                var next = _islands[i];
                TraversPath(i, paths, currentPath, next.Up);
                TraversPath(i, paths, currentPath, next.Right);
                TraversPath(i, paths, currentPath, next.Down);
                TraversPath(i, paths, currentPath, next.Left);
            }
        }


        public void BuildGraph(int rows, int columns) {
            int bridgeCount = columns * (rows + 1) + (columns - 1) * rows;
            int islandCount = rows * columns;

            for (int i = 0; i < rows; i++) {
                for (int j = 0; j < columns; j++) {

                }
            }
        }

        //        public int UniquePaths() {
        //            var paths = new List<ulong>();
        //
        //        }

        //        public Dictionary<ulong,int> GetPaths(
        //            Dictionary<ulong,int> existing, Dictionary) {
        //            return existing;
        //        }

        public bool CanCross() {

            return false;
        }
    }
}
