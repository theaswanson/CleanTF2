namespace CleanTF2.Core.Materials
{
    public interface IMaterialConverter
    {
        Task<IEnumerable<string>> ConvertFromTGAtoVTF(string directory);
        Task<IEnumerable<string>> ConvertFromVTFtoTGA(string directory);
    }
}