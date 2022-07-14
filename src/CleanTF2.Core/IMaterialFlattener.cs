namespace CleanTF2.Core
{
    public interface IMaterialFlattener
    {
        /// <summary>
        /// Flattens the given .tga file.
        /// </summary>
        /// <param name="pathToImage">Path to a .tga file.</param>
        /// <param name="upscale">If true, resizes the flattened material to 1024x1024.</param>
        /// <returns></returns>
        Task FlattenMaterial(string pathToImage, bool upscale);
    }
}
