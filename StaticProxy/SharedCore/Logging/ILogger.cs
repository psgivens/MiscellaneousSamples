using System;

namespace PhillipScottGivens.SharedCore.Logging
{
    public interface ILogger
    {
        void Log(Enum category, Enum priority, string message, params object[] args);
        void Log(Enum category, Enum priority, Exception exception, string message, params object[] args);
    }
}
