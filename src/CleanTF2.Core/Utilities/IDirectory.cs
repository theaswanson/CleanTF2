namespace CleanTF2.Core.Utilities
{
    public interface IDirectory
    {
        DirectoryInfo CreateDirectory(string path);
        string[] GetFiles(string path);
    }
}
