namespace CleanTF2.Core.Utilities
{
    public class DirectoryWrapper : IDirectory
    {
        public DirectoryInfo CreateDirectory(string path) => Directory.CreateDirectory(path);
        /// <summary>
        /// Returns the names of files (including their paths) in the specified directory.
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public string[] GetFiles(string path) => Directory.GetFiles(path);
    }
}
