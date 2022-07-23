using CleanTF2.Core.Utilities;
using CleanTF2.Core.Libraries;

namespace CleanTF2.Core.Tests
{
    public class TheVPK
    {
        protected VPK _vpk;
        protected Mock<IProcessRunner> _processRunner;
        protected string _tf2Directory;
        protected string _directoryToPack;

        [SetUp]
        public virtual void Setup()
        {
            _processRunner = new Mock<IProcessRunner>();
            _vpk = new VPK(_processRunner.Object);
            _tf2Directory = "path/to/tf2";
            _directoryToPack = "textures.vpk";
        }

        [Test]
        public async Task CallsLibrary()
        {
            await _vpk.Run(_tf2Directory, _directoryToPack);
            _processRunner.Verify(p => p.Run(
                Path.Combine(_tf2Directory, "bin", "vpk.exe"),
                It.IsAny<IEnumerable<string>>()
            ));
        }

        [Test]
        public async Task IncludesDirectoryToPack()
        {
            await _vpk.Run(_tf2Directory, _directoryToPack);
            _processRunner.Verify(p => p.Run(
                It.IsAny<string>(),
                It.Is<IEnumerable<string>>(args => args.Contains(_directoryToPack))
            ));
        }

        [Test]
        public async Task GivenProduceMultiChunkVPK_AddsArg()
        {
            await _vpk.Run(_tf2Directory, _directoryToPack, produceMultiChunkVPK: true);
            _processRunner.Verify(p => p.Run(
                It.IsAny<string>(),
                It.Is<IEnumerable<string>>(args => args.Contains("-M"))
            ));
        }
    }
}
