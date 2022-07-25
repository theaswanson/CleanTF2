using CleanTF2.Core.Materials;
using CleanTF2.Core.Utilities;

namespace CleanTF2.Core.Tests
{
    public class TheFlatMaterialGenerator
    {
        private FlatMaterialGenerator _flatMaterialGenerator;
        private Mock<IFile> _file;
        private Mock<IMaterialExtractor> _materialExtractor;
        private Mock<IMaterialConverter> _materialConverter;
        private Mock<IMaterialFlattener> _materialFlattener;

        [SetUp]
        public virtual void Setup()
        {
            _file = new Mock<IFile>();
            _materialExtractor = new Mock<IMaterialExtractor>();
            _materialConverter = new Mock<IMaterialConverter>();
            _materialFlattener = new Mock<IMaterialFlattener>();
            _flatMaterialGenerator = new FlatMaterialGenerator(_file.Object, _materialExtractor.Object, _materialConverter.Object, _materialFlattener.Object);
        }

        public class WhenGenerating : TheFlatMaterialGenerator
        {
            protected string _package;
            protected List<string> _materialsToFlatten;
            protected string _outputDirectory;
            protected bool _upscale;
            protected List<string> _statuses;
            protected Action<string> _setCurrentStatus;

            protected async Task Act() => await _flatMaterialGenerator.Generate(_package, _materialsToFlatten, _outputDirectory, _upscale, _setCurrentStatus);

            public override void Setup()
            {
                base.Setup();
                // Simple happy path
                _package = "tf2-texture-dir.vpk";
                _materialsToFlatten = new List<string>();
                _outputDirectory = "working";
                _upscale = false;
                _statuses = new List<string>();
                _setCurrentStatus = (string status) => { _statuses.Add(status); };
            }

            public class GivenThePackageDoesNotExist : WhenGenerating
            {
                public override void Setup()
                {
                    base.Setup();
                    _file.Setup(f => f.Exists(_package)).Returns(false);
                }

                [Test]
                public void ThrowsException()
                {
                    var exception = Assert.ThrowsAsync<Exception>(async () => await Act());
                    Assert.That(exception.Message, Is.EqualTo("Could not find the package at tf2-texture-dir.vpk. Check the path and try again."));
                }
            }

            public class GivenThePackageExists : WhenGenerating
            {
                public override void Setup()
                {
                    base.Setup();
                    _file.Setup(f => f.Exists(_package)).Returns(true);
                }

                public class GivenNoMaterialsToFlatten : GivenThePackageExists
                {
                    public override void Setup()
                    {
                        base.Setup();
                        _materialsToFlatten.Clear();
                    }

                    [Test]
                    public async Task DoesNotExtractTextures()
                    {
                        await Act();
                        _materialExtractor.Verify(e => e.ExtractAllMaterials(It.IsAny<string>(), It.IsAny<IEnumerable<string>>(), It.IsAny<string>(), It.IsAny<Action<string>>()), Times.Never);
                    }

                    [Test]
                    public async Task DoesNotFlattenTextures()
                    {
                        await Act();
                        _materialFlattener.Verify(f => f.FlattenMaterial(It.IsAny<string>(), It.IsAny<bool>()), Times.Never);
                    }

                    [Test]
                    public async Task DoesNotConvertTextures()
                    {
                        await Act();
                        _materialConverter.Verify(c => c.ConvertFromVTFtoTGA(It.IsAny<string>()), Times.Never);
                        _materialConverter.Verify(c => c.ConvertFromTGAtoVTF(It.IsAny<string>()), Times.Never);
                    }
                }

                public class GivenAMaterialDirectory : GivenThePackageExists
                {
                    protected string _materialDirectory;
                    protected IEnumerable<string> _tgaFiles;

                    public override void Setup()
                    {
                        base.Setup();
                        _materialDirectory = "materials/brick";
                        _materialsToFlatten.Add(_materialDirectory);
                        _tgaFiles = new List<string>
                        {
                            "materials/brick/file.tga",
                            "materials/brick/other-file.tga"
                        };
                        _materialConverter.Setup(c => c.ConvertFromVTFtoTGA(_outputDirectory)).ReturnsAsync(_tgaFiles);
                    }

                    [Test]
                    public async Task ExtractsTheMaterials()
                    {
                        await Act();
                        _materialExtractor.Verify(e => e.ExtractAllMaterials(_package, _materialsToFlatten, _outputDirectory, _setCurrentStatus), Times.Once);
                    }

                    [Test]
                    public async Task ConvertsMaterials()
                    {
                        await Act();
                        _materialConverter.Verify(c => c.ConvertFromVTFtoTGA(_outputDirectory), Times.Once);
                        _materialConverter.Verify(c => c.ConvertFromTGAtoVTF(_outputDirectory), Times.Once);
                    }

                    [Test]
                    public async Task FlattensMaterials()
                    {
                        await Act();
                        foreach (var tga in _tgaFiles)
                        {
                            _materialFlattener.Verify(f => f.FlattenMaterial(tga, _upscale), Times.Once);
                        }
                    }

                    [Test]
                    public async Task SetsTheStatus()
                    {
                        await Act();
                        CollectionAssert.AreEqual(new List<string>
                        {
                            "Converting materials to .tga files",
                            "Flattening .tga file 1/2",
                            "Flattening .tga file 2/2",
                            "Converting .tga files to materials",
                            "Deleting .tga files"
                        }, _statuses);
                    }
                }
            }
        }
    }
}
