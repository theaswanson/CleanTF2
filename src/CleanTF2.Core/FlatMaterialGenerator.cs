using CleanTF2.Core.Utilities;
using CleanTF2.Core.Valve;

namespace CleanTF2.Core
{
    public class FlatMaterialGenerator : IFlatMaterialGenerator
    {
        private readonly IFile _file;
        private readonly IDirectory _directory;
        private readonly IVTFCmd _vtfCmd;
        private readonly IImageManipulator _imageManipulator;
        private readonly IMaterialExtractor _materialExtractor;

        public FlatMaterialGenerator(
            IFile file,
            IDirectory directory,
            IVTFCmd vtfCmd,
            IImageManipulator imageManipulator,
            IMaterialExtractor materialExtractor)
        {
            _file = file;
            _directory = directory;
            _vtfCmd = vtfCmd;
            _imageManipulator = imageManipulator;
            _materialExtractor = materialExtractor;
        }

        /// <inheritdoc/>
        public async Task Generate(string package, IEnumerable<string> materials, string outputDirectory, bool resize, Action<string> setCurrentStatus)
        {
            if (!_file.Exists(package))
            {
                throw new Exception($"Could not find the package at {package}. Check the path and try again.");
            }

            await _materialExtractor.ExtractAllMaterials(package, materials, outputDirectory, setCurrentStatus);
            await FlattenMaterials(outputDirectory, resize, setCurrentStatus);
        }

        /// <summary>
        /// Converts all .vtf files in the given directory to flattened materials.
        /// </summary>
        /// <param name="directory">Directory containing materials to be converted.</param>
        /// <param name="resize">If true, resizes the flattened material to 1024x1024.</param>
        /// <returns></returns>
        private async Task FlattenMaterials(string directory, bool resize, Action<string> setCurrentStatus)
        {
            setCurrentStatus($"Converting materials to .tga files");
            var tgaFiles = await ConvertFilesFromVTFtoTGA(directory);

            var materialNumber = 0;
            foreach (var tga in tgaFiles)
            {
                materialNumber++;
                setCurrentStatus($"Flattening .tga file {materialNumber}/{tgaFiles.Count()}");
                await CreateFlatTGA(tga, resize);
            }

            setCurrentStatus($"Converting .tga files to materials");
            await ConvertFilesFromTGAtoVTF(directory);

            setCurrentStatus($"Deleting .tga files");
            foreach (var tga in tgaFiles)
            {
                _file.Delete(tga);
            }
        }

        /// <summary>
        /// Flattens the given .tga file.
        /// </summary>
        /// <param name="pathToTGA">Path to a .tga file.</param>
        /// <param name="resize">If true, resizes the flattened material to 1024x1024.</param>
        /// <returns></returns>
        private async Task CreateFlatTGA(string pathToTGA, bool resize)
        {
            // TODO: determine how composite should work
            _imageManipulator
                .WithImage(pathToTGA)
                .Resize(1, 1);

            if (!resize)
            {
                _imageManipulator.Resize(1024, 1024);
            }

            // Overwrite original tga
            await _imageManipulator.Save(pathToTGA);
        }

        /// <summary>
        /// Creates .tga files from .vtf files in the given directory and subdirectories.
        /// </summary>
        /// <param name="directory">Directory containing .vtf files.</param>
        /// <returns>Paths to the .tga files.</returns>
        private async Task<IEnumerable<string>> ConvertFilesFromVTFtoTGA(string directory)
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
        private async Task<IEnumerable<string>> ConvertFilesFromTGAtoVTF(string directory)
        {
            await _vtfCmd.Run(folder: Path.Combine(directory, "*.tga"), format: "BGR888", alphaFormat: "ABGR8888", flag: "MINMIP", version: "7.4");
            return _directory.GetFiles(directory, "*.vtf", SearchOption.AllDirectories);
        }
    }
}
