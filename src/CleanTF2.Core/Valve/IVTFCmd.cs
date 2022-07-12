namespace CleanTF2.Core.Valve
{
    public interface IVTFCmd
    {
        Task Run(string file, string? output = null, string? format = null, string? alphaFormat = null, string? exportFormat = null, string? flag = null, string? version = null);
    }
}
