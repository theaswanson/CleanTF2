using CleanTF2.Core.Materials;

namespace CleanTF2.Core.Tests
{
    public class TheMaterialFlattener
    {
        protected MaterialFlattener _materialFlattener;
        protected Mock<IImageManipulator> _imageManipulator;

        [SetUp]
        public virtual void Setup()
        {
            _imageManipulator = new Mock<IImageManipulator>();
            _imageManipulator.Setup(im => im.WithImage(It.IsAny<string>())).Returns(_imageManipulator.Object);
            _imageManipulator.Setup(im => im.Resize(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<bool>())).Returns(_imageManipulator.Object);
            _imageManipulator.Setup(im => im.Composite(It.IsAny<string>())).Returns(_imageManipulator.Object);

            _materialFlattener = new MaterialFlattener(_imageManipulator.Object);
        }

        [Test]
        public async Task WithoutUpscale_FlattensMaterial()
        {
            var image = "path/to/image.tga";
            
            await _materialFlattener.FlattenMaterial(image, false);

            _imageManipulator.Verify(im => im.WithImage(image));
            _imageManipulator.Verify(im => im.Resize(1, 1, true));
            _imageManipulator.Verify(im => im.Resize(1024, 1024, It.IsAny<bool>()), Times.Never);
            _imageManipulator.Verify(im => im.Save(image));
        }

        [Test]
        public async Task WithUpscale_FlattensMaterialAndUpscales()
        {
            var image = "path/to/image.tga";

            await _materialFlattener.FlattenMaterial(image, true);

            _imageManipulator.Verify(im => im.WithImage(image));
            _imageManipulator.Verify(im => im.Resize(1, 1, true));
            _imageManipulator.Verify(im => im.Resize(1024, 1024, true));
            _imageManipulator.Verify(im => im.Save(image));
        }
    }
}
