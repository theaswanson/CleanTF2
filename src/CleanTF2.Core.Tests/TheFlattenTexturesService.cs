using CleanTF2.Core.Materials;
using CleanTF2.Core.Utilities;
using DeepEqual.Syntax;

namespace CleanTF2.Core.Tests
{
    public class TheFlattenTexturesService
    {
        private FlattenTexturesService _flattenTexturesService;
        private Mock<IFlatMaterialGenerator> _flatMaterialGenerator;
        private Mock<IDirectory> _directory;
        private Mock<IVPKGenerator> _vpkGenerator;

        [SetUp]
        public void Setup()
        {
            _flatMaterialGenerator = new Mock<IFlatMaterialGenerator>();
            _directory = new Mock<IDirectory>();
            _vpkGenerator = new Mock<IVPKGenerator>();
            _flattenTexturesService = new FlattenTexturesService(_flatMaterialGenerator.Object, _directory.Object, _vpkGenerator.Object);
        }

        public class WhenGeneratingFlatTextures : TheFlattenTexturesService
        {
            [Test]
            public async Task CallsTheFlatTextureGenerator()
            {
                var package = "the-package.vpk";
                var materials = new List<string> { "material1", "material2" };
                var saveTo = "save-here";
                var upscale = true;
                var setCurrentStatus = (string status) => { };

                await _flattenTexturesService.GenerateFlatTextures(package, materials, saveTo, upscale, setCurrentStatus);
                
                _flatMaterialGenerator.Verify(g => g.Generate(package, materials, saveTo, upscale, setCurrentStatus));
            }
        }

        public class WhenConsolidatingTextures : TheFlattenTexturesService
        {
            [Test]
            public void GivenNoDirectories_DoesNotCopyAnyDirectories()
            {
                _flattenTexturesService.ConsolidateTextures(default, Enumerable.Empty<string>());
                _directory.Verify(d => d.CopyDirectory(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<bool>()), Times.Never);
            }

            [Test]
            public void GivenDirectoryThatDoesNotExist_DoesNotCopyDirectory()
            {
                var textureDirectory = "working-dir/tf2";
                _directory.Setup(d => d.Exists(textureDirectory)).Returns(false);
                
                _flattenTexturesService.ConsolidateTextures(default, new List<string> { textureDirectory });
                
                _directory.Verify(d => d.CopyDirectory(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<bool>()), Times.Never);
            }

            [Test]
            public void GivenDirectoryThatExists_CopiesDirectory()
            {
                var consolidateTo = "save-them-here";
                var textureDirectory = "working-dir/tf2";
                _directory.Setup(d => d.Exists(textureDirectory)).Returns(true);

                _flattenTexturesService.ConsolidateTextures(consolidateTo, new List<string> { textureDirectory });

                _directory.Verify(d => d.CopyDirectory(textureDirectory, consolidateTo, true, true), Times.Once);
            }
        }

        public class WhenCreatingOutput : TheFlattenTexturesService
        {
            [Test]
            public async Task GivenTypeForSingleVPK_CreatesSingleVPK()
            {
                var tf2Directory = "some/dir/Team Fortress 2";
                var flattenedMaterialsDirectory = "another/dir/flat-textures";
                var generatedVpks = new List<string> { "new.vpk" };
                _vpkGenerator.Setup(g => g.Generate(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<bool>())).ReturnsAsync(generatedVpks);
                
                var output = await _flattenTexturesService.CreateOutput(tf2Directory, flattenedMaterialsDirectory, FlattenOutputType.SingleVPK);
                
                _vpkGenerator.Verify(g => g.Generate(tf2Directory, flattenedMaterialsDirectory, false));
                output.ShouldDeepEqual(generatedVpks);
            }

            [Test]
            public async Task GivenTypeForMultiChunkVPK_CreatesMultiChunkVPK()
            {
                var tf2Directory = "some/dir/Team Fortress 2";
                var flattenedMaterialsDirectory = "another/dir/flat-textures";
                var generatedVpks = new List<string> { "new_000.vpk", "new_dir.vpk" };
                _vpkGenerator.Setup(g => g.Generate(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<bool>())).ReturnsAsync(generatedVpks);

                var output = await _flattenTexturesService.CreateOutput(tf2Directory, flattenedMaterialsDirectory, FlattenOutputType.MultiChunkVPK);
                
                _vpkGenerator.Verify(g => g.Generate(tf2Directory, flattenedMaterialsDirectory, true));
                output.ShouldDeepEqual(generatedVpks);
            }

            [Test]
            public async Task GivenTypeForTextureFiles_LeavesFilesAsIs()
            {
                var tf2Directory = "some/dir/Team Fortress 2";
                var flattenedMaterialsDirectory = "another/dir/flat-textures";

                var output = await _flattenTexturesService.CreateOutput(tf2Directory, flattenedMaterialsDirectory, FlattenOutputType.TextureFiles);
                Assert.Multiple(() =>
                {
                    Assert.That(output.Count(), Is.EqualTo(1));
                    Assert.That(output.First(), Is.EqualTo(Path.Combine(flattenedMaterialsDirectory, "materials")));
                });
            }
        }

        public class WhenCleaningUp : TheFlattenTexturesService
        {
            [Test]
            public void GivenTypeForSingleVPK_DeletesDirectory()
            {
                var directory = "flat-textures";
                _flattenTexturesService.CleanUp(directory, FlattenOutputType.SingleVPK);
                _directory.Verify(d => d.Delete(directory, true));
            }

            [Test]
            public void GivenTypeForMultiChunkVPK_DeletesDirectory()
            {
                var directory = "flat-textures";
                _flattenTexturesService.CleanUp(directory, FlattenOutputType.MultiChunkVPK);
                _directory.Verify(d => d.Delete(directory, true));
            }

            [Test]
            public void GivenTypeForTextureFiles_DoesNotDeletesDirectory()
            {
                var directory = "flat-textures";
                _flattenTexturesService.CleanUp(directory, FlattenOutputType.TextureFiles);
                _directory.Verify(d => d.Delete(It.IsAny<string>(), It.IsAny<bool>()), Times.Never);
            }
        }
    }
}
