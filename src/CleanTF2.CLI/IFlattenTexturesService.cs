namespace CleanTF2.CLI
{
    public interface IFlattenTexturesService
    {
        Task Flatten(string tf2Directory, bool upscaleTextures, OutputType outputType);
    }
}