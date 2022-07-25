using CleanTF2.Core;
using CleanTF2.Core.Utilities;
using Spectre.Console;

namespace CleanTF2.CLI
{
    /// <inheritdoc/>
    public class FlattenService : IFlattenService
    {
        private readonly IFlattenTexturesService _flattenTexturesService;
        private readonly IDirectory _directory;
        private readonly Defaults _defaults;

        public FlattenService(
            IFlattenTexturesService flattenTexturesService,
            IDirectory directory,
            Defaults defaults)
        {
            _flattenTexturesService = flattenTexturesService;
            _directory = directory;
            _defaults = defaults;
        }

        public async Task Flatten(string tf2Directory, bool upscaleTextures, FlattenOutputType outputType, FlattenMode mode)
        {
            // Determine working directories
            var currentDirectory = _directory.GetCurrentDirectory();
            var workingDirectory = Path.Combine(currentDirectory, "working");
            var tf2WorkingDirectory = Path.Combine(workingDirectory, "tf2");
            var hl2WorkingDirectory = Path.Combine(workingDirectory, "hl2");


            // Flatten textures
            IEnumerable<string> flattenedTextureDirectories;
            switch (mode)
            {
                case FlattenMode.All:
                    await FlattenTF2Textures(tf2Directory, upscaleTextures, tf2WorkingDirectory);
                    await FlattenHL2Textures(tf2Directory, upscaleTextures, hl2WorkingDirectory);
                    flattenedTextureDirectories = new List<string> { tf2WorkingDirectory, hl2WorkingDirectory };
                    break;
                case FlattenMode.TF2:
                    await FlattenTF2Textures(tf2Directory, upscaleTextures, tf2WorkingDirectory);
                    flattenedTextureDirectories = new List<string> { tf2WorkingDirectory };
                    break;
                case FlattenMode.HL2:
                    await FlattenHL2Textures(tf2Directory, upscaleTextures, hl2WorkingDirectory);
                    flattenedTextureDirectories = new List<string> { hl2WorkingDirectory };
                    break;
                default:
                    throw new Exception("Unknown mode.");
            }

            // Move textures together to prepare for output
            AnsiConsole.WriteLine("Consolidating textures...");
            var consolidateTo = Path.Combine(currentDirectory, "flat-textures");
            _flattenTexturesService.ConsolidateTextures(consolidateTo, flattenedTextureDirectories);
            _directory.Delete(workingDirectory, recursive: true);

            // Create output
            AnsiConsole.WriteLine("Creating final output...");
            var generatedFiles = await _flattenTexturesService.CreateOutput(tf2Directory, consolidateTo, outputType);

            _flattenTexturesService.CleanUp(consolidateTo, outputType);

            // Print file paths
            if (!generatedFiles.Any())
            {
                AnsiConsole.WriteLine("No output.");
            }
            else
            {
                AnsiConsole.WriteLine("Done! Created the following:");
                foreach (var file in generatedFiles)
                {
                    AnsiConsole.WriteLine(file);
                }
            }
        }

        private async Task FlattenHL2Textures(string tf2Directory, bool upscaleTextures, string hl2WorkingDirectory)
        {
            AnsiConsole.WriteLine("Flattening HL2 textures...");
            await AnsiConsole.Status().StartAsync("Starting", async context =>
            {
                await _flattenTexturesService.GenerateFlatTextures(_defaults.HL2TexturesPackage(tf2Directory), await _defaults.HL2MaterialList(), hl2WorkingDirectory, upscaleTextures, (string status) => context.Status(status));
            });
        }

        private async Task FlattenTF2Textures(string tf2Directory, bool upscaleTextures, string tf2WorkingDirectory)
        {
            AnsiConsole.WriteLine("Flattening TF2 textures...");
            await AnsiConsole.Status().StartAsync("Starting", async context =>
            {
                await _flattenTexturesService.GenerateFlatTextures(_defaults.TF2TexturesPackage(tf2Directory), await _defaults.TF2MaterialList(), tf2WorkingDirectory, upscaleTextures, (string status) => context.Status(status));
            });
        }
    }
}
