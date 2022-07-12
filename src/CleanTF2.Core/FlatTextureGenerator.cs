using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleanTF2.Core
{
    public class FlatTextureGenerator
    {
        private readonly IFile _file;
        private readonly IDirectory _directory;
        private readonly IHLExtract _hLExtract;

        public FlatTextureGenerator(IFile file, IDirectory directory, IHLExtract hLExtract)
        {
            _file = file;
            _directory = directory;
            _hLExtract = hLExtract;
        }

        /// <summary>
        /// Extracts the specified .vtf files from the given .vpk and flattens them to new .vtf files.
        /// </summary>
        /// <param name="vpk">Path to .vpk package containing the given materials.</param>
        /// <param name="materialsToFlatten">List of paths to material files or directories to flatten.</param>
        public async Task Generate(string vpk, IEnumerable<string> materialsToFlatten)
        {
            var workingDirectory = "working";
            foreach (var material in materialsToFlatten)
            {
                // Create directory to hold generated textures
                var materialDirectory = _file.IsDirectory(material)
                    ? material
                    : Path.GetDirectoryName(material)!;

                var extractDirectory = Path.Combine(workingDirectory, materialDirectory);
                _directory.CreateDirectory(extractDirectory);

                // Call HLExtract to extract .vtf files
                await _hLExtract.Run(vpk, extractDirectory, material);

                // Convert extracted .vtf files to .tga files
                // For each .tga file
                    // Average the color
                    // Resize image (based on user preference)
                    // Convert to .vtf
                    // Delete .tga
            }
        }
    }
}
