using CleanTF2.Core;

namespace CleanTF2.CLI
{
    public interface IFlattenService
    {
        Task Flatten(string tf2Directory, bool upscaleTextures, FlattenOutputType outputType);
    }
}
