namespace CleanTF2.Core.Utilities
{
    public interface IDirectory
    {
        void CopyDirectory(string sourceDir, string destinationDir, bool recursive, bool overwrite = false);
        DirectoryInfo CreateDirectory(string path);
        void Delete(string path, bool recursive);
        bool Exists(string? path);
        string GetCurrentDirectory();
        string[] GetFiles(string path);
        string[] GetFiles(string path, string searchPattern, SearchOption searchOption);
    }
}
