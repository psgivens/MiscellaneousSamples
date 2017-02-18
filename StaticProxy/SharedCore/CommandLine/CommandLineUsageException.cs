using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace PhillipScottGivens.SharedCore
{
    public class CommandLineUsageException : ApplicationException
    {
        public CommandLineUsageException()
            : base() { }

        public CommandLineUsageException(string message)
            : base(message) { }

        public CommandLineUsageException(string message, Exception exception)
            : base(message, exception) { }

        public CommandLineUsageException(SerializationInfo info, StreamingContext context)
            : base(info, context) { }
    }
}
