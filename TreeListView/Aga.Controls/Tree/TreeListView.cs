using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Data;
using System.Collections.ObjectModel;
using System.Collections;
using System.ComponentModel;
using System.Collections.Specialized;
using System.Windows.Input;
using System.Windows.Controls.Primitives;

namespace Aga.Controls.Tree {
    public class TreeListView : ListView {
        public TreeListView() {
            ItemContainerGenerator.StatusChanged += ItemContainerGeneratorStatusChanged;
        }

        void ItemContainerGeneratorStatusChanged(object sender, EventArgs e) {
            if (ItemContainerGenerator.Status == GeneratorStatus.ContainersGenerated && PendingFocusNode != null) {
                var item = ItemContainerGenerator.ContainerFromItem(PendingFocusNode) as TreeListViewItem;
                if (item != null)
                    item.Focus();
                PendingFocusNode = null;
            }
        }

        internal TreeNode PendingFocusNode { get; set; }

        protected override DependencyObject GetContainerForItemOverride() {
            return new TreeListViewItem(this);
        }

        protected override bool IsItemItsOwnContainerOverride(object item) {
            return item is TreeListViewItem;
        }

        protected override void PrepareContainerForItemOverride(DependencyObject element, object item) {
            var ti = element as TreeListViewItem;
            var node = item as TreeNode;
            if (ti != null && node != null) {
                ti.Node = node;
                base.PrepareContainerForItemOverride(element, node.Value);
            }
            else {
                base.PrepareContainerForItemOverride(element, item);
            }
        }

        public TreeListViewModel Model {
            set {
                ItemsSource = value;
            }
        }
    }
}
