using System;

namespace PhillipScottGivens.SharedCore.Logging
{
    public class ConsoleLogger : ILogger
    {
        #region Fields
        private const string FormatString = "[{0},{1}] {2}";
        #endregion

        #region Log Methods
        public void Log(Enum category, Enum priority, string message, params object[] args)
        {
            if (args.Length > 0)
                message = string.Format(message, args);
            Console.WriteLine(FormatString, category.ToString(), priority.ToString(), message);
        }

        public void Log(Enum category, Enum priority, Exception exception, string message, params object[] args)
        {
            if (args.Length > 0)
                message = string.Format(message, args);
            Console.WriteLine(FormatString, string.Format("{0} \n {1}", message, exception.ToString()));
        }
        #endregion
    }
}
