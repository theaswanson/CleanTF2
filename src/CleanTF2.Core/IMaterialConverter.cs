namespace CleanTF2.Core
{
    public interface IMaterialConverter
    {
        Task<IEnumerable<string>> ConvertFromTGAtoVTF(string directory);
        Task<IEnumerable<string>> ConvertFromVTFtoTGA(string directory);
    }
}