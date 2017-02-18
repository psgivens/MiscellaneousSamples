using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PhillipScottGivens.StaticProxy.Templates
{
    [AttributeUsage(AttributeTargets.Class)]
    public class AspectTemplateAttribute : Attribute
    {
        public Type TemplateType { get; private set; }

        public AspectTemplateAttribute(Type templateType)
        {
            TemplateType = templateType;
        }
    }
}
