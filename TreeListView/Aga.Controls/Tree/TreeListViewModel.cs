using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;

namespace Aga.Controls.Tree {
    public class TreeListViewModel : ObservableCollectionAdv<TreeNode> {
        #region Properties
        private readonly ITreeAdaptor _treeAdaptor;
        #endregion

        public TreeListViewModel(ITreeAdaptor treeAdaptor) {
            _treeAdaptor = treeAdaptor;

            TreeNode.CreateRoot(this);
        }

        internal IEnumerable GetChildren(TreeNode parent) {
            return _treeAdaptor.GetChildren(parent.Value);
        }

        internal bool HasChildren(TreeNode parent) {
            return _treeAdaptor.HasChildren(parent.Value);
        }
    }
}
