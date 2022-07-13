using System.Text.RegularExpressions;

namespace CleanTF2.Core.Utilities
{
    public class FileWrapper : IFile
    {
        public void Delete(string path) => File.Delete(path);
    }
}
