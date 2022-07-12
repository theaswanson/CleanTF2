namespace CleanTF2.Core.Utilities
{
    public interface IFile
    {
        void Delete(string path);
        bool IsDirectory(string path);
    }
}