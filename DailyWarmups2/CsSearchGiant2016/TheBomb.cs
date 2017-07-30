using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CsSearchGiatn2016 {
    public class TheBomb {
        public TheBomb() {
            //Console.WriteLine("({0},{1})=>{2}: Expect {3}", 1, 1, Generations(2, 1, 0), 1);
            Console.WriteLine("({0},{1})=>{2}: Expect {3}", 4, 7, Generations(4, 7, 0), 4);
        }
        public static int Generations(int m, int f, int g) {            
            int larger = Math.Max(m, f);
            int smaller = Math.Min(m, f);
            
            if (smaller == 1) {
                return g + larger - 1;
            }
            if (larger % smaller == 0) {
                return -1;
            }
            return Generations(smaller, larger - smaller, g + 1);
        }
    }
}
