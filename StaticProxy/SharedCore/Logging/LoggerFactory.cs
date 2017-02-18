using System;

namespace PhillipScottGivens.SharedCore.Logging
{
    public class LoggerFactory<TCategory, TPriority>
        where TCategory : struct, IConvertible
        where TPriority : struct, IConvertible
    {
        private ILogger logger;

        protected LoggerFactory(ILogger logger)
        {
            this.logger = logger;
        }

        public ComponentLogger<TCategory, TPriority> Create(TCategory defaultCategory, TPriority defaultPriority)
        {
            return new ComponentLogger<TCategory, TPriority>(logger, defaultCategory, defaultPriority);
        }
    }
}
