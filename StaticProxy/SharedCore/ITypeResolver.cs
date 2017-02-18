using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PhillipScottGivens.SharedCore
{
    public interface ITypeResolver
    {
        TType Resolve<TType>();
        object Resolve(Type type);
    }
}
