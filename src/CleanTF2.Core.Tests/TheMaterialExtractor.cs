using CleanTF2.Core.Materials;
using CleanTF2.Core.Utilities;
using CleanTF2.Core.Valve;

namespace CleanTF2.Core.Tests
{
    [TestFixture]
    public class TheMaterialExtractor
    {
        private MaterialExtractor _extractor;
        private Mock<IDirectory> _directory;
        private Mock<IHLExtract> _hlExtract;
        private string _package;
        private IEnumerable<string> _materials;
        private string _outputDirectory;
        private Action<string> _setCurrentStatus;
        private List<string> _statuses;

        [SetUp]
        public virtual void Setup()
        {
            _directory = new Mock<IDirectory>();
            _hlExtract = new Mock<IHLExtract>();
            _extractor = new MaterialExtractor(_directory.Object, _hlExtract.Object);

            _package = "some-package.vpk";
            _materials = Enumerable.Empty<string>();
            _outputDirectory = Path.Combine("some", "output", "directory");
            _statuses = new List<string>();
            _setCurrentStatus = (string status) => { _statuses.Add(status); };
        }

        protected Task Act() => _extractor.ExtractAllMaterials(_package, _materials, _outputDirectory, _setCurrentStatus);

        public class GivenNoMaterials : TheMaterialExtractor
        {
            public override void Setup()
            {
                base.Setup();
                _materials = Enumerable.Empty<string>();
            }

            [Test]
            public async Task DoesNothing()
            {
                await Act();

                _hlExtract.Verify(hl => hl.Run(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<IEnumerable<string>>(), false, false, false), Times.Never);
                Assert.That(_statuses, Is.Empty);
            }
        }

        public class GivenOneMaterials : TheMaterialExtractor
        {
            public override void Setup()
            {
                base.Setup();
                _materials = new List<string>
                {
                    //"materials\\concrete",
                    //"materials\\wall",
                    "materials\\harvest\\harvest_grass.vtf"
                };
            }

            [Test]
            public async Task SetsTheStatus()
            {
                await Act();

                var status = _statuses.Single();
                Assert.That(status, Is.EqualTo("Extracting material 1/1"));
            }

            [Test]
            public async Task ExtractsTheMaterial()
            {
                await Act();

                var extractTo = Path.Combine(_outputDirectory, "materials\\harvest");
                _directory.Verify(d => d.CreateDirectory(extractTo));
                _hlExtract.Verify(hl => hl.Run(_package, extractTo, _materials, true, true, true));
            }
        }

        public class GivenOneMaterialDirectory : TheMaterialExtractor
        {
            public override void Setup()
            {
                base.Setup();
                _materials = new List<string>
                {
                    "materials\\concrete",
                };
            }

            [Test]
            public async Task SetsTheStatus()
            {
                await Act();

                var status = _statuses.Single();
                Assert.That(status, Is.EqualTo("Extracting materials from 1 directories"));
            }

            [Test]
            public async Task ExtractsTheMaterial()
            {
                await Act();

                var extractTo = Path.Combine(_outputDirectory, "materials");
                _directory.Verify(d => d.CreateDirectory(extractTo));
                _hlExtract.Verify(hl => hl.Run(_package, extractTo, _materials, true, true, true));
            }
        }

        public class GivenMaterialsAndDirectories : TheMaterialExtractor
        {
            public override void Setup()
            {
                base.Setup();
                _materials = new List<string>
                {
                    "materials\\test\\test-material.vtf",
                    "materials\\wall",
                    "materials\\test\\yeet.vtf",
                    "materials\\ground",
                    "materials\\long\\path\\lol",
                };
            }

            [Test]
            public async Task SetsTheStatus()
            {
                await Act();

                CollectionAssert.AreEqual(_statuses, new List<string>
                {
                    "Extracting material 1/2",
                    "Extracting material 2/2",
                    "Extracting materials from 3 directories"
                });
            }

            [Test]
            public async Task ExtractsTheMaterials()
            {
                await Act();

                // Single materials
                _directory.Verify(d => d.CreateDirectory(Path.Combine(_outputDirectory, "materials\\test")));
                _hlExtract.Verify(hl => hl.Run(_package, Path.Combine(_outputDirectory, "materials\\test"), It.Is<IEnumerable<string>>(l => l.Count() == 1 && l.Contains("materials\\test\\test-material.vtf")), true, true, true));
                _hlExtract.Verify(hl => hl.Run(_package, Path.Combine(_outputDirectory, "materials\\test"), It.Is<IEnumerable<string>>(l => l.Count() == 1 && l.Contains("materials\\test\\yeet.vtf")), true, true, true));

                // Directories of materials
                _directory.Verify(d => d.CreateDirectory(Path.Combine(_outputDirectory, "materials")));
                _hlExtract.Verify(hl => hl.Run(_package, Path.Combine(_outputDirectory, "materials"), It.Is<IEnumerable<string>>(materials => materials.Count() == 3), true, true, true));
            }
        }
    }
}
