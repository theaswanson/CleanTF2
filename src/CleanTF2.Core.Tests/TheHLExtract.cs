using CleanTF2.Core.Utilities;
using CleanTF2.Core.Libraries;
using DeepEqual.Syntax;

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
            protected IEnumerable<string> _pathsToExtract;

            public override void Setup()
            {
                base.Setup();
                _package = "package.vpk";
                _extractDirectory = "path/to/dir";
                _pathsToExtract = new List<string> { "dir/to/extract" };
            }

            [Test]
            public async Task CallsLibraryWithArgsAndDefaults()
            {
                await _hlextract.Run(_package, _extractDirectory, _pathsToExtract);
                _processRunner.Verify(p => p.Run(
                    Path.Combine("lib", "hlextract.exe"),
                    It.Is<IEnumerable<string>>(args => args.IsDeepEqual(new List<string> { "-p", _package, "-d", _extractDirectory, "-e", "dir/to/extract" }))
                ));
            }

            [Test]
            public async Task WhenUsingFileMapping_AddsArg()
            {
                await _hlextract.Run(_package, _extractDirectory, _pathsToExtract, useFileMapping: true);
                _processRunner.Verify(p => p.Run(
                    It.IsAny<string>(),
                    It.Is<IEnumerable<string>>(args => args.Contains("-m"))
                ));
            }

            [Test]
            public async Task WhenAllowingVolatileAccess_AddsArg()
            {
                await _hlextract.Run(_package, _extractDirectory, _pathsToExtract, allowVolatileAccess: true);
                _processRunner.Verify(p => p.Run(
                    It.IsAny<string>(),
                    It.Is<IEnumerable<string>>(args => args.Contains("-v"))
                ));
            }

            [Test]
            public async Task WhenUsingSilentMode_AddsArg()
            {
                await _hlextract.Run(_package, _extractDirectory, _pathsToExtract, useSilentMode: true);
                _processRunner.Verify(p => p.Run(
                    It.IsAny<string>(),
                    It.Is<IEnumerable<string>>(args => args.Contains("-s"))
                ));
            }
        }
    }
}
