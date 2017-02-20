using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfVoronoi {
    public class RedBlackTree {
        public enum Color { Red, Black, DoubleBlack }
        public enum Side { Left = 0x0, Right = 0x1 }
        private class Node {
            public Node(int value) {
                this.Value = value;
                Color = Color.Red;
            }
            private Node[] _children = new Node[2];
            public int Value { get; private set; }
            public Color Color { get; set; }
            public Node this[Side side] {
                get {
                    return _children[(int)side];
                }
                set {
                    _children[(int)side] = value;
                }
            }
            public override string ToString() {
                return string.Format("{0}{1}", Value, Color == Color.Red ? "R" : "B");
            }
        }

        public override string ToString() {
            StringBuilder builder = new StringBuilder("::");
            for (Node node = _root; node != null; node = node[Side.Right]) {
                builder.Append(node.ToString()).Append(" -> ");
            }
            builder.Append("null");
            return builder.ToString();
        }

        private class SentinelNode : Node {
            public SentinelNode(Color color) : base(0) { Color = color; }
        }

        private class NodeMeta {
            public NodeMeta(Node node) {
                this.Node = node;
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
        }
        private Node _root;
        public void Insert(int value) {
            Node node = new Node(value);
            InsertRoot(node);
        }
        private void InsertRoot(Node node) {
            if (_root == null) {
                _root = node;
                node.Color = Color.Black;
            }
            else { Insert(new NodeMeta(_root), node); }
        }
        private void Insert(NodeMeta meta, Node newNode) {
            var node = meta.Node;
            Side side = newNode.Value < node.Value ? Side.Left : Side.Right;
            Node child = node[side];

            if (child == null) {
                node[side] = newNode;
                BalanceTree(new NodeMeta(meta, newNode, side, node[OtherSide(side)]));
            }
            else {
                Insert(new NodeMeta(meta, child, side, node[OtherSide(side)]), newNode);
            }
        }

        private void Remove(int value) {
            Remove(new NodeMeta(_root), value);
        }
        private void Remove(NodeMeta meta, int value) {
            Node node = meta.Node;
            if (node.Value == value) {
                Side side = Side.Left;
                Node child = node[side];
                if (child == null) {
                    side = Side.Right;
                    child = node[side];
                }
                if (child == null) {
                    meta.ParentMeta.Node[meta.SideFromParent] = null;
                    // adjust color through parent. 
                    if (node.Color == Color.Black && meta.ParentMeta.Node.Color == Color.Black) {
                        meta.ParentMeta.Node.Color = Color.DoubleBlack;
                        BalanceTree(meta.ParentMeta);
                    }
                }
                else {
                    NodeMeta childMetaData = SwapWithAdjacent(meta, side);
                    BalanceTree(childMetaData);
                }
            }
            else {
                Side side = value < node.Value ? Side.Left : Side.Right;
                Side otherSidde = OtherSide(side);
                Node next = node[side];
                if (next == null)
                    throw new InvalidOperationException("Value not found.");
                Remove(new NodeMeta(meta, next, side, node[otherSidde]), value);
            }
        }

        public static Side OtherSide(Side side) {
            return side == Side.Left ? Side.Right : Side.Left;
        }
        private void BalanceTree(NodeMeta meta) {
            if (meta == null) return;

            var current = meta.Node;
            var parentMeta = meta.ParentMeta;
            NodeMeta nextMeta = parentMeta;
            if (current is SentinelNode) {
                // In all cases, we remove the sentinal. 
                parentMeta.Node[meta.SideFromParent] = null;

                // If the sentinal is Red, nothing changes. 

                if (current.Color == Color.Black) {
                    if (parentMeta.Node.Color == Color.Red) {
                        // Black with Red Parent
                        // We know that sibling is black because Red-Red is invalid, push the black up on both sides. 
                        parentMeta.Node.Color = Color.Black;
                        meta.Sibling.Color = Color.Red;
                    }
                    else if (meta.Sibling.Color == Color.Red) {
                        // Black, Black Parent and Red Sibling
                        nextMeta = RotateToChild(meta.SideFromParent, new NodeMeta(meta.ParentMeta, meta.Sibling, OtherSide(meta.SideFromParent), null));
                    }
                    else {
                        // Sibling is black, and parent is black. 
                        Node sibbling = meta.Sibling;

                        // Since we are dealing with a Sentinel node, all nephews are Red and baron. 

                        Node proximalNephew = sibbling[meta.SideFromParent];
                        if (proximalNephew != null) {
                            nextMeta = RotateToChild(meta.SideFromParent, parentMeta);
                            // proximalNephew is now the parent. 
                        }
                        else {
                            Node distalNephew = sibbling[OtherSide(meta.SideFromParent)];
                            if (distalNephew != null) {
                                nextMeta = RotateToChild(meta.SideFromParent, parentMeta);
                                distalNephew.Color = Color.Black;
                            }
                            else {
                                parentMeta.Node.Color = Color.DoubleBlack;
                                meta.Sibling.Color = Color.Red;
                            }
                        }
                    }
                }
                else {
                    // Red sentinel can simply be removed. 
                }
            }
            else if (current.Color == Color.DoubleBlack) {
                if (current == _root) {
                    current.Color = Color.Black;
                }
                else if (meta.Sibling.Color == Color.Black) {
                    meta.Sibling.Color = Color.Red;
                    current.Color = Color.Black;
                    parentMeta.Node.Color = parentMeta.Node.Color == Color.Red
                        ? Color.Black
                        : Color.DoubleBlack;
                }
                else {
                    // DoubleBlack, Red Sibling, Black Parent
                    nextMeta = RotateToChild(meta.SideFromParent, parentMeta);
                    parentMeta.Node.Color = Color.Red;
                    meta.Sibling.Color = Color.Red;
                }
                Side side = Side.Left;
                Node child = current[side];
                if (child == null) {
                    side = Side.Right;
                    child = current[side];
                }
            }
            else if (current.Color == Color.Red) {
                if (parentMeta.Node.Color == Color.Red) {
                    if (parentMeta.Sibling != null && parentMeta.Sibling.Color == Color.Red) {
                        nextMeta = parentMeta;
                    }
                    else if (meta.SideFromParent != parentMeta.SideFromParent) {
                        // Handle Left-Right or Right-Left
                        //http://www.geeksforgeeks.org/red-black-tree-set-2-insert/

                        // next meta is now parent. 
                        nextMeta = RotateToGrandChild(meta.SideFromParent, parentMeta.ParentMeta);
                    }
                    else {
                        // Handle Right-Right or Left-Left

                        // nextMeta is now great grand parent. 
                        nextMeta = RotateToChild(OtherSide(meta.SideFromParent), parentMeta.ParentMeta);
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

        private NodeMeta SwapWithAdjacent(NodeMeta meta, Side side) {
            Side otherSide = OtherSide(side);
            NodeMeta adjacentMeta = Adjacent(side, meta);
            Node adjacent = adjacentMeta.Node;
            Node toBeReplaced = meta.Node;


            Node adjacentParent = adjacentMeta.ParentMeta.Node;
            adjacentParent[otherSide] = adjacent[side];

            // Swap the nodes. 
            // Cache the children of item to be replaced. 
            Node child = adjacent[side];

            // Adjacent takes place of node to be removed. 
            meta.ReplaceNode(adjacent);
            meta.ParentMeta.Node[meta.SideFromParent] = adjacent;
            adjacent[side] = toBeReplaced[side];
            adjacent[otherSide] = toBeReplaced[otherSide];

            // Put node to be replaced in adjacent spot
            adjacentParent[otherSide] = toBeReplaced;
            toBeReplaced[side] = child;
            toBeReplaced[otherSide] = null; // This is null, that is how we know it is the leaf
            adjacentMeta.ReplaceNode(toBeReplaced);

            // Swap the color
            Color temp = toBeReplaced.Color;
            toBeReplaced.Color = adjacent.Color;
            adjacent.Color = temp;

            if (child == null) {
                // We have found the node to remove. Replace it with a sentinal
                SentinelNode sentinel = new SentinelNode(toBeReplaced.Color);
                adjacentParent[otherSide] = sentinel;
                return new NodeMeta(adjacentMeta.ParentMeta, sentinel, adjacentMeta.SideFromParent, adjacentMeta.Sibling);
            }
            else {
                return SwapWithAdjacent(adjacentMeta, side);
            }
        }

        /// <summary>
        /// Take the Top node and make it the left or right node. 
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
            newParent[otherSide] = oldParent[otherSide];
            oldParent[otherSide] = child;


            NodeMeta grandParentMeta = meta.ParentMeta.ParentMeta;
            if (grandParentMeta == null) {
                _root = newParent;
                newParent.Color = Color.Black;
            }
            else {
                Node grandParent = grandParentMeta.Node;
                grandParent[meta.ParentMeta.SideFromParent] = newParent;
            }

            return grandParentMeta;
        }

        private NodeMeta RotateToGrandChild(Side side, NodeMeta meta) {
            var otherSide = OtherSide(side);

            // Three nodes to rotate
            Node oldParent = meta.Node;
            Node newParent = oldParent[otherSide][side];
            Node child = newParent[side];


            newParent[side] = oldParent;
            newParent[otherSide] = oldParent[otherSide];
            oldParent[otherSide] = child;

            oldParent[otherSide] = child;
            newParent[side] = oldParent;

            NodeMeta parentMeta = meta.ParentMeta;
            if (parentMeta == null) {
                _root = newParent;
                newParent.Color = Color.Black;
            }
            else {
                Node nextLevelUp = parentMeta.Node;
                nextLevelUp[meta.ParentMeta.SideFromParent] = newParent;
            }

            return parentMeta;
        }

        private NodeMeta Adjacent(Side side, NodeMeta meta) {
            Node node = meta.Node;
            Side otherSide = OtherSide(side);
            var adjacent = node[side];
            var adjacentMeta = new NodeMeta(meta, adjacent, side, node[otherSide]);
            for (Node iter = adjacent; iter != null; iter = iter[otherSide]) {
                adjacent = iter;
                adjacentMeta = new NodeMeta(meta, adjacent, side, node[side]);
            }
            return adjacentMeta;
        }
        private void SetRed(Node node) {
            if (node != _root) node.Color = Color.Red;
        }
    }
}
