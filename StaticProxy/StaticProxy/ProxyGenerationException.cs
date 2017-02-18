using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PhillipScottGivens.StaticProxy
{
    public class ProxyGenerationException : Exception
    {
        public ProxyGenerationException(string message)
            : base(message) { }
    }
}
