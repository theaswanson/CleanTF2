using CleanTF2.Core.Utilities;
using DeepEqual.Syntax;

namespace CleanTF2.Core.Tests
{
    public class TheDefaults
    {
        private Defaults _defaults;
        private Mock<IFile> _file;
        private const string DefaultTF2Directory = @"C:\Program Files (x86)\Steam\steamapps\common\Team Fortress 2";

        [SetUp]
        public void Setup()
        {
            _file = new Mock<IFile>();
            _defaults = new Defaults(_file.Object);
        }

        [Test]
        public void TF2Directory()
        {
            var actual = _defaults.TF2Directory();
            Assert.That(actual, Is.EqualTo(DefaultTF2Directory));
        }

        [Test]
        public void GivenNoTF2Directory_TF2TexturesPackageUsesDefaultDirectory()
        {
            var actual = _defaults.TF2TexturesPackage();
            Assert.That(actual, Is.EqualTo(Path.Combine(DefaultTF2Directory, "tf", "tf2_textures_dir.vpk")));
        }

        [Test]
        public void GivenTF2Directory_TF2TexturesPackageUsesThatDirectory()
        {
            var customDirectory = "custom/install/path/for/Team Fortress 2";
            var actual = _defaults.TF2TexturesPackage(customDirectory);
            Assert.That(actual, Is.EqualTo(Path.Combine(customDirectory, "tf", "tf2_textures_dir.vpk")));
        }

        [Test]
        public void GivenNoTF2Directory_HL2TexturesPackageUsesDefaultDirectory()
        {
            var actual = _defaults.HL2TexturesPackage();
            Assert.That(actual, Is.EqualTo(Path.Combine(DefaultTF2Directory, "hl2", "hl2_textures_dir.vpk")));
        }

        [Test]
        public void GivenTF2Directory_HL2TexturesPackageUsesThatDirectory()
        {
            var customDirectory = "custom/install/path/for/Team Fortress 2";
            var actual = _defaults.HL2TexturesPackage(customDirectory);
            Assert.That(actual, Is.EqualTo(Path.Combine(customDirectory, "hl2", "hl2_textures_dir.vpk")));
        }

        [Test]
        public async Task TF2MaterialList_ReadsFromIncludedFile()
        {
            var expectedList = new List<string> { "materials/brick" };
            _file.Setup(f => f.ReadAllLinesAsync("flat.txt")).ReturnsAsync(expectedList.ToArray());
            var list = await _defaults.TF2MaterialList();
            list.ShouldDeepEqual(expectedList);
        }

        [Test]
        public async Task HL2MaterialList_ReadsFromIncludedFile()
        {
            var expectedList = new List<string> { "materials/brick" };
            _file.Setup(f => f.ReadAllLinesAsync("flat_hl2.txt")).ReturnsAsync(expectedList.ToArray());
            var list = await _defaults.HL2MaterialList();
            list.ShouldDeepEqual(expectedList);
        }
    }
}
