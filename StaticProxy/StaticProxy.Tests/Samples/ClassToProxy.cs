using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PhillipScottGivens.StaticProxy.Templates;

namespace PhillipScottGivens.StaticProxy.UnitTests.Samples
{
    [AspectTemplate(typeof(SampleTemplate))]
    public class ClassToProxy
    {
        /// <summary>
        /// Method to proxy.
        /// </summary>
        /// <returns>Depends on the proxy</returns>
        public virtual int ReturnThirteen()
        {
            return 13;
        }

        public virtual int ThrowException()
        {
            throw new NotSupportedException("ThrowException");
        }
    }
}
