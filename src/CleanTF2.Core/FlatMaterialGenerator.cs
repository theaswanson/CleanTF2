using CleanTF2.Core.Utilities;
using CleanTF2.Core.Valve;

namespace CleanTF2.Core
{
    public class FlatMaterialGenerator : IFlatMaterialGenerator
    {
        private readonly IFile _file;
        private readonly IDirectory _directory;
        private readonly IHLExtract _hlExtract;
        private readonly IVTFCmd _vtfCmd;
        private readonly IImageManipulator _imageManipulator;

        public FlatMaterialGenerator(
            IFile file,
            IDirectory directory,
            IHLExtract hlExtract,
            IVTFCmd vtfCmd,
            IImageManipulator imageManipulator)
        {
            _file = file;
            _directory = directory;
            _hlExtract = hlExtract;
            _vtfCmd = vtfCmd;
            _imageManipulator = imageManipulator;
        }

        /// <inheritdoc/>
        public async Task Generate(string package, IEnumerable<string> materials, string outputDirectory, bool resize, Action<string> setCurrentStatus)
        {
            if (!_file.Exists(package))
            {
                throw new Exception($"Could not find the package at {package}. Check the path and try agian.");
            }

            _directory.CreateDirectory(outputDirectory);

            await ExtractAllMaterials(package, materials, outputDirectory, setCurrentStatus);
            await FlattenMaterials(outputDirectory, resize, setCurrentStatus);
        }

        /// <summary>
        /// Extracts the given materials from the given package to the given output directory.
        /// </summary>
        /// <param name="package">Path to .vpk package containing the given materials (e.g. tf2_textures_dir.vpk).</param>
        /// <param name="materials">List of paths to material files or directories to flatten.</param>
        /// <param name="outputDirectory">Directory to save the materials to.</param>
        /// <returns></returns>
        private async Task ExtractAllMaterials(string package, IEnumerable<string> materials, string outputDirectory, Action<string> setCurrentStatus)
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
            setCurrentStatus($"Extracting materials from {directoriesOfMaterials.Count()} directories");
            await ExtractMaterials(package, directoriesOfMaterials, outputDirectory);
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
