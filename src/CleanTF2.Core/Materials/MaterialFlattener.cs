namespace CleanTF2.Core.Materials
{
    public class MaterialFlattener : IMaterialFlattener
    {
        private readonly IImageManipulator _imageManipulator;

        public MaterialFlattener(IImageManipulator imageManipulator)
        {
            _imageManipulator = imageManipulator;
        }

        /// <inheritdoc/>
        public async Task FlattenMaterial(string pathToImage, bool upscale)
        {
            // TODO: determine how composite should work
            _imageManipulator
                .WithImage(pathToImage)
                .Resize(1, 1);

            if (upscale)
            {
                _imageManipulator.Resize(1024, 1024);
            }

            // Overwrite original tga
            await _imageManipulator.Save(pathToImage);
        }
    }
}
