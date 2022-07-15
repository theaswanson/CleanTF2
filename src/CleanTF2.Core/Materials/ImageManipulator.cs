using System.Diagnostics.CodeAnalysis;
using ImageMagick;

namespace CleanTF2.Core.Materials
{
    [ExcludeFromCodeCoverage]
    public class ImageManipulator : IDisposable, IImageManipulator
    {
        private MagickImage? _image;

        public IImageManipulator WithImage(string path)
        {
            _image = new MagickImage(path);
            return this;
        }

        public IImageManipulator Resize(int width, int height, bool ignoreAspectRatio = true)
        {
            _image!.Resize(new MagickGeometry(width, height)
            {
                IgnoreAspectRatio = ignoreAspectRatio
            });
            return this;
        }

        public IImageManipulator Composite(string secondImage)
        {
            using var sourceImage = new MagickImage(secondImage);
            _image!.Composite(sourceImage, CompositeOperator.Src);
            return this;
        }

        public async Task Save(string saveTo)
        {
            await _image!.WriteAsync(saveTo);
            _image.Dispose();
        }

        public void Dispose() => _image?.Dispose();
    }
}
