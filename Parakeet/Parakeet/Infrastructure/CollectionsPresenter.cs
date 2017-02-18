using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using Parakeet.DataModel;
using System.Collections.Specialized;
using System.Collections;
using System.Reflection;
using System.Linq.Expressions;
using System.ComponentModel;
using Parakeet.Infrastructure;
using System.Windows.Input;

namespace Parakeet.Infrastructure
{
    public class CollectionsPresenter<TValue, TCollection, TItem> : Presenter<TValue>
        where TCollection : IEnumerable<TItem>, INotifyCollectionChanged
    {
        private readonly PropertyInfo selectedItemProperty;

        public CollectionsPresenter(
            TValue value,
            Func<TValue, TCollection> getCollection,
            Expression<Func<TValue, TItem>> accessItem)
            : base(value)
        {
            Messages = getCollection(value);
            selectedItemProperty = ReflectionHelper.GetPropertyInfo<TValue, TItem>(value, accessItem);
        }

        public TCollection Messages { get; private set; }

        public TItem SelectedItem
        {
            get
            {
                return (TItem)selectedItemProperty.GetValue(Value, null);
            }
            set
            {
                selectedItemProperty.SetValue(this.Value, value, null);
            }
        }
    }
}
