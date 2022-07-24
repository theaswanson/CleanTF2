using CleanTF2.Core.Utilities;

namespace CleanTF2.Core
{
    public class Defaults
    {
        private readonly IFile _file;

        public Defaults(IFile file)
        {
            _file = file;
        }

        public string TF2Directory() => @"C:\Program Files (x86)\Steam\steamapps\common\Team Fortress 2";

        public string TF2TexturesPackage(string? tf2Directory = null)
        {
            tf2Directory ??= TF2Directory();
            return Path.Combine(tf2Directory, "tf", "tf2_textures_dir.vpk");
        }

        public string HL2TexturesPackage(string? tf2Directory = null)
        {
            tf2Directory ??= TF2Directory();
            return Path.Combine(tf2Directory, "hl2", "hl2_textures_dir.vpk");
        }

        public async Task<IEnumerable<string>> TF2MaterialList()
        {
            return await _file.ReadAllLinesAsync("flat.txt");
        }

        public async Task<IEnumerable<string>> HL2MaterialList()
        {
            return await _file.ReadAllLinesAsync("flat_hl2.txt");
        }
    }
}
