using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PhillipScottGivens.StaticProxy.Templates
{
    public class Proxy
    {
        //public static int GetValue()
        //{
        //    return 14;
        //}

        //public static int GetOtherValue()
        //{
        //    return 15;
        //}

        public static int Call_Base()
        {
            return 56;
        }

        public static void Return_Parameter(object parameter) { }
    }
}
