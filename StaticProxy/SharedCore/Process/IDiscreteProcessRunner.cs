using System;
using System.Diagnostics;
namespace PhillipScottGivens.SharedCore
{
    public interface IDiscreteProcessRunner
    {
        ProcessStartInfo CreateInfo(string process, string command);
        DiscreteProcessResult Execute(ProcessStartInfo processInfo);
        DiscreteProcessResult Execute(string process, string command);
        DiscreteProcessResult Execute(string process, string command, int timeout);
    }
}
