using CleanTF2.Core.Utilities;
using CleanTF2.Core.Valve;

namespace CleanTF2.Core
{
    public class FlatTextureGenerator
    {
        private readonly IFile _file;
        private readonly IDirectory _directory;
        private readonly IHLExtract _hlExtract;
        private readonly IVTFCmd _vtfCmd;
        private readonly IImageManipulator _imageManipulator;

        public FlatTextureGenerator(IFile file, IDirectory directory, IHLExtract hlExtract, IVTFCmd vtfCmd, IImageManipulator imageManipulator)
        {
            _file = file;
            _directory = directory;
            _hlExtract = hlExtract;
            _vtfCmd = vtfCmd;
            _imageManipulator = imageManipulator;
        }

        /// <summary>
        /// Extracts the specified .vtf files from the given .vpk and flattens them to new .vtf files.
        /// </summary>
        /// <param name="vpk">Path to .vpk package containing the given materials.</param>
        /// <param name="materialsToFlatten">List of paths to material files or directories to flatten.</param>
        /// <param name="resize">If true, resizes textures to 1x1. If false, leaves textures in the default size (1024x1024).</param>
        public async Task Generate(string vpk, IEnumerable<string> materialsToFlatten, string outputDirectory, bool resize)
        {
            var extractedDirectory = GetExtractedDirectory(outputDirectory);
            var convertedDirectory = GetConvertedDirectory(outputDirectory);
            
            foreach (var material in materialsToFlatten)
            {
                await FlattenMaterials(vpk, resize, extractedDirectory, convertedDirectory, material);
            }
        }

        private async Task FlattenMaterials(string vpk, bool resize, string extractedDirectory, string convertedDirectory, string material)
        {
            // Create directories to hold textures
            var materialDirectory = GetMaterialDirectory(material);
            var extractTo = CreateMaterialSaveDirectory(extractedDirectory, materialDirectory);
            var saveTo = CreateMaterialSaveDirectory(convertedDirectory, materialDirectory);

            // Extract .vtf files
            await _hlExtract.Run(vpk, extractTo, material);

            // Flatten .vtf files
            var files = _directory.GetFiles(extractTo);
            foreach (var vtf in files)
            {
                await FlattenVTF(vtf, saveTo, resize);
            }
        }

        private async Task FlattenVTF(string vtf, string saveTo, bool resize)
        {
            // Convert to .tga
            await _vtfCmd.Run(vtf, exportFormat: "tga");
            var tga = Path.ChangeExtension(vtf, ".tga");

            // Average the color
            _imageManipulator
                .WithImage(tga)
                .Resize(1, 1)
                .Resize(1024, 1024)
                .Composite(tga);

            // (Optional) Resize to 1x1
            if (resize)
            {
                _imageManipulator.Resize(1, 1);
            }

            await _imageManipulator.Finish(Path.Combine(saveTo, tga));

            // Convert to .vtf
            await _vtfCmd.Run(tga, output: saveTo, format: "BGR888", alphaFormat: "ABGR8888", flag: "MINMIP", version: "7.4");

            // Delete .tga
            _file.Delete(tga);
        }

        /// <summary>
        /// Gets the directory to hold extracted textures.
        /// </summary>
        /// <param name="workingDirectory"></param>
        /// <returns></returns>
        private string GetExtractedDirectory(string workingDirectory) => Path.Combine(workingDirectory, "extracted");

        /// <summary>
        /// Gets the directory to hold converted textures.
        /// </summary>
        /// <param name="workingDirectory"></param>
        /// <returns></returns>
        private string GetConvertedDirectory(string workingDirectory) => Path.Combine(workingDirectory, "converted");

        /// <summary>
        /// Creates directory to save the given material(s) to.
        /// </summary>
        /// <param name="directory">Directory to extract files to.</param>
        /// <param name="materialDirectory">Directory of the material in the .vpk.</param>
        /// <returns></returns>
        private string CreateMaterialSaveDirectory(string directory, string materialDirectory)
        {
            var saveDirectory = Path.Combine(directory, materialDirectory);
            _directory.CreateDirectory(saveDirectory);
            return saveDirectory;
        }

        private string GetMaterialDirectory(string material)
            => _file.IsDirectory(material)
                ? material
                : Path.GetDirectoryName(material)!;
    }
}
