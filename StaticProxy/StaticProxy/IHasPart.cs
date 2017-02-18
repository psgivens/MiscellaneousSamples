using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PhillipScottGivens.StaticProxy
{
    public interface IHasPart<TPart>
    {
        TPart Part { get; }
    }
}
