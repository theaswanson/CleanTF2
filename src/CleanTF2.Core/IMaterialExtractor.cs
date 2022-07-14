namespace CleanTF2.Core
{
    public interface IMaterialExtractor
    {
        /// <summary>
        /// Extracts the given materials from the given package to the given output directory.
        /// </summary>
        /// <param name="package">Path to .vpk package containing the given materials (e.g. tf2_textures_dir.vpk).</param>
        /// <param name="materials">List of paths to material files or directories to flatten.</param>
        /// <param name="outputDirectory">Directory to save the materials to.</param>
        /// <returns></returns>
        Task ExtractAllMaterials(string package, IEnumerable<string> materials, string outputDirectory, Action<string> setCurrentStatus);
    }
}
