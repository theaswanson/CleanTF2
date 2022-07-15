using CleanTF2.Core.Utilities;

namespace CleanTF2.Core.Valve
{
    /// <summary>
    /// <seealso href="https://developer.valvesoftware.com/wiki/VPK"></seealso> 
    /// </summary>
    public class VPK : IVPK
    {
        private readonly IProcessRunner _processRunner;

        public VPK(IProcessRunner processRunner)
        {
            _processRunner = processRunner;
        }

        public async Task Run(string tf2Directory, string directoryToPack, bool produceMultiChunkVPK = false)
        {
            var args = new List<string>();

            if (produceMultiChunkVPK)
            {
                args.Add("-M");
            }

            args.Add(directoryToPack);

            // TODO: configure path to executable
            await _processRunner.Run(Path.Combine(tf2Directory, "bin", "vpk.exe"), args);
        }
    }
}
