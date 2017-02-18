using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Reflection;

namespace Parakeet.Infrastructure
{
    public class Presenter : INotifyPropertyChanged
    {
        protected void RaisePropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
        public event PropertyChangedEventHandler PropertyChanged;
    }

    public class Presenter<TValue> : Presenter, ICustomTypeDescriptor, IHasTValue<TValue>
    {
        public TValue Value { get; private set; }

        public Presenter(TValue value)
        {
            this.Value = value;
        }

        AttributeCollection ICustomTypeDescriptor.GetAttributes()
        {
            return TypeDescriptor.GetAttributes(this);
        }

        string ICustomTypeDescriptor.GetClassName()
        {
            return TypeDescriptor.GetClassName(this);
        }

        string ICustomTypeDescriptor.GetComponentName()
        {
            return TypeDescriptor.GetComponentName(this);
        }

        TypeConverter ICustomTypeDescriptor.GetConverter()
        {
            return TypeDescriptor.GetConverter(this);
        }

        EventDescriptor ICustomTypeDescriptor.GetDefaultEvent()
        {
            return TypeDescriptor.GetDefaultEvent(this);
        }

        PropertyDescriptor ICustomTypeDescriptor.GetDefaultProperty()
        {
            return TypeDescriptor.GetDefaultProperty(this);
        }

        object ICustomTypeDescriptor.GetEditor(Type editorBaseType)
        {
            return TypeDescriptor.GetEditor(this, editorBaseType);
        }

        EventDescriptorCollection ICustomTypeDescriptor.GetEvents(Attribute[] attributes)
        {
            return TypeDescriptor.GetEvents(this, attributes);
        }

        EventDescriptorCollection ICustomTypeDescriptor.GetEvents()
        {
            return TypeDescriptor.GetEvents(this);
        }

        PropertyDescriptorCollection ICustomTypeDescriptor.GetProperties(Attribute[] attributes)
        {
            return TypeDescriptor.GetProperties(this, attributes);
        }

        private PropertyDescriptorCollection _properties;
        PropertyDescriptorCollection ICustomTypeDescriptor.GetProperties()
        {
            if (_properties == null)
            {
                var properties = TypeDescriptor.GetProperties(Value);
                var descriptors = new List<PropertyDescriptor>(properties.OfType<PropertyDescriptor>());
                foreach (MethodInfo method in typeof(TValue).GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance))
                    descriptors.Add(new MethodCommandDescriptor<TValue>(method));
                _properties = new PropertyDescriptorCollection(descriptors.ToArray());
            }
            return _properties;
        }

        object ICustomTypeDescriptor.GetPropertyOwner(PropertyDescriptor pd)
        {
            return this;
        }
    }
}
