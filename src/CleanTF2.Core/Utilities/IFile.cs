namespace CleanTF2.Core.Utilities
{
    public interface IFile
    {
        void Delete(string path);
        bool Exists(string path);
        Task<string[]> ReadAllLinesAsync(string path);
    }
}
