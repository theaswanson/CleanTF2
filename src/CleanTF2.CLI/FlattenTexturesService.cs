using CleanTF2.Core;
using CleanTF2.Core.Materials;
using CleanTF2.Core.Utilities;
using Spectre.Console;

namespace CleanTF2.CLI
{
    public class FlattenTexturesService : IFlattenTexturesService
    {
        private readonly IFlatMaterialGenerator _flatTextureGenerator;
        private readonly IFile _file;
        private readonly IDirectory _directory;
        private readonly IVPKGenerator _vpkGenerator;

        public FlattenTexturesService(
            IFlatMaterialGenerator flatTextureGenerator,
            IFile file,
            IDirectory directory,
            IVPKGenerator vpkGenerator)
        {
            _flatTextureGenerator = flatTextureGenerator;
            _file = file;
            _directory = directory;
            _vpkGenerator = vpkGenerator;
        }

        public async Task Flatten(string tf2Directory, bool upscaleTextures, OutputType outputType)
        {
            // Determine directory paths
            var tempSaveDirectory = Path.Combine(_directory.GetCurrentDirectory(), "flattened");
            var tf2MaterialsSaveDirectory = Path.Combine(tempSaveDirectory, "tf2");
            var hl2MaterialsSaveDirectory = Path.Combine(tempSaveDirectory, "hl2");

            // Flatten textures
            await GenerateFlatTF2Textures(tf2Directory, tf2MaterialsSaveDirectory, upscaleTextures);
            await GenerateFlatHL2Textures(tf2Directory, hl2MaterialsSaveDirectory, upscaleTextures);

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

            switch (outputType)
            {
                case OutputType.SingleVPK:
                    await OutputVPK(tf2Directory, saveMaterialsTo, multiChunk: false);
                    _directory.Delete(saveMaterialsTo, recursive: true);
                    break;
                case OutputType.MultiChunkVPK:
                    await OutputVPK(tf2Directory, saveMaterialsTo, multiChunk: false);
                    _directory.Delete(saveMaterialsTo, recursive: true);
                    break;
                case OutputType.TextureFiles:
                    var materialsDirectory = Path.Combine(saveMaterialsTo, "materials");
                    AnsiConsole.WriteLine("Finished!");
                    AnsiConsole.WriteLine(materialsDirectory);
                    break;
                default:
                    break;
            }
        }

        private async Task OutputVPK(string tf2Directory, string directoryToPack, bool multiChunk)
        {
            AnsiConsole.WriteLine("Generating .vpk files...");
            var vpks = await _vpkGenerator.Generate(tf2Directory, directoryToPack, multiChunk);

            AnsiConsole.WriteLine("Finished!");
            foreach (var vpk in vpks)
            {
                AnsiConsole.WriteLine(vpk);
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
    }
}
