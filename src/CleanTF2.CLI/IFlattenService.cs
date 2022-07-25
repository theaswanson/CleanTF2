using CleanTF2.Core;

namespace CleanTF2.CLI
{
    /// <summary>
    /// Handles the flattening of TF2 and HL2 textures.
    /// </summary>
    public interface IFlattenService
    {
        /// <summary>
        /// Flattens textures and outputs them while reporting progress to the console.
        /// </summary>
        /// <param name="tf2Directory">Path to the Team Fortress 2 folder.</param>
        /// <param name="upscaleTextures"></param>
        /// <param name="outputType"></param>
        /// <returns></returns>
        Task Flatten(string tf2Directory, bool upscaleTextures, FlattenOutputType outputType);
    }
}
