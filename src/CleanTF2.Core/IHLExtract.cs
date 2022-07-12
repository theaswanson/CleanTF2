using System.Diagnostics;

namespace CleanTF2.Core
{
    public interface IHLExtract
    {
        Task Run(string package, string extractDirectory, string pathToExtract, bool useFileMapping = false, bool allowVolatileAccess = false, bool useSilentMode = false);
    }
}