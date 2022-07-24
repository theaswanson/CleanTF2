using CleanTF2.Core;
using Spectre.Console;

namespace CleanTF2.CLI
{
    public class FlattenService : IFlattenService
    {
        private readonly IFlattenTexturesService _flattenTexturesService;
        private readonly Defaults _defaults;

        public FlattenService(
            IFlattenTexturesService flattenTexturesService,
            Defaults defaults)
        {
            _flattenTexturesService = flattenTexturesService;
            _defaults = defaults;
        }

        public async Task Flatten(string tf2Directory, bool upscaleTextures, FlattenOutputType outputType)
        {
            var directories = _flattenTexturesService.GetWorkingDirectories();

            AnsiConsole.WriteLine("Flattening TF2 textures...");
            await AnsiConsole.Status().StartAsync("Starting", async context =>
            {
                await _flattenTexturesService.GenerateFlatTextures(_defaults.TF2TexturesPackage(tf2Directory), await _defaults.TF2MaterialList(), directories.TF2WorkingDirectory, upscaleTextures, (string status) => context.Status(status));
            });

            AnsiConsole.WriteLine("Flattening HL2 textures...");
            await AnsiConsole.Status().StartAsync("Starting", async context =>
            {
                await _flattenTexturesService.GenerateFlatTextures(_defaults.HL2TexturesPackage(tf2Directory), await _defaults.HL2MaterialList(), directories.HL2WorkingDirectory, upscaleTextures, (string status) => context.Status(status));
            });

            AnsiConsole.WriteLine("Consolidating textures...");
            var flattenedTexturesDirectory = _flattenTexturesService.ConsolidateTextures(directories);

            AnsiConsole.WriteLine("Creating final output...");
            var generatedFiles = await _flattenTexturesService.CreateOutput(tf2Directory, flattenedTexturesDirectory, outputType);

            if (!generatedFiles.Any())
            {
                AnsiConsole.WriteLine("No output.");
            }
            else
            {
                AnsiConsole.WriteLine("Output:");
                foreach (var file in generatedFiles)
                {
                    AnsiConsole.WriteLine(file);
                }
            }
        }
    }
}
