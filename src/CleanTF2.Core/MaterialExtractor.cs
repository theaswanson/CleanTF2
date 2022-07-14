using CleanTF2.Core.Utilities;
using CleanTF2.Core.Valve;

namespace CleanTF2.Core
{
    public class MaterialExtractor : IMaterialExtractor
    {
        private readonly IDirectory _directory;
        private readonly IHLExtract _hlExtract;

        public MaterialExtractor(IDirectory directory, IHLExtract hlExtract)
        {
            _directory = directory;
            _hlExtract = hlExtract;
        }

        /// <inheritdoc/>
        public async Task ExtractAllMaterials(string package, IEnumerable<string> materials, string outputDirectory, Action<string> setCurrentStatus)
        {
            var singleMaterials = materials.Where(m => m.EndsWith(".vtf"));
            var directoriesOfMaterials = materials.Except(singleMaterials);

            var materialNumber = 0;
            foreach (var material in singleMaterials)
            {
                materialNumber++;
                setCurrentStatus($"Extracting material {materialNumber}/{singleMaterials.Count()}");
                await ExtractMaterial(package, material, outputDirectory);
            }

            if (directoriesOfMaterials.Any())
            {
                setCurrentStatus($"Extracting materials from {directoriesOfMaterials.Count()} directories");
                await ExtractMaterials(package, directoriesOfMaterials, outputDirectory);
            }
        }

        /// <summary>
        /// Extracts the given material from the given package to the given output directory.
        /// </summary>
        /// <param name="package">Path to .vpk package containing the given materials (e.g. tf2_textures_dir.vpk).</param>
        /// <param name="material">Material to flatten (e.g. materials/concrete/ceiling_01.vtf).</param>
        /// <param name="outputDirectory">Directory to save the material to.</param>
        /// <returns></returns>
        private async Task ExtractMaterial(string package, string material, string outputDirectory)
        {
            // When extracting an individual material, HLExtract will extract the material to the exact directory it's given.
            // Thus, we have to append the material directory to the output directory to retain the correct directory structure.
            var extractTo = Path.Combine(outputDirectory, Path.GetDirectoryName(material)!);
            _directory.CreateDirectory(extractTo);
            await _hlExtract.Run(package, extractTo, new List<string> { material }, useFileMapping: true, allowVolatileAccess: true, useSilentMode: true);
        }

        /// <summary>
        /// Extracts the given directories of materials from the given package to the given output directory.
        /// </summary>
        /// <param name="package">Path to .vpk package containing the given materials (e.g. tf2_textures_dir.vpk).</param>
        /// <param name="directoriesOfMaterials">Directories of materials to flatten (e.g. materials/concrete).</param>
        /// <param name="outputDirectory">Directory to save the materials to.</param>
        /// <returns></returns>
        private async Task ExtractMaterials(string package, IEnumerable<string> directoriesOfMaterials, string outputDirectory)
        {
            // When extracting a directory of materials, HLExtract ignores the "materials" directory when saving.
            // Thus, we have to tell it to extract to some/dir/materials in order to retain the correct directory structure
            var extractTo = Path.Combine(outputDirectory, "materials");
            _directory.CreateDirectory(extractTo);
            await _hlExtract.Run(package, extractTo, directoriesOfMaterials, useFileMapping: true, allowVolatileAccess: true, useSilentMode: true);
        }
    }
}
