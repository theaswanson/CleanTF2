using CleanTF2.Core.Utilities;
using CleanTF2.Core.Libraries;
using DeepEqual.Syntax;

namespace CleanTF2.Core.Tests
{
    public class TheVTFCmd
    {
        protected VTFCmd _vtfCmd;
        protected Mock<IProcessRunner> _processRunner;

        [SetUp]
        public virtual void Setup()
        {
            _processRunner = new Mock<IProcessRunner>();
            _vtfCmd = new VTFCmd(_processRunner.Object);
        }

        [Test]
        public async Task CallsLibrary()
        {
            var folder = "path/to/folder";
            await _vtfCmd.Run(folder: folder);
            _processRunner.Verify(p => p.Run(
                Path.Combine("lib", "vtfcmd.exe"),
                It.IsAny<IEnumerable<string>>()
            ));
        }

        [Test]
        public async Task GivenFile_AddsArg()
        {
            var file = "path/to/file.txt";
            await _vtfCmd.Run(file: file);
            _processRunner.Verify(p => p.Run(
                It.IsAny<string>(),
                It.Is<IEnumerable<string>>(args => args.IsDeepEqual(new List<string> { "-file", file }))
            ));
        }

        [Test]
        public async Task GivenFolder_AddsArg()
        {
            var folder = "path/to/folder";
            await _vtfCmd.Run(folder: folder);
            _processRunner.Verify(p => p.Run(
                It.IsAny<string>(),
                It.Is<IEnumerable<string>>(args => args.IsDeepEqual(new List<string> { "-folder", folder, "-recurse" }))
            ));
        }

        [Test]
        public async Task GivenFormat_AddsArg()
        {
            var format = "BGR888";
            await _vtfCmd.Run(format: format);
            _processRunner.Verify(p => p.Run(
                It.IsAny<string>(),
                It.Is<IEnumerable<string>>(args => args.IsDeepEqual(new List<string> { "-format", format }))
            ));
        }

        [Test]
        public async Task GivenAlphaFormat_AddsArg()
        {
            var alphaFormat = "ABGR8888";
            await _vtfCmd.Run(alphaFormat: alphaFormat);
            _processRunner.Verify(p => p.Run(
                It.IsAny<string>(),
                It.Is<IEnumerable<string>>(args => args.IsDeepEqual(new List<string> { "-alphaformat", alphaFormat }))
            ));
        }

        [Test]
        public async Task GivenExportFormat_AddsArg()
        {
            var exportFormat = "tga";
            await _vtfCmd.Run(exportFormat: exportFormat);
            _processRunner.Verify(p => p.Run(
                It.IsAny<string>(),
                It.Is<IEnumerable<string>>(args => args.IsDeepEqual(new List<string> { "-exportformat", exportFormat }))
            ));
        }

        [Test]
        public async Task GivenFlag_AddsArg()
        {
            var flag = "MINMIP";
            await _vtfCmd.Run(flag: flag);
            _processRunner.Verify(p => p.Run(
                It.IsAny<string>(),
                It.Is<IEnumerable<string>>(args => args.IsDeepEqual(new List<string> { "-flag", flag }))
            ));
        }

        [Test]
        public async Task GivenVersion_AddsArg()
        {
            var version = "7.4";
            await _vtfCmd.Run(version: version);
            _processRunner.Verify(p => p.Run(
                It.IsAny<string>(),
                It.Is<IEnumerable<string>>(args => args.IsDeepEqual(new List<string> { "-version", version }))
            ));
        }
    }
}
