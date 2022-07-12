using System.Diagnostics;

namespace CleanTF2.Core.Utilities
{
    public class ProcessRunner : IProcessRunner
    {
        public async Task<int> Run(string fileName, IEnumerable<string> arguments)
        {
            var process = Process.Start(fileName, arguments);
            await process.WaitForExitAsync();
            return process.ExitCode;
        }
    }
}
