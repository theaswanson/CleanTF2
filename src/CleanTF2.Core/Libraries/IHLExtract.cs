namespace CleanTF2.Core.Libraries
{
    public interface IHLExtract
    {
        Task Run(string package, string extractDirectory, IEnumerable<string> pathToExtract, bool useFileMapping = false, bool allowVolatileAccess = false, bool useSilentMode = false);
    }
}
