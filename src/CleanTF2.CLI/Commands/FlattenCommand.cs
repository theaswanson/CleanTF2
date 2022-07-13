using CleanTF2.Core;
using CleanTF2.Core.Utilities;
using Spectre.Console.Cli;

namespace CleanTF2.CLI.Commands
{
    internal sealed class FlattenCommand : AsyncCommand<FlattenCommand.Settings>
    {
        private readonly IFlatTextureGenerator _flatTextureGenerator;
        private readonly IDirectory _directory;

        public FlattenCommand(IFlatTextureGenerator flatTextureGenerator, IDirectory directory)
        {
            _flatTextureGenerator = flatTextureGenerator;
            _directory = directory;
        }
        public sealed class Settings : CommandSettings
        {
        }

        public override async Task<int> ExecuteAsync(CommandContext context, Settings settings)
        {
            var tf2TextureFile = @"C:\Program Files (x86)\Steam\steamapps\common\Team Fortress 2\tf\tf2_textures_dir.vpk";
            //var materials = new List<string> { "materials\\brick", "materials\\building_template", "materials\\concrete" };
            var materials = new List<string> { "materials\\harvest\\harvest_grass.vtf" };
            //var materials = await File.ReadAllLinesAsync("flat.txt");
            var saveTo = _directory.GetCurrentDirectory();

            await _flatTextureGenerator.Generate(tf2TextureFile, materials, saveTo, resize: true);

            return 0;
        }
    }
}
