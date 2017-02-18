using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.ComponentModel;
using System.Windows.Input;

namespace Parakeet.Infrastructure
{
    public class MethodCommandDescriptor<TValue> : PropertyDescriptor
    {
        private readonly MethodInfo methodInfo;
        public MethodCommandDescriptor(MethodInfo methodInfo)
            : base(methodInfo.Name, null)
        {
            this.methodInfo = methodInfo;
        }

        public override object GetValue(object component)
        {
            return new DelegateCommand(() => methodInfo.Invoke(((IHasTValue<TValue>)component).Value, null));
        }
        public override void SetValue(object component, object value)
        {
            throw new NotSupportedException("SetValue");
        }
        public override void ResetValue(object component)
        {
            throw new NotSupportedException("ResetValue");
        }
        public override bool IsReadOnly
        {
            get { return true; }
        }
        public override bool CanResetValue(object component)
        {
            return false;
        }
        public override Type PropertyType
        {
            get { return typeof(ICommand); }
        }
        public override bool ShouldSerializeValue(object component)
        {
            return false;
        }
        public override Type ComponentType
        {
            get
            {
                return typeof(TValue);
            }
        }
    }
}
