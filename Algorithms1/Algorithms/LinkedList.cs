using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Algorithms
{
    public class LinkedList
    {
        private class LinkedListNode
        {
            public int Value { get; private set; }
            public LinkedListNode(int value)
            {
                Value = value;
            }
            public LinkedListNode Next { get; set; }
            public override string ToString()
            {
                return Value.ToString();
            }
        }
        private LinkedListNode Root;
        private LinkedListNode End;
        public void InsertBegining(int value)
        {
            var node = new LinkedListNode(value);
            if (Root == null)
            {
                Root = node;
                End = node;
            }
            else
            {
                node.Next = Root;
                Root = node;
            }
        }
        public void InsertEnd(int value)
        {
            var node = new LinkedListNode(value);
            if (Root == null)
            {
                Root = node;
                End = node;
            }
            else
            {
                End.Next = node;
                End = node;
            }
        }

        public IEnumerable<int> GetValues()
        {
            if (Root == null)
            {
                yield break;
            }
            else
            {
                var node = Root;
                while (node != null)
                {
                    yield return node.Value;
                    node = node.Next;
                }
            }
        }

        public void Reverse()
        {
            if (Root == null) return;
            LinkedListNode pre = null;
            var node = Root;
            var post = node.Next;
            End = Root;
            while (node != null)
            {
                node.Next = pre;
                pre = node;
                node = post;
                if (post != null)
                    post = post.Next;
            }
            Root = pre;
        }


        public override string ToString()
        {
            StringBuilder builder = new StringBuilder("[");
            var enumerator = GetValues().GetEnumerator();
            if (enumerator.MoveNext())
            {
                bool hasMore;
                do
                {
                    builder.Append(enumerator.Current);
                    hasMore = enumerator.MoveNext();
                    if (hasMore)
                        builder.Append(',');
                } while (hasMore);
            }
            builder.Append(']');
            return builder.ToString();
        }
    }
}
