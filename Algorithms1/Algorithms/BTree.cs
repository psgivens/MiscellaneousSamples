using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Algorithms
{
    internal class BTree
    {
        private class BTreeNodeMeta
        {
            public BTreeNode Previous { get; set; }
            public BTreeNode Next { get; set; }
            public BTreeNode Current { get; set; }
        }
        private class BTreeListNode {
            public int Value { get; set; }
            public BTreeNode Branch { get; set; }
            public BTreeListNode Next { get; set; }
        }
        private class BTreeNode {
            public BTreeNode After { get; set; }
            public BTreeListNode Root { get; set; }
            public BTreeListNode Find(int value)
            {
                for (BTreeListNode node = Root; node != null; node = node.Next)
                {
                    if (value == node.Value) return node;
                    else if (value < node.Value) return node.Branch.Find(value);
                }
                return After.Find(value);
            }
        }
        private BTreeNode root;
        public void Insert(int value)
        {
            if (root == null)
            {
                root = new BTreeNode
                {
                    Root = new BTreeListNode
                    {
                        Value = value
                    }
                };
            }
            else
            {

            }
        }
    }
}
