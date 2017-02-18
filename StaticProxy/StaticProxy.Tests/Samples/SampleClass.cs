using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using PhillipScottGivens.StaticProxy.Templates;

namespace PhillipScottGivens.StaticProxy.UnitTests.Samples
{
    public class SampleClass
    {
        /// <summary>
        /// Method to proxy.
        /// </summary>
        /// <returns>Depends on the proxy</returns>
        public virtual int GetInt()
        {
            return 13;
        }

        public virtual int ThrowException()
        {
            throw new NotSupportedException("ThrowException");
        }

        [Aspect]
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
    }
}
