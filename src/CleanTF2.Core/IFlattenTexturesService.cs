namespace CleanTF2.Core
{
    public interface IFlattenTexturesService
    {
        WorkingDirectories GetWorkingDirectories();
        Task GenerateFlatTextures(string package, IEnumerable<string> materials, string saveTo, bool upscale, Action<string> setCurrentStatus);
        string ConsolidateTextures(WorkingDirectories directories);
        Task<IEnumerable<string>> CreateOutput(string tf2Directory, string flattenedMaterialsDirectory, FlattenOutputType outputType);
    }
}
