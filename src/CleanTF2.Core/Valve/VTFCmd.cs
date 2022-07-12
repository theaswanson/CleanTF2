using CleanTF2.Core.Utilities;

namespace CleanTF2.Core.Valve
{
    /// <summary>
    /// <seealso href="https://developer.valvesoftware.com/wiki/VTFCmd"></seealso> 
    /// </summary>
    public class VTFCmd : IVTFCmd
    {
        private readonly IProcessRunner _processRunner;

        public VTFCmd(IProcessRunner processRunner)
        {
            _processRunner = processRunner;
        }

        public async Task Run(string file, string? output = null, string? format = null, string? alphaFormat = null, string? exportFormat = null, string? flag = null, string? version = null)
        {
            var args = new List<string>
            {
                "-file", file
            };

            if (!string.IsNullOrWhiteSpace(output))
            {
                args.Add("-output");
                args.Add(output);
            }

            if (!string.IsNullOrWhiteSpace(format))
            {
                args.Add("-format");
                args.Add(format);
            }

            if (!string.IsNullOrWhiteSpace(alphaFormat))
            {
                args.Add("-alphaformat");
                args.Add(alphaFormat);
            }

            if (!string.IsNullOrWhiteSpace(exportFormat))
            {
                args.Add("-exportformat");
                args.Add(exportFormat);
            }

            if (!string.IsNullOrWhiteSpace(flag))
            {
                args.Add("-flag");
                args.Add(flag);
            }

            if (!string.IsNullOrWhiteSpace(version))
            {
                args.Add("-version");
                args.Add(version);
            }

            await _processRunner.Run("VTFCmd.exe", args);
        }
    }
}
