namespace CleanTF2.Core.Materials
{
    public interface IFlatMaterialGenerator
    {
        /// <summary>
        /// Creates flattened versions of the materials from the package and saves them to the given directory.
        /// </summary>
        /// <param name="package">Path to .vpk package containing the given materials (e.g. tf2_textures_dir.vpk).</param>
        /// <param name="materials">List of paths to material files or directories to flatten (e.g. materials/concrete/ceiling_01.vtf or materials/concrete).</param>
        /// <param name="outputDirectory">Directory to save the flattened materials to.</param>
        /// <param name="upscale">If true, resizes materials to 1024x1024. If false, leaves materials at 1x1.</param>
        Task Generate(string package, IEnumerable<string> materials, string outputDirectory, bool upscale, Action<string> setCurrentStatus);
    }
}
