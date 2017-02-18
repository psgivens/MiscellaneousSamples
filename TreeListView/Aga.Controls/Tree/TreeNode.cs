using System;
using System.Collections.Generic;
using System.Text;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Collections.Specialized;

namespace Aga.Controls.Tree {
    public sealed class TreeNode : INotifyPropertyChanged {
        #region NodeCollection
        private class NodeCollection : Collection<TreeNode> {
            private TreeNode _owner;

            public NodeCollection(TreeNode owner) {
                _owner = owner;
            }

            protected override void ClearItems() {
                while (this.Count != 0)
                    this.RemoveAt(this.Count - 1);
            }

            protected override void InsertItem(int index, TreeNode item) {
                if (item == null)
                    throw new ArgumentNullException("item");

                if (item.Parent != _owner) {
                    if (item.Parent != null)
                        item.Parent.Children.Remove(item);
                    item._parent = _owner;
                    item._index = index;
                    for (int i = index; i < Count; i++)
                        this[i]._index++;
                    base.InsertItem(index, item);
                }
            }

            protected override void RemoveItem(int index) {
                TreeNode item = this[index];
                item._parent = null;
                item._index = -1;
                for (int i = index + 1; i < Count; i++)
                    this[i]._index--;
                base.RemoveItem(index);
            }

            protected override void SetItem(int index, TreeNode item) {
                if (item == null)
                    throw new ArgumentNullException("item");
                RemoveAt(index);
                InsertItem(index, item);
            }
        }
        #endregion

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string name) {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(name));
        }

        #endregion

        #region Properties

        private TreeListViewModel _tree;

        private INotifyCollectionChanged _childrenSource;
        internal INotifyCollectionChanged ChildrenSource {
            get { return _childrenSource; }
            set {
                if (_childrenSource != null)
                    _childrenSource.CollectionChanged -= ChildrenChanged;

                _childrenSource = value;

                if (_childrenSource != null)
                    _childrenSource.CollectionChanged += ChildrenChanged;
            }
        }

        private int _index = -1;
        public int Index {
            get {
                return _index;
            }
        }

        /// <summary>
        /// Returns true if all parent nodes of this node are expanded.
        /// </summary>
        internal bool IsVisible {
            get {
                TreeNode node = _parent;
                while (node != null) {
                    if (!node.IsExpanded)
                        return false;
                    node = node.Parent;
                }
                return true;
            }
        }

        private bool _isExpandedOnce;

        public bool HasChildren {
            get;
            internal set;
        }

        private bool _isExpanded;
        public bool IsExpanded {
            get { return _isExpanded; }
            set {
                if (value != IsExpanded) {
                    if (value) {
                        if (!_isExpandedOnce) {
                            _isExpandedOnce = true;
                            AssignIsExpanded(value);
                            CreateChildrenNodes();
                        }
                        else {
                            AssignIsExpanded(value);
                            CreateChildrenRows();
                        }
                    }
                    else {
                        DropChildrenRows(false);
                        AssignIsExpanded(value);
                    }
                    OnPropertyChanged("IsExpanded");
                    OnPropertyChanged("IsExpandable");
                }
            }
        }

        internal void AssignIsExpanded(bool value) {
            _isExpanded = value;
        }

        public bool IsExpandable {
            get {
                return (HasChildren && !_isExpandedOnce) || Nodes.Count > 0;
            }
        }

        private bool _isSelected;
        public bool IsSelected {
            get { return _isSelected; }
            set {
                if (value != _isSelected) {
                    _isSelected = value;
                    OnPropertyChanged("IsSelected");
                }
            }
        }


        private TreeNode _parent;
        public TreeNode Parent {
            get { return _parent; }
        }

        public int Level {
            get {
                if (_parent == null)
                    return -1;
                else
                    return _parent.Level + 1;
            }
        }

        public TreeNode PreviousNode {
            get {
                if (_parent != null) {
                    int index = Index;
                    if (index > 0)
                        return _parent.Nodes[index - 1];
                }
                return null;
            }
        }

        public TreeNode NextNode {
            get {
                if (_parent != null) {
                    int index = Index;
                    if (index < _parent.Nodes.Count - 1)
                        return _parent.Nodes[index + 1];
                }
                return null;
            }
        }

        internal TreeNode BottomNode {
            get {
                TreeNode parent = this.Parent;
                if (parent != null) {
                    if (parent.NextNode != null)
                        return parent.NextNode;
                    else
                        return parent.BottomNode;
                }
                return null;
            }
        }

        internal TreeNode NextVisibleNode {
            get {
                if (IsExpanded && Nodes.Count > 0)
                    return Nodes[0];
                else {
                    TreeNode nn = NextNode;
                    if (nn != null)
                        return nn;
                    else
                        return BottomNode;
                }
            }
        }

        public int VisibleChildrenCount {
            get {
                return AllVisibleChildren.Count();
            }
        }

        public IEnumerable<TreeNode> AllVisibleChildren {
            get {
                int level = this.Level;
                TreeNode node = this;
                while (true) {
                    node = node.NextVisibleNode;
                    if (node != null && node.Level > level)
                        yield return node;
                    else
                        break;
                }
            }
        }

        private object _value;
        public object Value {
            get { return _value; }
        }

        private Collection<TreeNode> _children;
        internal Collection<TreeNode> Children {
            get { return _children; }
        }

        private ReadOnlyCollection<TreeNode> _nodes;
        public ReadOnlyCollection<TreeNode> Nodes {
            get { return _nodes; }
        }

        #endregion

        private readonly bool _isRootNode;

        internal static void CreateRoot(TreeListViewModel tree) {
            var root = new TreeNode(tree) {
                IsExpanded = true
            };
            root.Children.Clear();
        }

        private TreeNode(TreeListViewModel tree)
            : this(tree, null) {
            _isRootNode = true;
        }
        internal TreeNode(TreeListViewModel tree, object tag) {
            if (tree == null)
                throw new ArgumentNullException("tree");

            _tree = tree;
            _children = new NodeCollection(this);
            _nodes = new ReadOnlyCollection<TreeNode>(_children);
            _value = tag;
        }

        public override string ToString() {
            if (Value != null)
                return Value.ToString();
            else
                return base.ToString();
        }

        void ChildrenChanged(object sender, NotifyCollectionChangedEventArgs e) {
            switch (e.Action) {
                case NotifyCollectionChangedAction.Add:
                    if (e.NewItems != null) {
                        int index = e.NewStartingIndex;
                        int rowIndex = _tree.IndexOf(this);
                        foreach (object obj in e.NewItems) {
                            InsertNewNode(this, obj, rowIndex, index);
                            index++;
                        }
                    }
                    break;

                case NotifyCollectionChangedAction.Remove:
                    if (Children.Count > e.OldStartingIndex)
                        RemoveChildAt(e.OldStartingIndex);
                    break;

                case NotifyCollectionChangedAction.Move:
                case NotifyCollectionChangedAction.Replace:
                case NotifyCollectionChangedAction.Reset:
                    while (Children.Count > 0)
                        RemoveChildAt(0);
                    CreateChildrenNodes();
                    break;
            }
            HasChildren = Children.Count > 0;
            OnPropertyChanged("IsExpandable");
        }

        private void RemoveChildAt(int index) {
            var child = Children[index];
            child.DropChildrenRows(true);
            ClearChildrenSource(child);
            Children.RemoveAt(index);
        }

        private void ClearChildrenSource(TreeNode node) {
            node.ChildrenSource = null;
            foreach (var n in node.Children)
                ClearChildrenSource(n);
        }

        internal void CreateChildrenNodes() {
            var children = _tree.GetChildren(this);
            if (children != null) {
                int rowIndex = _tree.IndexOf(this);
                ChildrenSource = children as INotifyCollectionChanged;
                foreach (object obj in children) {
                    TreeNode child = new TreeNode(_tree, obj);
                    child.HasChildren = _isRootNode || _tree.HasChildren(child);
                    Children.Add(child);
                }
                _tree.InsertRange(rowIndex + 1, Children.ToArray());
            }
        }

        private void CreateChildrenRows() {
            int index = _tree.IndexOf(this);
            if (index >= 0 || _isRootNode) // ignore invisible nodes
			{
                var nodes = AllVisibleChildren.ToArray();
                _tree.InsertRange(index + 1, nodes);
            }
        }

        internal void DropChildrenRows(bool removeParent) {
            int start = _tree.IndexOf(this);
            if (start >= 0 || _isRootNode) // ignore invisible nodes
			{
                int count = VisibleChildrenCount;
                if (removeParent)
                    count++;
                else
                    start++;
                _tree.RemoveRange(start, count);
            }
        }

        internal void InsertNewNode(TreeNode parent, object obj, int rowIndex, int index) {
            TreeNode node = new TreeNode(_tree, obj);
            if (index >= 0 && index < parent.Children.Count)
                parent.Children.Insert(index, node);
            else {
                index = parent.Children.Count;
                parent.Children.Add(node);
            }
            _tree.Insert(rowIndex + index + 1, node);
        }


    }
}
