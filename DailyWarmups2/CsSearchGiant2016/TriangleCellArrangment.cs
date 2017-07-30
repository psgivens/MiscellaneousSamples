using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CsSearchGiatn2016 {
    public class TriangleCellArrangment {
        public TriangleCellArrangment() {
            Console.WriteLine("({0},{1})=>{2}: Expect {3}", 3, 2, GetCellNumber(3, 2), 9);
            Console.WriteLine("({0},{1})=>{2}: Expect {3}", 3, 2, GetCellNumber(5, 10), 96);
        }
        public static int GetCellNumber(int x, int y) {
            int value = 0;
            int k = 0;
            for (int i = 0; i < x; i++) {
                value += ++k;
            }
            for (int j = 1; j < y; j++) {
                value += k++;
            }
            return value;
        }
    }
}
