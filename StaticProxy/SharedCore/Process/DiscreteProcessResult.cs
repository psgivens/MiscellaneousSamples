using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PhillipScottGivens.SharedCore
{
    public class DiscreteProcessResult
    {
        public string Executable { get; private set; }
        public string Arguments { get; private set; }
        public bool IsTimedOut { get; private set; }
        public bool IsError { get; private set; }
        public bool IsSuccess { get; private set; }
        public string StandardOutput { get; private set; }
        public string ErrorOutput { get; private set; }

        public DiscreteProcessResult(string executable, string arguments, bool isError, bool isTimedOut, string output, string error)
        {
            this.Executable = executable;
            this.Arguments = arguments;
            this.IsError = isError;
            this.IsTimedOut = isTimedOut;
            this.IsSuccess = !isError && !isTimedOut;
            this.StandardOutput = output;
            this.ErrorOutput = error;
        }

   }
}
