using CleanTF2.Core.Utilities;

namespace CleanTF2.Core.Libraries
{
    /// <summary>
    /// <seealso href="https://developer.valvesoftware.com/wiki/HLLib"></seealso> 
    /// </summary>
    public class HLExtract : IHLExtract
    {
        private readonly IProcessRunner _processRunner;

        public HLExtract(IProcessRunner processRunner)
        {
            _processRunner = processRunner;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="package">Package file to open.</param>
        /// <param name="extractDirectory">Directory to extract content to (defaults to the package's directory).</param>
        /// <param name="pathsToExtract">Paths to a file for folder in the package to extract.</param>
        /// <param name="useFileMapping"></param>
        /// <param name="allowVolatileAccess"></param>
        /// <param name="useSilentMode"></param>
        /// <returns></returns>
        public async Task Run(string package, string extractDirectory, IEnumerable<string> pathsToExtract, bool useFileMapping = false, bool allowVolatileAccess = false, bool useSilentMode = false)
        {
            var args = new List<string>
            {
                "-p", package,
                "-d", extractDirectory
            };

            foreach (var path in pathsToExtract)
            {
                args.Add("-e");
                args.Add(path);
            }

            if (useFileMapping)
            {
                args.Add("-m");
            }
            if (allowVolatileAccess)
            {
                args.Add("-v");
            }
            if (useSilentMode)
            {
                args.Add("-s");
            }

            // TODO: configure path to executable
            await _processRunner.Run(Path.Combine("lib", "hlextract.exe"), args);
        }
    }
}
