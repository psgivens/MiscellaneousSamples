using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CsSearchGiatn2016 {
    public class Program {
        static void Main(string[] args) {
            int[,] values = new int[3, 4];
            values[2, 3] = 13;
            int leng = values.Length;
            int lengt = values.GetUpperBound(0);
            int lengtt = values.GetUpperBound(1);
            int rank = values.Rank;

            new TheBomb();
            new TriangleCellArrangment();
            var hopper = new IslandHopper(1, 2);
            var hopper2 = new IslandHopper(2, 3);
            Console.WriteLine(Divide(2, 7));
            Console.WriteLine(Divide(23, 56));
            Console.WriteLine(Divide(1, 8));
            Console.ReadKey();
        }

        private class Node {
            public Node(int q, int r) {
                Quotient = q;
                Remainder = r;
            }
            public readonly int Quotient;
            public readonly int Remainder;
            public override bool Equals(object obj) {
                var other = obj as Node;
                if (other == null) return false;
                return other.Quotient == Quotient && other.Remainder == Remainder;
            }
        }
        private static string Divide(int dividend, int divisor) {
            int quotient, remainder, index;
            var visited = new List<Node>();
            while (true) {
                dividend = dividend * 10;
                quotient = dividend / divisor;
                remainder = dividend % divisor;
                dividend = remainder;
                var node = new Node(quotient, remainder);
                if (remainder == 0) {
                    // Print the non-repeating sequence
                    var builder = new StringBuilder("0.");
                    for (int i = 0; i < visited.Count; i++) {
                        builder.Append(visited[i].Quotient);
                    }
                    builder.Append(quotient);
                    return builder.ToString();
                }
                else if ((index = visited.IndexOf(node)) >= 0) {
                    // Print the repeating sequence
                    var builder = new StringBuilder("0.");
                    for (int i = 0; i < index; i++) {
                        builder.Append(visited[i].Quotient);
                    }
                    builder.Append('(');
                    for (int i = index; i < visited.Count; i++) {
                        builder.Append(visited[i].Quotient);
                    }
                    builder.Append(')');
                    return builder.ToString();
                }
                visited.Add(node);
            }
        }
    }
}
