using System;
using System.Diagnostics;

namespace PhillipScottGivens.SharedCore.Logging
{
    public class ComponentLogger<TCategory, TPriority>
        where TCategory : struct, IConvertible
        where TPriority : struct, IConvertible
    {
        #region Fields
        private readonly ILogger logger;
        private TCategory initializedCategory;
        private TPriority initializedPriority;
        #endregion

        #region Initialize and Teardown
        internal ComponentLogger(ILogger logger, TCategory category, TPriority priority)
        {
            this.logger = logger;        
            this.initializedCategory = category;
            this.initializedPriority = priority;
        }
        #endregion

        #region Log Message
        public void Log(TCategory category, string message, params object[] args)
        {
            CheckParameters();
            Log(category, initializedPriority, message, args);
        }
        public void Log(TPriority priority, string message, params object[] args)
        {
            CheckParameters();
            Log(initializedCategory, priority, message, args);
        }
        public void Log(string message, params object[] args)
        {
            CheckParameters();
            Log(initializedCategory, initializedPriority, message, args);
        }
        public void Log(TCategory category, TPriority priority, string message, params object[] args)
        {
            CheckParameters();
            logger.Log(category as Enum, priority as Enum, message, args);
        }
        #endregion

        #region LogException
        public void Log(TCategory category, Exception exception, string message, params object[] args)
        {
            CheckParameters();
            Log(category, initializedPriority, exception, message, args);
        }
        public void Log(TPriority priority, Exception exception, string message, params object[] args)
        {
            CheckParameters();
            Log(initializedCategory, priority, exception, message, args);
        }
        public void Log(Exception exception, string message, params object[] args)
        {
            CheckParameters();
            Log(initializedCategory, initializedPriority, exception, message, args);
        }
        public void Log(TCategory category, TPriority priority, Exception exception, string message, params object[] args)
        {
            CheckParameters();
            logger.Log(category as Enum, priority as Enum, exception, message, args);
        }
        #endregion

        [Conditional("DEBUG")]
        private void CheckParameters()
        {
            initializedCategory.Guard("initializedCategory");
            initializedPriority.Guard("initializedPriority");
        }
    }
}
