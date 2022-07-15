namespace CleanTF2.Core.Utilities
{
    public interface IProcessRunner
    {
        /// <summary>
        /// Starts a process with the given parameters and waits for the process to exit.
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="arguments"></param>
        /// <returns>The process's ExitCode.</returns>
        Task<int> Run(string fileName, IEnumerable<string> arguments);
    }
}
