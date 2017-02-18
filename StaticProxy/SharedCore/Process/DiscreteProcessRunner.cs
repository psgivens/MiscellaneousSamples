using System;
using System.Diagnostics;
using System.Text;

namespace PhillipScottGivens.SharedCore
{
    public class DiscreteProcessRunner : IDiscreteProcessRunner
    {
        #region Fields
        public const int MinutesToWait = 1;
        private static readonly int DefaultTimeout;
        private int timeout = DefaultTimeout;
        #endregion

        #region Initialize and Teardown
        static DiscreteProcessRunner()
        {
            DefaultTimeout = (int)TimeSpan.FromMinutes(MinutesToWait).TotalMilliseconds;
        }
        #endregion

        #region Static Methods
        public ProcessStartInfo CreateInfo(string process, string command)
        {
            var processInfo = new ProcessStartInfo(process, command)
            {
                RedirectStandardInput = true,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true,
            };
            return processInfo;
        }
        #endregion

        #region Execute Methods
        public DiscreteProcessResult Execute(string process, string command, int timeout)
        {
            this.timeout = timeout;
            return Execute(process, command);
        }
        public DiscreteProcessResult Execute(string process, string command)
        {
            return Execute(CreateInfo(process, command));
        }
        public DiscreteProcessResult Execute(ProcessStartInfo processInfo)
        {
            var error = new StringBuilder();
            var output = new StringBuilder();

            using (Process commandProcess = new Process())
            {
                bool isTimedOut = false;

                commandProcess.StartInfo = processInfo;

                // Because we want the output of both streams and to apply a timeout,
                // we must read the streams asynchronously
                
                commandProcess.OutputDataReceived += (sender, e) => output.AppendLine(e.Data);
                commandProcess.ErrorDataReceived += (sender, e) => error.AppendLine(e.Data);

                // Setup and execute the process
                
                commandProcess.Start();

                commandProcess.BeginOutputReadLine();
                commandProcess.BeginErrorReadLine();
                

                // Wait for the process or kill it. 
                if (!commandProcess.WaitForExit(timeout))
                {
                    isTimedOut = true;
                    commandProcess.Kill();
                    commandProcess.WaitForExit();
                }

                return new DiscreteProcessResult(processInfo.FileName,
                    processInfo.Arguments,
                    commandProcess.ExitCode < 0,
                    isTimedOut,
                    output.ToString(),
                    error.ToString());
            }
        }
        #endregion
    }
}
