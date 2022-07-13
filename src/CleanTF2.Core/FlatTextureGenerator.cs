using System.Diagnostics;
using CleanTF2.Core.Utilities;
using CleanTF2.Core.Valve;

namespace CleanTF2.Core
{
    public class FlatTextureGenerator : IFlatTextureGenerator
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
            _directory.CreateDirectory(extractedDirectory);

            Console.WriteLine("Extracting textures...");
            var timer = Stopwatch.StartNew();
            foreach (var material in materialsToFlatten)
            {
                await ExtractTexture(vpk, material, extractedDirectory);
            }
            timer.Stop();
            Console.WriteLine($"Extracting textures took {timer.Elapsed.TotalSeconds} seconds.");

            await FlattenTextures(extractedDirectory, extractedDirectory, resize);
        }

        private async Task ExtractTexture(string vpk, string materialToExtract, string extractedDirectory)
        {
            // material should be extracted to the parent directory of the material path
            var extractTo = Path.Combine(extractedDirectory, GetMaterialDirectory(materialToExtract));
            await _hlExtract.Run(vpk, extractTo, new List<string> { materialToExtract }, useFileMapping: true, allowVolatileAccess: true, useSilentMode: true);
            //await _hlExtract.Run(vpk, extractedDirectory, materialsToFlatten, useFileMapping: true, allowVolatileAccess: true, useSilentMode: true);
        }

        private async Task ExtractTextures(string vpk, IEnumerable<string> materialsToFlatten, string extractedDirectory)
        {
            await _hlExtract.Run(vpk, extractedDirectory, materialsToFlatten, useFileMapping: true, allowVolatileAccess: true, useSilentMode: true);
        }

        private async Task FlattenTextures(string directory, string saveTo, bool resize)
        {
            Console.WriteLine("Converting .vtf to .tga...");
            var timer = Stopwatch.StartNew();
            var tgaFiles = await ConvertFilesFromVTFtoTGA(directory);
            timer.Stop();
            Console.WriteLine($".vtf to .tga took {timer.Elapsed.TotalSeconds} seconds.");

            Console.WriteLine("Flattening .tga files...");
            timer.Restart();
            foreach (var tga in tgaFiles)
            {
                await CreateFlatTGA(tga, resize);
            }
            timer.Stop();
            Console.WriteLine($"Flattening .tga files took {timer.Elapsed.TotalSeconds} seconds.");

            Console.WriteLine("Converting .tga to .vtf...");
            timer.Restart();
            await ConvertFilesFromTGAtoVTF(directory);
            timer.Stop();
            Console.WriteLine($".tga to .vtf took {timer.Elapsed.TotalSeconds} seconds.");

            Console.WriteLine("Deleting .tga files...");
            timer.Restart();
            foreach (var tga in tgaFiles)
            {
                _file.Delete(tga);
            }
            timer.Stop();
            Console.WriteLine($"Deleting .tga files took {timer.Elapsed.TotalSeconds} seconds.");
        }

        private async Task<IEnumerable<string>> ConvertFilesFromTGAtoVTF(string directory)
        {
            await _vtfCmd.Run(folder: Path.Combine(directory, "*.tga"), format: "BGR888", alphaFormat: "ABGR8888", flag: "MINMIP", version: "7.4");
            return _directory.GetFiles(directory, "*.vtf", SearchOption.AllDirectories);
        }

        private async Task CreateFlatTGA(string tga, bool resize)
        {
            _imageManipulator
                .WithImage(tga)
                .Resize(1, 1);
                //.Resize(1024, 1024);
                //.Composite(tga);

            // TODO: determine how composite should work

            if (!resize)
            {
                _imageManipulator.Resize(1024, 1024);
            }

            // Overwrite original tga
            await _imageManipulator.Save(tga);
        }

        private async Task<IEnumerable<string>> ConvertFilesFromVTFtoTGA(string directory)
        {
            var pattern = Path.Combine(directory, "*.vtf");
            await _vtfCmd.Run(folder: pattern, exportFormat: "tga");
            return _directory.GetFiles(directory, "*.tga", SearchOption.AllDirectories);
        }

        /// <summary>
        /// Gets the directory to hold extracted textures.
        /// </summary>
        /// <param name="workingDirectory"></param>
        /// <returns></returns>
        private string GetExtractedDirectory(string workingDirectory)
        {
            return Path.Combine(workingDirectory, "extracted", "materials");
        }

        /// <summary>
        /// Gets the directory to hold converted textures.
        /// </summary>
        /// <param name="workingDirectory"></param>
        /// <returns></returns>
        private string GetConvertedDirectory(string workingDirectory)
        {
            return Path.Combine(workingDirectory, "converted", "materials");
        }

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
        {
            return material.EndsWith(".vtf")
                ? Path.GetDirectoryName(material)!
                : material;
        }
    }
}
