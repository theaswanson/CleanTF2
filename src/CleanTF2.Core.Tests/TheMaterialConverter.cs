using CleanTF2.Core.Utilities;
using CleanTF2.Core.Valve;

namespace CleanTF2.Core.Tests
{
    public class TheMaterialConverter
    {
        protected MaterialConverter _materialConverter;
        protected Mock<IVTFCmd> _vtfCmd;
        protected Mock<IDirectory> _directory;

        [SetUp]
        public virtual void Setup()
        {
            _vtfCmd = new Mock<IVTFCmd>();
            _directory = new Mock<IDirectory>();
            _materialConverter = new MaterialConverter(_vtfCmd.Object, _directory.Object);
        }

        [Test]
        public async Task ConvertsFromVTFtoTGA()
        {
            var directory = "path/to/materials";
            var expectedConvertedFiles = new List<string> { "sup.tga", "hello.tga" };
            _directory.Setup(d => d.GetFiles(directory, "*.tga", SearchOption.AllDirectories)).Returns(expectedConvertedFiles.ToArray());

            var convertedFiles = await _materialConverter.ConvertFromVTFtoTGA(directory);
            
            _vtfCmd.Verify(vtf => vtf.Run(null, Path.Combine(directory, "*.vtf"), null, null, "tga", null, null), Times.Once);
            CollectionAssert.AreEqual(expectedConvertedFiles, convertedFiles);
        }

        [Test]
        public async Task ConvertsFromTGAtoVTF()
        {
            var directory = "path/to/materials";
            var expectedConvertedFiles = new List<string> { "sup.vtf", "hello.vtf" };
            _directory.Setup(d => d.GetFiles(directory, "*.vtf", SearchOption.AllDirectories)).Returns(expectedConvertedFiles.ToArray());

            var convertedFiles = await _materialConverter.ConvertFromTGAtoVTF(directory);

            _vtfCmd.Verify(vtf => vtf.Run(null, Path.Combine(directory, "*.tga"), "BGR888", "ABGR8888", null, "MINMIP", "7.4"), Times.Once);
            CollectionAssert.AreEqual(expectedConvertedFiles, convertedFiles);
        }
    }
}
