using DeepEqual.Syntax;
using Moq;

namespace CleanTF2.Core.Tests
{
    public class TheHLExtract
    {
        private Mock<IProcessRunner> _processRunner;
        private HLExtract _hlextract;

        [SetUp]
        public virtual void Setup()
        {
            _processRunner = new Mock<IProcessRunner>();
            _hlextract = new HLExtract(_processRunner.Object);
        }

        public class GivenRequiredArgs : TheHLExtract
        {
            protected string _package;
            protected string _extractDirectory;
            protected string _pathToExtract;

            public override void Setup()
            {
                base.Setup();
                _package = "package.vpk";
                _extractDirectory = "path/to/dir";
                _pathToExtract = "dir/to/extract";
            }

            [Test]
            public void CallsLibraryWithArgsAndDefaults()
            {
                _hlextract.Run(_package, _extractDirectory, _pathToExtract);
                _processRunner.Verify(p => p.Start(
                    It.Is<string>(fileName => fileName == "hlextract.exe"),
                    It.Is<IEnumerable<string>>(args => args.IsDeepEqual(new List<string> { "-p", _package, "-d", _extractDirectory, "-e", "dir/to/extract" }))
                ));
            }

            [Test]
            public void WhenUsingFileMapping_AddsArg()
            {
                _hlextract.Run(_package, _extractDirectory, _pathToExtract, useFileMapping: true);
                _processRunner.Verify(p => p.Start(
                    It.IsAny<string>(),
                    It.Is<IEnumerable<string>>(args => args.Contains("-m"))
                ));
            }

            [Test]
            public void WhenAllowingVolatileAccess_AddsArg()
            {
                _hlextract.Run(_package, _extractDirectory, _pathToExtract, allowVolatileAccess: true);
                _processRunner.Verify(p => p.Start(
                    It.IsAny<string>(),
                    It.Is<IEnumerable<string>>(args => args.Contains("-v"))
                ));
            }

            [Test]
            public void WhenUsingSilentMode_AddsArg()
            {
                _hlextract.Run(_package, _extractDirectory, _pathToExtract, useSilentMode: true);
                _processRunner.Verify(p => p.Start(
                    It.IsAny<string>(),
                    It.Is<IEnumerable<string>>(args => args.Contains("-s"))
                ));
            }
        }
    }
}
