using CleanTF2.Core.Utilities;
using CleanTF2.Core.Libraries;
using DeepEqual.Syntax;

namespace CleanTF2.Core.Tests
{
    public class TheVPKGenerator
    {
        private Mock<IFile> _file;
        private Mock<IDirectory> _directory;
        private Mock<IVPK> _vpk;
        private VPKGenerator _vpkGenerator;

        private string _tf2Directory;
        private string _directoryToPack;

        [SetUp]
        public void Setup()
        {
            _file = new Mock<IFile>();
            _directory = new Mock<IDirectory>();
            _vpk = new Mock<IVPK>();
            _vpkGenerator = new VPKGenerator(_file.Object, _directory.Object, _vpk.Object);
            _tf2Directory = "path/to/tf2";
            _directoryToPack = "my-textures/are/here";
        }

        [Test]
        public async Task CreatesREADME()
        {
            await _vpkGenerator.Generate(_tf2Directory, _directoryToPack, default);
            _file.Verify(f => f.WriteAllLinesAsync(Path.Combine(_directoryToPack, "README.txt"), It.Is<List<string>>(l => l.Contains("Created by CleanTF2") && l.Contains("https://github.com/theaswanson/CleanTF2"))));
        }

        [TestCase(true)]
        [TestCase(false)]
        public async Task RunsVPKProgram(bool multiChunk)
        {
            await _vpkGenerator.Generate(_tf2Directory, _directoryToPack, multiChunk);
            _vpk.Verify(v => v.Run(_tf2Directory, _directoryToPack, multiChunk));
        }

        [Test]
        public async Task WhenMultiChunk_ReturnsGeneratedVPKs()
        {
            _directory.Setup(d => d.GetFiles(Path.GetDirectoryName(_directoryToPack), "here_*.vpk", SearchOption.TopDirectoryOnly)).Returns(new string[] { "here.vpk" });
            var vpks = await _vpkGenerator.Generate(_tf2Directory, _directoryToPack, true);
            vpks.ShouldDeepEqual(new List<string> { "here.vpk" });
        }

        [Test]
        public async Task WithoutMultiChunk_ReturnsGeneratedVPK()
        {
            _directory.Setup(d => d.GetFiles(Path.GetDirectoryName(_directoryToPack), "here.vpk", SearchOption.TopDirectoryOnly)).Returns(new string[] { "here.vpk" });
            var vpks = await _vpkGenerator.Generate(_tf2Directory, _directoryToPack, default);
            vpks.ShouldDeepEqual(new List<string> { "here.vpk" });
        }
    }
}
