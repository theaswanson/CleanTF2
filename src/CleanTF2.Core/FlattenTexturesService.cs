using CleanTF2.Core.Materials;
using CleanTF2.Core.Utilities;
using Microsoft.Extensions.Logging;

namespace CleanTF2.Core
{
    public class WorkingDirectories
    {
        public string WorkingDirectory { get; set; }
        public string TF2WorkingDirectory { get; set; }
        public string HL2WorkingDirectory { get; set; }
    }

    public class FlattenTexturesService : IFlattenTexturesService
    {
        private readonly IFlatMaterialGenerator _flatTextureGenerator;
        private readonly IFile _file;
        private readonly IDirectory _directory;
        private readonly IVPKGenerator _vpkGenerator;
        private readonly ILogger _logger;

        public FlattenTexturesService(
            IFlatMaterialGenerator flatTextureGenerator,
            IFile file,
            IDirectory directory,
            IVPKGenerator vpkGenerator,
            ILogger logger)
        {
            _flatTextureGenerator = flatTextureGenerator;
            _file = file;
            _directory = directory;
            _vpkGenerator = vpkGenerator;
            _logger = logger;
        }

        public WorkingDirectories GetWorkingDirectories()
        {
            var tempSaveDirectory = Path.Combine(_directory.GetCurrentDirectory(), "flattened");

            return new WorkingDirectories
            {
                WorkingDirectory = tempSaveDirectory,
                TF2WorkingDirectory = Path.Combine(tempSaveDirectory, "tf2"),
                HL2WorkingDirectory = Path.Combine(tempSaveDirectory, "hl2")
            };
        }

        public async Task GenerateFlatTextures(string package, IEnumerable<string> materials, string saveTo, bool upscale, Action<string> setCurrentStatus)
        {
            await _flatTextureGenerator.Generate(package, materials, saveTo, upscale, setCurrentStatus);
        }

        public string ConsolidateTextures(WorkingDirectories directories)
        {
            var saveMaterialsTo = Path.Combine(_directory.GetCurrentDirectory(), "flat-textures");
            if (_directory.Exists(directories.TF2WorkingDirectory))
            {
                _directory.CopyDirectory(directories.TF2WorkingDirectory, saveMaterialsTo, recursive: true, overwrite: true);
            }
            if (_directory.Exists(directories.HL2WorkingDirectory))
            {
                _directory.CopyDirectory(directories.HL2WorkingDirectory, saveMaterialsTo, recursive: true, overwrite: true);
            }

            RemoveTempFiles(directories);

            return saveMaterialsTo;

            void RemoveTempFiles(WorkingDirectories directories)
            {
                _directory.Delete(directories.WorkingDirectory, recursive: true);
            }
        }

        public async Task<IEnumerable<string>> CreateOutput(string tf2Directory, string flattenedMaterialsDirectory, FlattenOutputType outputType)
        {
            IEnumerable<string> generatedFiles = new List<string>();

            switch (outputType)
            {
                case FlattenOutputType.SingleVPK:
                    generatedFiles = await _vpkGenerator.Generate(tf2Directory, flattenedMaterialsDirectory, multiChunk: false);
                    _directory.Delete(flattenedMaterialsDirectory, recursive: true);
                    break;
                case FlattenOutputType.MultiChunkVPK:
                    generatedFiles = await _vpkGenerator.Generate(tf2Directory, flattenedMaterialsDirectory, multiChunk: true);
                    _directory.Delete(flattenedMaterialsDirectory, recursive: true);
                    break;
                case FlattenOutputType.TextureFiles:
                    var materialsDirectory = Path.Combine(flattenedMaterialsDirectory, "materials");
                    generatedFiles = new List<string> { materialsDirectory };
                    break;
                default:
                    break;
            }

            return generatedFiles;
        }
    }
}
