using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhillipScottGivens.Common
{
    public static class ExtensionMethods
    {
        public static TInstance Guard<TInstance>(this TInstance instance)
        {
            if (instance == null)
                throw new ArgumentNullException(string.Format("typeof({0})", typeof(TInstance)));
            return instance;
        }
    }
}
