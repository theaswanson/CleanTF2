namespace CleanTF2.Core
{
    public interface IFlatTextureGenerator
    {
        Task Generate(string vpk, IEnumerable<string> materialsToFlatten, string outputDirectory, bool resize);
    }
}