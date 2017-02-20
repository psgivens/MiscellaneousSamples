using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Algorithms
{
    internal class LinkedListHeap
    {
        LinkedList<int> list = new LinkedList<int>();
        
        public bool HasValues()
        {
            return list.Count > 0;
        }

        private bool FavorFirst(int first, int second)
        {
            return first < second;
        }

    }
}
