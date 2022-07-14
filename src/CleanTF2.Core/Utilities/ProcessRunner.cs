using System.Diagnostics;

namespace CleanTF2.Core.Utilities
{
    public class ProcessRunner : IProcessRunner
    {
        public async Task<int> Run(string fileName, IEnumerable<string> arguments)
        {
            var process = CreateProcess(fileName, arguments, silent: true);
            process.Start();
            process.StandardOutput.ReadToEnd(); // to avoid deadlocks
            await process.WaitForExitAsync();
            return process.ExitCode;
        }

        private static Process CreateProcess(string fileName, IEnumerable<string> arguments, bool silent = true)
        {
            var processStartInfo = new ProcessStartInfo(fileName);

            if (silent)
            {
                processStartInfo.UseShellExecute = false;
                processStartInfo.RedirectStandardOutput = true;
            }

            foreach (var argument in arguments)
            {
                processStartInfo.ArgumentList.Add(argument);
            }

            return new Process
            {
                StartInfo = processStartInfo,
            };
        }
    }
}
