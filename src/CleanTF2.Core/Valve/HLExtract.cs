using CleanTF2.Core.Utilities;

namespace CleanTF2.Core.Valve
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

        public async Task Run(string package, string extractDirectory, string pathToExtract, bool useFileMapping = false, bool allowVolatileAccess = false, bool useSilentMode = false)
        {
            var args = new List<string>
            {
                "-p", package,
                "-d", extractDirectory,
                "-e", pathToExtract,
            };

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

            await _processRunner.Run("hlextract.exe", args);
        }
    }
}
