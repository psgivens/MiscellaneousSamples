using PhillipScottGivens.StaticProxy.UnitTests.Samples;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace StaticProxy.Tests.Samples
{
    public class DerivedClassToProxy : ClassToProxy
    {
        public virtual int DoSomethingSpecial()
        {
            Console.WriteLine("Fun times");

            ThrowException();

            return 29;
        }
    }
}
