using CleanTF2.Core.Utilities;
using CleanTF2.Core.Valve;
using Moq;

namespace CleanTF2.Core.Tests
{
    public class TheFlatTextureGenerator
    {
        private FlatTextureGenerator _flatTextureGenerator;
        private Mock<IFile> _file;
        private Mock<IDirectory> _directory;
        private Mock<IHLExtract> _hlExtract;
        private Mock<IVTFCmd> _vtfCmd;
        private Mock<IImageManipulator> _imageManipulator;

        [SetUp]
        public virtual void Setup()
        {
            _file = new Mock<IFile>();
            _directory = new Mock<IDirectory>();
            _hlExtract = new Mock<IHLExtract>();
            _vtfCmd = new Mock<IVTFCmd>();
            _imageManipulator = new Mock<IImageManipulator>();
            _imageManipulator.Setup(im => im.WithImage(It.IsAny<string>())).Returns(_imageManipulator.Object);
            _imageManipulator.Setup(im => im.Resize(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<bool>())).Returns(_imageManipulator.Object);
            _imageManipulator.Setup(im => im.Composite(It.IsAny<string>())).Returns(_imageManipulator.Object);
            _flatTextureGenerator = new FlatTextureGenerator(_file.Object, _directory.Object, _hlExtract.Object, _vtfCmd.Object, _imageManipulator.Object);
        }

        public class WhenGenerating : TheFlatTextureGenerator
        {
            protected string _vpk;
            protected List<string> _materialsToFlatten;
            protected string _outputDirectory;
            protected bool _resize;

            protected async Task Act() => await _flatTextureGenerator.Generate(_vpk, _materialsToFlatten, _outputDirectory, _resize);

            public override void Setup()
            {
                base.Setup();
                _vpk = "tf2-texture-dir.vpk";
                _outputDirectory = "working";
                _materialsToFlatten = new List<string>();
                _resize = false;
            }

            public class GivenAMaterialDirectory : WhenGenerating
            {
                protected string _materialDirectory;

                public override void Setup()
                {
                    base.Setup();
                    _materialDirectory = "materials/brick";
                    _materialsToFlatten.Add(_materialDirectory);
                }

                [Test]
                public async Task CreatesTheWorkingDirectories()
                {
                    await Act();

                    var expectedExtractedDirectory = Path.Combine(_outputDirectory, "extracted", _materialDirectory);
                    var expectedConvertedDirectory = Path.Combine(_outputDirectory, "converted", _materialDirectory);
                    _directory.Verify(d => d.CreateDirectory(expectedExtractedDirectory));
                    _directory.Verify(d => d.CreateDirectory(expectedConvertedDirectory));
                }

                [Test]
                public async Task ExtractsTheVtfFiles()
                {
                    await Act();

                    var expectedExtractedDirectory = Path.Combine(_outputDirectory, "extracted", _materialDirectory);
                    _hlExtract.Verify(hl => hl.Run(_vpk, expectedExtractedDirectory, _materialDirectory, default, default, default));
                }

                public class GivenNoVtfFilesInDirectory : GivenAMaterialDirectory
                {
                    public override void Setup()
                    {
                        base.Setup();
                        _directory.Setup(d => d.GetFiles(_materialDirectory)).Returns(Enumerable.Empty<string>().ToArray());
                    }

                    [Test]
                    public void DoesNotThrowException()
                    {
                        Assert.DoesNotThrowAsync(async () => await Act());
                    }
                }

                public class GivenVtfFiles : GivenAMaterialDirectory
                {
                    protected string[] _files;

                    public override void Setup()
                    {
                        base.Setup();
                        _files = new List<string> { "wall.vtf" }.ToArray();
                        var expectedExtractedDirectory = Path.Combine(_outputDirectory, "extracted", _materialDirectory);
                        _directory.Setup(d => d.GetFiles(expectedExtractedDirectory)).Returns(_files);
                    }

                    [Test]
                    public async Task ConvertsToTGA()
                    {
                        await Act();

                        _vtfCmd.Verify(cmd => cmd.Run(
                            "wall.vtf",
                            default,
                            default,
                            default,
                            "tga",
                            default,
                            default));
                    }

                    [Test]
                    public async Task AveragesTheColor()
                    {
                        await Act();

                        Assert.Multiple(() =>
                        {
                            _imageManipulator.Verify(im => im.WithImage("wall.tga"));
                            _imageManipulator.Verify(im => im.Resize(1, 1, true));
                            _imageManipulator.Verify(im => im.Resize(1024, 1024, true));
                            _imageManipulator.Verify(im => im.Composite("wall.tga"));
                        });
                    }

                    [Test]
                    public async Task GivenResizeIsTrue_ResizesTexture()
                    {
                        _resize = true;
                        await Act();
                        _imageManipulator.Verify(im => im.Resize(1, 1, true), Times.Exactly(2));
                    }

                    [Test]
                    public async Task SavesTheFlattenedTexture()
                    {
                        await Act();

                        var expectedSaveFile = Path.Combine(_outputDirectory, "converted", _materialDirectory, "wall.tga");
                        _imageManipulator.Verify(im => im.Save(Path.Combine(expectedSaveFile)));
                    }

                    [Test]
                    public async Task ConvertsTheFlattenedTextureToVTF()
                    {
                        await Act();
                        var expectedSaveDirectory = Path.Combine(_outputDirectory, "converted", _materialDirectory);
                        _vtfCmd.Verify(cmd => cmd.Run(
                            "wall.tga",
                            expectedSaveDirectory,
                            "BGR888",
                            "ABGR8888",
                            default,
                            "MINMIP",
                            "7.4"
                        ));
                    }

                    [Test]
                    public async Task DeletesTheTempTGA()
                    {
                        await Act();
                        _file.Verify(f => f.Delete("wall.tga"));
                    }
                }
            }
        }
    }
}
