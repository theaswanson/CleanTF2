namespace CleanTF2.Core
{
    public class FileWrapper : IFile
    {
        public bool IsDirectory(string path) => File.GetAttributes(@"c:\Temp").HasFlag(FileAttributes.Directory);
    }
}
