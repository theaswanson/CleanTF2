namespace CleanTF2.Core.Utilities
{
    public interface IDirectory
    {
        DirectoryInfo CreateDirectory(string path);
        string GetCurrentDirectory();
        string[] GetFiles(string path);
        string[] GetFiles(string path, string searchPattern, SearchOption searchOption);
    }
}
