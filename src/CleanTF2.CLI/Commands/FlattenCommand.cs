using System.ComponentModel;
using CleanTF2.Core;
using CleanTF2.Core.Materials;
using CleanTF2.Core.Utilities;
using Spectre.Console;
using Spectre.Console.Cli;

namespace CleanTF2.CLI.Commands
{
    internal sealed class FlattenCommand : AsyncCommand<FlattenCommand.Settings>
    {
        private readonly IFlatMaterialGenerator _flatTextureGenerator;
        private readonly IFile _file;
        private readonly IDirectory _directory;
        private readonly IInterop _interop;
        private readonly IVPKGenerator _vpkGenerator;

        public FlattenCommand(
            IFlatMaterialGenerator flatTextureGenerator,
            IFile file,
            IDirectory directory,
            IInterop interop,
            IVPKGenerator vpkGenerator)
        {
            _flatTextureGenerator = flatTextureGenerator;
            _file = file;
            _directory = directory;
            _interop = interop;
            _vpkGenerator = vpkGenerator;
        }

        public sealed class Settings : CommandSettings
        {
            [Description("Full path to the Team Fortress 2 directory (the directory that contains the hl2 executable).")]
            [CommandOption("-d")]
            public string TF2Directory { get; set; }

            [Description("1: .vpk files (default), 2: material files")]
            [CommandOption("--output-type")]
            [DefaultValue(1)]
            public int OutputType { get; set; }

            [Description("Upscales flattened textures to 1024x1024. Helps address compatibility problems with sv_pure. (warning: takes more file size and processing time)")]
            [CommandOption("--upscale")]
            public bool Upscale { get; set; }
        }

        public override async Task<int> ExecuteAsync(CommandContext context, Settings settings)
        {
            ValidateOS();
            ValidateTF2Directory(settings);

            // Determine directory paths
            var tempSaveDirectory = Path.Combine(_directory.GetCurrentDirectory(), "flattened");
            var tf2MaterialsSaveDirectory = Path.Combine(tempSaveDirectory, "tf2");
            var hl2MaterialsSaveDirectory = Path.Combine(tempSaveDirectory, "hl2");

            // Flatten textures
            await GenerateFlatTF2Textures(settings.TF2Directory, tf2MaterialsSaveDirectory, settings.Upscale);
            await GenerateFlatHL2Textures(settings.TF2Directory, hl2MaterialsSaveDirectory, settings.Upscale);

            // Consolidate all textures
            AnsiConsole.WriteLine("Preparing textures...");
            var saveMaterialsTo = Path.Combine(_directory.GetCurrentDirectory(), "flat-textures");
            if (_directory.Exists(tf2MaterialsSaveDirectory))
            {
                _directory.CopyDirectory(tf2MaterialsSaveDirectory, saveMaterialsTo, recursive: true, overwrite: true);
            }
            if (_directory.Exists(hl2MaterialsSaveDirectory))
            {
                _directory.CopyDirectory(hl2MaterialsSaveDirectory, saveMaterialsTo, recursive: true, overwrite: true);
            }

            // Delete temp directory of textures
            AnsiConsole.WriteLine("Removing temp textures...");
            _directory.Delete(tempSaveDirectory, recursive: true);

            if (settings.OutputType == 1)
            {
                // Package textures into .vpk
                AnsiConsole.WriteLine("Generating .vpk files...");
                var vpks = await _vpkGenerator.Generate(settings.TF2Directory, saveMaterialsTo, multiChunk: true);

                AnsiConsole.WriteLine("Finished!");
                foreach (var vpk in vpks)
                {
                    AnsiConsole.WriteLine(vpk);
                }

                _directory.Delete(saveMaterialsTo, recursive: true);
            }
            else
            {
                var materialsDirectory = Path.Combine(saveMaterialsTo, "materials");
                AnsiConsole.WriteLine("Finished!");
                AnsiConsole.WriteLine(materialsDirectory);
            }

            return 0;
        }

        private void ValidateOS()
        {
            if (!_interop.IsOSPlatform(System.Runtime.InteropServices.OSPlatform.Windows))
            {
                throw new Exception("Windows is the only supported OS at this time.");
            }
        }

        private void ValidateTF2Directory(Settings settings)
        {
            if (string.IsNullOrWhiteSpace(settings.TF2Directory))
            {
                settings.TF2Directory = DefaultTF2Directory();
            }

            if (!_directory.Exists(settings.TF2Directory))
            {
                throw new DirectoryNotFoundException("The given TF2 directory could not be found. Check the path and try again.");
            }
        }

        private async Task GenerateFlatTextures(string package, IEnumerable<string> materials, string saveTo, bool upscale)
        {
            await AnsiConsole.Status().StartAsync("Starting", async context =>
            {
                await _flatTextureGenerator.Generate(package, materials, saveTo, upscale, (string status) => context.Status(status));
            });
        }

        private async Task GenerateFlatTF2Textures(string tf2Directory, string saveTo, bool upscale)
        {
            var tf2TextureFile = Path.Combine(tf2Directory, "tf", "tf2_textures_dir.vpk");
            var tf2Materials = await GetTF2MaterialList();

            AnsiConsole.WriteLine("Generating flat TF2 textures...");
            await GenerateFlatTextures(tf2TextureFile, tf2Materials, saveTo, upscale);
        }

        private async Task<IEnumerable<string>> GetTF2MaterialList()
        {
            return await _file.ReadAllLinesAsync("flat.txt");
        }

        private async Task GenerateFlatHL2Textures(string tf2Directory, string saveTo, bool upscale)
        {
            var hl2TextureFile = Path.Combine(tf2Directory, "hl2", "hl2_textures_dir.vpk");
            var hl2Materials = await GetHL2MaterialList();

            AnsiConsole.WriteLine("Generating flat HL2 textures...");
            await GenerateFlatTextures(hl2TextureFile, hl2Materials, saveTo, upscale);
        }

        private async Task<IEnumerable<string>> GetHL2MaterialList()
        {
            return await _file.ReadAllLinesAsync("flat_hl2.txt");
        }

        private string DefaultTF2Directory() => @"C:\Program Files (x86)\Steam\steamapps\common\Team Fortress 2";
    }
}
