namespace CleanTF2.Core.Valve
{
    public interface IHLExtract
    {
        Task Run(string package, string extractDirectory, IEnumerable<string> pathToExtract, bool useFileMapping = false, bool allowVolatileAccess = false, bool useSilentMode = false);
    }
}
