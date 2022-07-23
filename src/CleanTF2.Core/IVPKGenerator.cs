namespace CleanTF2.Core
{
    public interface IVPKGenerator
    {
        Task<IEnumerable<string>> Generate(string tf2Directory, string directoryToPack, bool multiChunk);
    }
}