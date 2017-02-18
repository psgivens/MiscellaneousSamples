using System;

namespace PhillipScottGivens.SharedCore
{
    public static class GuardExtensionMethods
    {
        public static TObject Guard<TObject>( this TObject value, string parameterName)
        {
            if (value == null)
                throw new ArgumentNullException(parameterName);

            return value;
        }
    }
}
