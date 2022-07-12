namespace CleanTF2.Core
{
    /// <summary>
    /// <seealso href="https://developer.valvesoftware.com/wiki/HLLib"></seealso> 
    /// </summary>
    public class HLExtract
    {
        private readonly IProcessRunner _processRunner;

        public HLExtract(IProcessRunner processRunner)
        {
            _processRunner = processRunner;
        }

        public void Run(string package, string extractDirectory, string pathToExtract, bool useFileMapping = false, bool allowVolatileAccess = false, bool useSilentMode = false)
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

            _processRunner.Start("hlextract.exe", args);
        }
    }
}
