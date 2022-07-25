namespace CleanTF2.Core
{
    public interface IFlattenTexturesService
    {
        Task GenerateFlatTextures(string package, IEnumerable<string> materials, string saveTo, bool upscale, Action<string> setCurrentStatus);
        /// <summary>
        /// Moves all textures into a single directory while maintaining directory structure.
        /// </summary>
        /// <param name="consolidateToThisDirectory"></param>
        /// <param name="textureDirectories"></param>
        void ConsolidateTextures(string consolidateToThisDirectory, IEnumerable<string> textureDirectories);
        Task<IEnumerable<string>> CreateOutput(string tf2Directory, string flattenedMaterialsDirectory, FlattenOutputType outputType);
        /// <summary>
        /// Removes the flattened texture files, depending on the output type. 
        /// </summary>
        /// <param name="flattenedMaterialsDirectory"></param>
        /// <param name="outputType"></param>
        void CleanUp(string flattenedMaterialsDirectory, FlattenOutputType outputType);
    }
}
