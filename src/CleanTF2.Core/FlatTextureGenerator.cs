using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleanTF2.Core
{
    public class FlatTextureGenerator
    {
        /// <summary>
        /// Extracts the specified .vtf files from the given .vpk and flattens them to new .vtf files.
        /// </summary>
        /// <param name="materialsToFlatten">List of paths to material files or directories to flatten.</param>
        /// <param name="vpk">Path to .vpk package containing the given materials.</param>
        public void Generate(IEnumerable<string> materialsToFlatten, string vpk)
        {
            foreach (var materialPath in materialsToFlatten)
            {
                // Create directory to hold generated textures
                // Call HLExtract to extract .vtf files
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
