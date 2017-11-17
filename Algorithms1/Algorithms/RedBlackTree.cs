using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Algorithms {
    public class RedBlackTree {

        #region Nested Classes
        private class Node {
            public Node(int value) { Value = value; }
            public int Value { get; private set; }
            public Color Color { get; set; } = Color.Red;
            private Node[] children = new Node[2];
            public virtual Node this[Side side] {
                get { return children[(int)side]; }
                set { children[(int)side] = value; }
            }
            public override string ToString() {
                return string.Format("{{{0}{1}",
                    Value,
                    Color == Color.Red
                        ? "R"
                        : Color == Color.Black
                        ? "B"
                        : "DB");
            }
        }

        private class SentinelNode : Node {
            public SentinelNode(Color color) : base(0) {
                Color = color;
            }
            public SentinelNode() : base(0) {
                Color = Color.DoubleBlack;
            }
            public override Node this[Side side] {
                get {
                    throw new NotSupportedException("SentinalNodes do not have children");
                }

                set {
                    throw new NotSupportedException("SentinalNodes do not have children");
                }
            }
            public override string ToString() {
                return "SN";
            }
        }

        private class NodeMeta {
            public NodeMeta(Node node) {
                Node = node;
            }
            public NodeMeta(NodeMeta parentMeta, Node node, Side sideFromParent, Node sibling) {
                this.ParentMeta = parentMeta;
                this.Node = node;
                this.SideFromParent = sideFromParent;
                this.Sibling = sibling;
            }
            public void ReplaceNode(Node node) {
                Node = node;
            }
            public Node Node { get; private set; }
            public NodeMeta ParentMeta { get; private set; }
            public Node Sibling { get; private set; }
            public Side SideFromParent { get; private set; }
            public override string ToString() {
                return string.Format("[N={0};P={1};S={2};Side={3}]",
                    Node,
                    ParentMeta == null ? "root" : ParentMeta.Node.ToString(),
                    Sibling,
                    SideFromParent);
            }
        }
        public enum Color { Red, Black, DoubleBlack }
        public enum Side { Left = 0x0, Right = 0x1 }

        #endregion

        #region Data
        private Node root;
        #endregion

        #region Has Value
        public bool HasValue(int value) {
            return HasValue(root, value);
        }
        private bool HasValue(Node node, int value) {
            if (node == null) return false;
            if (value == node.Value) return true;
            if (value < node.Value) return HasValue(node[Side.Left], value);
            else return HasValue(node[Side.Right], value);
        }
        #endregion

        #region Insert
        public void Insert(int value) {
            var newNode = new Node(value);
            if (root == null) {
                SetRoot(newNode);
            }
            else {
                var meta = new NodeMeta(root);
                Insert(meta, newNode);
            }
        }

        private void Insert(NodeMeta meta, Node newNode) {

            var node = meta.Node;
            Side side = newNode.Value < node.Value ? Side.Left : Side.Right;
            Node child = node[side];

            if (child == null) {
                node[side] = newNode;
                var childMeta = new NodeMeta(meta, newNode, side, node[OtherSide(side)]);
                BalanceTree(childMeta);
            }
            else {
                var childMeta = new NodeMeta(meta, child, side, node[OtherSide(side)]);
                Insert(childMeta, newNode);
            }
        }
        #endregion

        #region Delete
        public void Delete(int value) {
            if (root == null) {
                // Do nothing. Not found.
            }
            else {
                var meta = new NodeMeta(root);
                var removed = Remove(meta, value);
                BalanceTree(removed);
            }
        }

        private NodeMeta Remove(NodeMeta meta) {
            Node node = meta.Node;
            Side side = Side.Left;
            Node child = node[side];
            if (child == null) {
                side = Side.Right;
                child = node[side];
            }
            if (child == null) {

                // adjust color through parent. 
                if (node.Color == Color.Black) {
                    // Black-Black 
                    var sentinel = new SentinelNode();
                    meta.ParentMeta.Node[meta.SideFromParent] = sentinel;
                    var sentinelMeta = new NodeMeta(meta.ParentMeta, sentinel, meta.SideFromParent, meta.Sibling);
                    return sentinelMeta;
                }
                else {
                    // Red with Black Parent. Done.
                    meta.ParentMeta.Node[meta.SideFromParent] = null;
                    return null;
                }
            }
            else {
                NodeMeta leaf = SwapWithAdjacent(meta, side);
                return Remove(leaf);
            }
        }

        private NodeMeta Remove(NodeMeta meta, int value) {
            // 1. Swap with Leaf
            // 2. Remove the node, replace with Sentinel
            // 3. Balance the tree. 


            Node node = meta.Node;
            if (node.Value == value) {
                return Remove(meta);
            }
            else {
                Side side = value < node.Value ? Side.Left : Side.Right;
                Side otherSide = OtherSide(side);
                Node next = node[side];
                if (next == null)
                    throw new InvalidOperationException("Value not found.");
                return Remove(new NodeMeta(meta, next, side, node[otherSide]), value);
            }
        }
        #endregion

        #region To String
        public override string ToString() {

            StringBuilder builder = new StringBuilder();
            NodeToString(builder, root, 0);
            return builder.ToString();
        }

        private void NodeToString(StringBuilder builder, Node node, int level) {
//            builder.Append('|');
//            builder.Append('\t', level);
//            if (node == null) {
//                builder.AppendLine("null");
//            }
//            else {
//                builder.AppendLine(node.ToString());
//                NodeToString(builder, node[Side.Left], level + 1);
//                NodeToString(builder, node[Side.Right], level + 1);
//            }
        }
        #endregion

        #region BalanceTree
        private void BalanceTree(NodeMeta meta) {
            if (meta == null)
                return;

            var current = meta.Node;
            var parentMeta = meta.ParentMeta;
            NodeMeta nextMeta = parentMeta;
            if (current.Color == Color.DoubleBlack) {
                Side otherSide = OtherSide(meta.SideFromParent);
                Node distalNephew = meta.Sibling[otherSide];
                Node proximalNephew = meta.Sibling[meta.SideFromParent];

                if (meta.Sibling.Color == Color.Black) {
                    if (distalNephew != null && distalNephew.Color == Color.Red) {
                        RemoveDoubleBlack(meta);
                        nextMeta = RotateToChild(meta.SideFromParent, meta.ParentMeta);
                        meta.Sibling.Color = Color.Red;
                        distalNephew.Color = Color.Black;
                    }
                    else if (proximalNephew != null && proximalNephew.Color == Color.Red) {
                        var siblingMeta = new NodeMeta(meta.ParentMeta, meta.Sibling, otherSide, current);
                        nextMeta = RotateToChild(otherSide, siblingMeta);
                        proximalNephew.Color = Color.Black;
                        meta.Sibling.Color = Color.Red;

                        // Re-run this algorithm to process the distalNephew case. 
                        nextMeta = meta;
                    }
                    else {
                        RemoveDoubleBlack(meta);
                        meta.Sibling.Color = Color.Red;
                        meta.ParentMeta.Node.Color = meta.ParentMeta.Node.Color == Color.Red ? Color.Black : Color.DoubleBlack;
                        //nextMeta =  new NodeMeta(meta.ParentMeta, meta.Sibling, OtherSide(meta.SideFromParent), meta.Sibling);
                    }
                }
                else {
                    RemoveDoubleBlack(meta);
                    nextMeta = RotateToChild(meta.SideFromParent, meta.ParentMeta);
                    meta.Sibling.Color = Color.Black;
                    //meta.ParentMeta.Node.Color = Color.Red;
                    if (proximalNephew != null) proximalNephew.Color = Color.Red;
                }
            }
            else if (current.Color == Color.Red) {
                if (parentMeta.Node.Color == Color.Red) {
                    if (parentMeta.Sibling != null && parentMeta.Sibling.Color == Color.Red) {
                        // Papa and Uncle are Red, but Grandpa is black. Swap colors for everyone. 
                        parentMeta.Sibling.Color = Color.Black;
                        parentMeta.Node.Color = Color.Black;
                        SetRed(parentMeta.ParentMeta.Node);

                        // Skip a generation.
                        nextMeta = parentMeta.ParentMeta;
                    }
                    else if (meta.SideFromParent == parentMeta.SideFromParent) {
                        // Handle Left-Left or Right-Right
                        //http://www.geeksforgeeks.org/red-black-tree-set-2-insert/

                        var newSibling = parentMeta.ParentMeta.Node;
                        nextMeta = RotateToChild(OtherSide(meta.SideFromParent), parentMeta.ParentMeta);
                        //current.Color = Color.Black;
                        SetRed(newSibling);

                        //// TODO: Scrutinize this code, Why am I throwing away 'nextMeta' after retrieving 
                        //// it from Rotate. Also, isn't 'parentMeta' out of sync after the rotate. 
                        //nextMeta = RotateUpOne(parentMeta);
                        //parentMeta.Node.Color = Color.Black;
                        //SetRed(parentMeta.ParentMeta.Node);
                    }
                    else {
                        // Handle Left-Right or Right-Left
                        //http://www.geeksforgeeks.org/red-black-tree-set-2-insert/

                        // We know that our sibling is null 
                        // We know that we have a grandparent. 
                        nextMeta = RotateUpTwo(meta);

                        meta.Node.Color = Color.Black;
                        SetRed(meta.ParentMeta.ParentMeta.Node);
                    }
                }
                else if (meta.Sibling != null && meta.Sibling.Color == Color.Red) {
                    meta.Node.Color = Color.Black;
                    meta.Sibling.Color = Color.Black;
                    SetRed(meta.ParentMeta.Node);
                }
            }
            BalanceTree(nextMeta);
        }
        #endregion

        #region Node relocation program.
        /// <summary>
        /// Take the Top node and make it the left or right child. 
        /// Take the left or right child and make it top node. 
        /// </summary>
        /// <param name="side"></param>
        /// <param name="meta"></param>
        /// <returns></returns>
        private NodeMeta RotateToChild(Side side, NodeMeta meta) {
            var otherSide = OtherSide(side);

            // Three nodes to rotate
            Node oldParent = meta.Node;
            Node newParent = oldParent[otherSide];
            Node child = newParent[side];

            newParent[side] = oldParent;
            //            newParent[otherSide] = oldParent[otherSide];
            oldParent[otherSide] = child;

            NodeMeta grandParentMeta = meta.ParentMeta;
            if (grandParentMeta == null) {
                SetRoot(newParent);
            }
            else {
                Node grandParent = grandParentMeta.Node;
                grandParent[meta.SideFromParent] = newParent;
            }

            return grandParentMeta;
        }

        private NodeMeta RotateUpTwo(NodeMeta meta) {
            // TODO Great grand parent needs to point to node. 

            var target = meta.Node;
            var child = target[OtherSide(meta.SideFromParent)];

            target[OtherSide(meta.SideFromParent)] = meta.ParentMeta.Node;
            meta.ParentMeta.Node[meta.SideFromParent] = child;
            target[meta.SideFromParent] = meta.ParentMeta.ParentMeta.Node;
            meta.ParentMeta.ParentMeta.Node[OtherSide(meta.SideFromParent)] = meta.ParentMeta.Sibling;

            NodeMeta parentMeta = meta.ParentMeta;
            NodeMeta grandParentMeta = parentMeta.ParentMeta;
            NodeMeta greatGrandParentMeta = grandParentMeta.ParentMeta;
            if (greatGrandParentMeta == null) {
                root = target;
                root.Color = Color.Black;
                return null;
            }

            greatGrandParentMeta.Node[grandParentMeta.SideFromParent] = target;
            return greatGrandParentMeta;
        }

        private NodeMeta RotateUpOne(NodeMeta meta) {
            // TODO Great grand parent needs to point to node. 

            var target = meta.Node;
            var child = target[OtherSide(meta.SideFromParent)];

            target[OtherSide(meta.SideFromParent)] = meta.ParentMeta.Node;
            meta.ParentMeta.Node[meta.SideFromParent] = child;

            NodeMeta parentMeta = meta.ParentMeta;
            NodeMeta grandParentMeta = parentMeta.ParentMeta;
            if (grandParentMeta == null) {
                SetRoot(target);
                return null;
            }

            grandParentMeta.Node[parentMeta.SideFromParent] = target;
            return grandParentMeta;
        }

        private NodeMeta SwapWithAdjacent(NodeMeta meta, Side side) {
            Side otherSide = OtherSide(side);
            NodeMeta adjacentMeta = Adjacent(side, meta);
            Node adjacent = adjacentMeta.Node;
            Node toBeReplaced = meta.Node;


            Node adjacentParent = adjacentMeta.ParentMeta.Node;


            // Swap the nodes. 
            // Cache the children of item to be replaced. 
            Node child = adjacent[side];

            bool parentChildSwap = toBeReplaced[side] == adjacent;

            // Adjacent takes place of node to be removed. 
            meta.ReplaceNode(adjacent);
            if (meta.ParentMeta == null) { root = adjacent; }
            else { meta.ParentMeta.Node[meta.SideFromParent] = adjacent; }
            if (!parentChildSwap) { adjacent[side] = toBeReplaced[side]; }
            adjacent[otherSide] = toBeReplaced[otherSide];

            // Put node to be replaced in adjacent spot            
            if (parentChildSwap) { adjacent[otherSide] = toBeReplaced; }
            else { adjacentParent[otherSide] = toBeReplaced; }
            toBeReplaced[side] = child;
            toBeReplaced[otherSide] = null; // This is null, that is how we know it is the leaf
            adjacentMeta.ReplaceNode(toBeReplaced);

            // Swap the color
            Color temp = toBeReplaced.Color;
            toBeReplaced.Color = adjacent.Color;
            adjacent.Color = temp;
            if (meta.ParentMeta == null) { adjacent.Color = Color.Black; }

            if (child == null) {

                // We have found the node to remove. Replace it with a sentinal
                return adjacentMeta;
            }
            else {
                return SwapWithAdjacent(adjacentMeta, side);
            }
        }
        #endregion

        #region Helpers
        private static Side OtherSide(Side side) {
            return side == Side.Left ? Side.Right : Side.Left;
        }

        private void RemoveDoubleBlack(NodeMeta meta) {
            Node node = meta.Node;
            if (node is SentinelNode) { meta.ParentMeta.Node[meta.SideFromParent] = null; }
            else { node.Color = Color.Black; }
        }

        private void SetRed(Node node) {
            if (node != root) node.Color = Color.Red;
        }

        private void SetRoot(Node node) {
            root = node;
            root.Color = Color.Black;
        }
        #endregion

        #region Traversing
        private NodeMeta Adjacent(Side side, NodeMeta meta) {
            Node node = meta.Node;
            Side otherSide = OtherSide(side);
            var adjacent = node[side];
            var adjacentMeta = new NodeMeta(meta, adjacent, side, node[otherSide]);
            for (Node iter = adjacent; iter != null; iter = iter[otherSide]) {
                adjacent = iter;
                adjacentMeta = new NodeMeta(adjacentMeta, adjacent, otherSide, adjacentMeta.Node[side]);
            }
            return adjacentMeta;
        }
        #endregion        
    }
}
