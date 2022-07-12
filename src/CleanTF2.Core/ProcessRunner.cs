using System.Diagnostics;

namespace CleanTF2.Core
{
    public class ProcessRunner : IProcessRunner
    {
        public Process Start(string fileName, IEnumerable<string> arguments)
        {
            return Process.Start(fileName, arguments);
        }
    }
}
