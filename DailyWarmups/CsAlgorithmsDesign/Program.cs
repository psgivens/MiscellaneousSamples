using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CsAlgorithmsDesign {
    class Program {
        private static readonly ulong[] _values;
        private const int _Total = 10000;
        static Program() {
            _values = new ulong[_Total];
            for (ulong i = 0; i < _Total; i++) {
                _values[i] = i*i*i;
            }
        }
        static void Main(string[] args) {
            var found = new List<int>();
            foreach(var i in FindRamanujanHardyNumbers()) {
                found.Add(i);
                Console.WriteLine("Found {0} at {1}", _values[i], i);
            }
            Console.WriteLine("========== Final Report ============");
            foreach (var i in found) {
                Console.WriteLine("Found {0} at {1}", _values[i], i);
            }

            Console.ReadKey();
        }

        private static IEnumerable<int> FindRamanujanHardyNumbers() {
            int joffset = 1;
            int koffset = 1;

            for (int i = 3; i < _Total; i++) {
                bool jbreak = false;
                for (int j = i-joffset; j >= 2 && !jbreak; j--) {
                    bool kbreak = false;
                    for (int k = j-koffset; k >= 1 && !kbreak; k--) {
                        ulong upperLowerSum = _values[0] + _values[i];
                        ulong iteratingSum = _values[j] + _values[k];
                        if (upperLowerSum == iteratingSum) {
                            yield return i;
                            jbreak = true;
                            kbreak = true;
                            joffset = i - j;
                        }
                        else if (upperLowerSum > iteratingSum) {
                            kbreak = true;
                        }
                    }
                    Console.Write('.');
                }
                Console.WriteLine("\nchecking {0} at {1}", _values[i], i);
            }
            yield break;
        }
    }
}
