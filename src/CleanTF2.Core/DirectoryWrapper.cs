namespace CleanTF2.Core
{
    public class DirectoryWrapper : IDirectory
    {
        public DirectoryInfo CreateDirectory(string path) => Directory.CreateDirectory(path);
    }
}
