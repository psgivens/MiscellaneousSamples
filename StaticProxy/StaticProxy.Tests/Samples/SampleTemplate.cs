using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Linq.Expressions;
using PhillipScottGivens.StaticProxy.Templates;

namespace PhillipScottGivens.StaticProxy.UnitTests.Samples
{
    public class SampleTemplate : ClassToProxy
    {
        public SampleTemplate(ProxyTemplateRegistrar<SampleTemplate> registrar)
        {
            registrar.RegisterSelector(SelectAspect);
        }

        private void HandleExceptionsAspect()
        {
            try
            {
                Proxy.Return_Parameter(3 + Proxy.Call_Base() + 3);
            }
            catch
            {
                Console.WriteLine("Hello World");
            }
            Proxy.Return_Parameter(42);
        }

        private Expression<Action<SampleTemplate>> SelectAspect(MethodInfo method)
        {
            return template => template.HandleExceptionsAspect();
        }
    }
}
