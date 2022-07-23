using CleanTF2.Core.Utilities;
using CleanTF2.Core.Libraries;

namespace CleanTF2.Core
{
    public class VPKGenerator : IVPKGenerator
    {
        private readonly IFile _file;
        private readonly IDirectory _directory;
        private readonly IVPK _vpk;

        public VPKGenerator(IFile file, IDirectory directory, IVPK vpk)
        {
            _file = file;
            _directory = directory;
            _vpk = vpk;
        }

        public async Task<IEnumerable<string>> Generate(string tf2Directory, string directoryToPack, bool multiChunk)
        {
            await CreateREADME(directoryToPack);

            // Create VPKs (generated next to the given directory)
            await _vpk.Run(tf2Directory, directoryToPack, produceMultiChunkVPK: multiChunk);

            var vpkFileName = Path.GetFileName(directoryToPack);
            var pattern = multiChunk
                ? $"{vpkFileName}_*.vpk"
                : $"{vpkFileName}.vpk";

            return _directory.GetFiles(Path.GetDirectoryName(directoryToPack), pattern, SearchOption.TopDirectoryOnly);
        }

        private async Task CreateREADME(string directoryToPack)
        {
            var pathToREADME = Path.Combine(directoryToPack, "README.txt");
            await _file.WriteAllLinesAsync(pathToREADME, new List<string>
            {
                "Created by CleanTF2",
                "https://github.com/theaswanson/CleanTF2"
            });
        }
    }
}
