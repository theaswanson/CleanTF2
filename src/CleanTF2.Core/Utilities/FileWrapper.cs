namespace CleanTF2.Core.Utilities
{
    public class FileWrapper : IFile
    {
        public bool IsDirectory(string path) => File.GetAttributes(@"c:\Temp").HasFlag(FileAttributes.Directory);
        public void Delete(string path) => File.Delete(path);
    }
}
