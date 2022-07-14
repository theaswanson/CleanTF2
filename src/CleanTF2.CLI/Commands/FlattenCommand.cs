using System.ComponentModel;
using CleanTF2.Core;
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

        public FlattenCommand(IFlatMaterialGenerator flatTextureGenerator, IFile file, IDirectory directory, IInterop interop)
        {
            _flatTextureGenerator = flatTextureGenerator;
            _file = file;
            _directory = directory;
            _interop = interop;
        }

        public sealed class Settings : CommandSettings
        {
            [Description("Full path to the Team Fortress 2 directory (the directory that contains the hl2 executable).")]
            [CommandOption("-d")]
            public string TF2Directory { get; set; }
        }

        public override async Task<int> ExecuteAsync(CommandContext context, Settings settings)
        {
            if (!_interop.IsOSPlatform(System.Runtime.InteropServices.OSPlatform.Windows))
            {
                throw new Exception("Windows is the only supported OS at this time.");
            }

            ValidateTF2Directory(settings);

            var saveTo = Path.Combine(_directory.GetCurrentDirectory(), "flattened");

            await GenerateFlatTF2Textures(settings.TF2Directory, Path.Combine(saveTo, "tf2"));
            await GenerateFlatHL2Textures(settings.TF2Directory, Path.Combine(saveTo, "hl2"));

            return 0;
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

        private async Task GenerateFlatTextures(string package, IEnumerable<string> materials, string saveTo)
        {
            await AnsiConsole.Status().StartAsync("Starting", async context =>
            {
                try
                {
                    await _flatTextureGenerator.Generate(package, materials, saveTo, resize: true, (string status) => context.Status(status));
                }
                catch (Exception ex)
                {
                    AnsiConsole.WriteLine("There was a problem generating flat textures. Here are the details:");
                    AnsiConsole.WriteException(ex);
                }
            });
        }

        private async Task GenerateFlatTF2Textures(string tf2Directory, string saveTo)
        {
            var tf2TextureFile = Path.Combine(tf2Directory, "tf", "tf2_textures_dir.vpk");
            var tf2Materials = await _file.ReadAllLinesAsync("flat.txt");

            AnsiConsole.WriteLine("Generating flat HL2 textures...");
            await GenerateFlatTextures(tf2TextureFile, tf2Materials, saveTo);
        }

        private async Task GenerateFlatHL2Textures(string tf2Directory, string saveTo)
        {
            var hl2TextureFile = Path.Combine(tf2Directory, "hl2", "hl2_textures_dir.vpk");
            var hl2Materials = await _file.ReadAllLinesAsync("flat_hl2.txt");

            AnsiConsole.WriteLine("Generating flat HL2 textures...");
            await GenerateFlatTextures(hl2TextureFile, hl2Materials, saveTo);
        }

        private string DefaultTF2Directory() => @"C:\Program Files (x86)\Steam\steamapps\common\Team Fortress 2";
    }
}
