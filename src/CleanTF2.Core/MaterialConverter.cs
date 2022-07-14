using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CleanTF2.Core.Utilities;
using CleanTF2.Core.Valve;

namespace CleanTF2.Core
{
    public class MaterialConverter : IMaterialConverter
    {
        private readonly IVTFCmd _vtfCmd;
        private readonly IDirectory _directory;

        public MaterialConverter(IVTFCmd vtfCmd, IDirectory directory)
        {
            _vtfCmd = vtfCmd;
            _directory = directory;
        }

        /// <summary>
        /// Creates .tga files from .vtf files in the given directory and subdirectories.
        /// </summary>
        /// <param name="directory">Directory containing .vtf files.</param>
        /// <returns>Paths to the .tga files.</returns>
        public async Task<IEnumerable<string>> ConvertFromVTFtoTGA(string directory)
        {
            var pattern = Path.Combine(directory, "*.vtf");
            await _vtfCmd.Run(folder: pattern, exportFormat: "tga");
            return _directory.GetFiles(directory, "*.tga", SearchOption.AllDirectories);
        }

        /// <summary>
        /// Creates .vtf files from .tga files in the given directory and subdirectories.
        /// </summary>
        /// <param name="directory">Directory containing .tga files.</param>
        /// <returns>Paths to the .vtf files.</returns>
        public async Task<IEnumerable<string>> ConvertFromTGAtoVTF(string directory)
        {
            await _vtfCmd.Run(folder: Path.Combine(directory, "*.tga"), format: "BGR888", alphaFormat: "ABGR8888", flag: "MINMIP", version: "7.4");
            return _directory.GetFiles(directory, "*.vtf", SearchOption.AllDirectories);
        }
    }
}
