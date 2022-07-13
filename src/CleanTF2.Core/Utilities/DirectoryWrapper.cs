namespace CleanTF2.Core.Utilities
{
    public class DirectoryWrapper : IDirectory
    {
        public DirectoryInfo CreateDirectory(string path) => Directory.CreateDirectory(path);
        public string GetCurrentDirectory() => Directory.GetCurrentDirectory();
        public string[] GetFiles(string path) => Directory.GetFiles(path);
        public string[] GetFiles(string path, string searchPattern, SearchOption searchOption) => Directory.GetFiles(path, searchPattern, searchOption);
    }
}
