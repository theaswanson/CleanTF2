using CleanTF2.Core.Utilities;

namespace CleanTF2.Core.Materials
{
    public class FlatMaterialGenerator : IFlatMaterialGenerator
    {
        private readonly IFile _file;
        private readonly IMaterialExtractor _materialExtractor;
        private readonly IMaterialConverter _materialConverter;
        private readonly IMaterialFlattener _materialFlattener;

        public FlatMaterialGenerator(
            IFile file,
            IMaterialExtractor materialExtractor,
            IMaterialConverter materialConverter,
            IMaterialFlattener materialFlattener)
        {
            _file = file;
            _materialExtractor = materialExtractor;
            _materialConverter = materialConverter;
            _materialFlattener = materialFlattener;
        }

        /// <inheritdoc/>
        public async Task Generate(string package, IEnumerable<string> materials, string outputDirectory, bool upscale, Action<string> setCurrentStatus)
        {
            if (!_file.Exists(package))
            {
                throw new Exception($"Could not find the package at {package}. Check the path and try again.");
            }

            if (!materials.Any())
            {
                return;
            }

            await _materialExtractor.ExtractAllMaterials(package, materials, outputDirectory, setCurrentStatus);
            await FlattenMaterials(outputDirectory, upscale, setCurrentStatus);
        }

        /// <summary>
        /// Converts all .vtf files in the given directory to flattened materials.
        /// </summary>
        /// <param name="directory">Directory containing materials to be converted.</param>
        /// <param name="upscale">If true, resizes the flattened material to 1024x1024.</param>
        /// <returns></returns>
        private async Task FlattenMaterials(string directory, bool upscale, Action<string> setCurrentStatus)
        {
            setCurrentStatus($"Converting materials to .tga files");
            var tgaFiles = await _materialConverter.ConvertFromVTFtoTGA(directory);

            await FlattenImages(upscale, setCurrentStatus, tgaFiles);

            setCurrentStatus($"Converting .tga files to materials");
            await _materialConverter.ConvertFromTGAtoVTF(directory);

            setCurrentStatus($"Deleting .tga files");
            foreach (var tga in tgaFiles)
            {
                _file.Delete(tga);
            }
        }

        private async Task FlattenImages(bool resize, Action<string> setCurrentStatus, IEnumerable<string> tgaFiles)
        {
            var materialNumber = 0;
            foreach (var tga in tgaFiles)
            {
                materialNumber++;
                setCurrentStatus($"Flattening .tga file {materialNumber}/{tgaFiles.Count()}");
                await _materialFlattener.FlattenMaterial(tga, resize);
            }
        }
    }
}
