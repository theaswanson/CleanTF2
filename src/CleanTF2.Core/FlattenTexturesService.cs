using CleanTF2.Core.Materials;
using CleanTF2.Core.Utilities;

namespace CleanTF2.Core
{
    public class FlattenTexturesService : IFlattenTexturesService
    {
        private readonly IFlatMaterialGenerator _flatTextureGenerator;
        private readonly IDirectory _directory;
        private readonly IVPKGenerator _vpkGenerator;

        public FlattenTexturesService(
            IFlatMaterialGenerator flatTextureGenerator,
            IDirectory directory,
            IVPKGenerator vpkGenerator)
        {
            _flatTextureGenerator = flatTextureGenerator;
            _directory = directory;
            _vpkGenerator = vpkGenerator;
        }

        public async Task GenerateFlatTextures(string package, IEnumerable<string> materials, string saveTo, bool upscale, Action<string> setCurrentStatus)
        {
            await _flatTextureGenerator.Generate(package, materials, saveTo, upscale, setCurrentStatus);
        }
        
        /// <inheritdoc/>
        public void ConsolidateTextures(string consolidateToThisDirectory, IEnumerable<string> textureDirectories)
        {
            foreach (var directory in textureDirectories)
            {
                if (!_directory.Exists(directory))
                {
                    continue;
                }
                _directory.CopyDirectory(directory, consolidateToThisDirectory, recursive: true, overwrite: true);
            }
        }

        public async Task<IEnumerable<string>> CreateOutput(string tf2Directory, string flattenedMaterialsDirectory, FlattenOutputType outputType)
        {
            IEnumerable<string> generatedFiles = new List<string>();

            switch (outputType)
            {
                case FlattenOutputType.SingleVPK:
                    generatedFiles = await _vpkGenerator.Generate(tf2Directory, flattenedMaterialsDirectory, multiChunk: false);
                    break;
                case FlattenOutputType.MultiChunkVPK:
                    generatedFiles = await _vpkGenerator.Generate(tf2Directory, flattenedMaterialsDirectory, multiChunk: true);
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

        /// <inheritdoc/>
        public void CleanUp(string flattenedMaterialsDirectory, FlattenOutputType outputType)
        {
            switch (outputType)
            {
                case FlattenOutputType.SingleVPK:
                    _directory.Delete(flattenedMaterialsDirectory, recursive: true);
                    break;
                case FlattenOutputType.MultiChunkVPK:
                    _directory.Delete(flattenedMaterialsDirectory, recursive: true);
                    break;
                case FlattenOutputType.TextureFiles:
                    break;
            }
        }
    }
}
