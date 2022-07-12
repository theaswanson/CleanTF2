using Moq;

namespace CleanTF2.Core.Tests
{
    public class TheFlatTextureGenerator
    {
        private FlatTextureGenerator _flatTextureGenerator;
        private Mock<IFile> _file;
        private Mock<IDirectory> _directory;
        private Mock<IHLExtract> _hlExtract;

        [SetUp]
        public virtual void Setup()
        {
            _file = new Mock<IFile>();
            _directory = new Mock<IDirectory>();
            _hlExtract = new Mock<IHLExtract>();
            _flatTextureGenerator = new FlatTextureGenerator(_file.Object, _directory.Object, _hlExtract.Object);
        }

        [Test]
        public async Task Works()
        {
            var materials = new List<string>
            {
                "materials/brick"
            };
            _file.Setup(f => f.IsDirectory("materials/brick")).Returns(true);

            await _flatTextureGenerator.Generate("tf2-texture-dir.vpk", materials);

            var expectedDirectory = Path.Combine("working", "materials/brick");
            _directory.Verify(d => d.CreateDirectory(expectedDirectory));
            _hlExtract.Verify(hl => hl.Run("tf2-texture-dir.vpk", expectedDirectory, "materials/brick", default, default, default));
	    }
    }
}
