using System.ComponentModel;
using CleanTF2.Core;
using CleanTF2.Core.Utilities;
using Spectre.Console.Cli;

namespace CleanTF2.CLI.Commands
{
    internal sealed class FlattenCommand : AsyncCommand<FlattenCommand.Settings>
    {
        private readonly IFlattenService _flattenService;
        private readonly IDirectory _directory;
        private readonly IInterop _interop;
        private readonly Defaults _defaults;

        public FlattenCommand(
            IFlattenService flattenService,
            IDirectory directory,
            IInterop interop,
            Defaults defaults)
        {
            _flattenService = flattenService;
            _directory = directory;
            _interop = interop;
            _defaults = defaults;
        }

        public sealed class Settings : CommandSettings
        {
            [Description("Full path to the Team Fortress 2 directory (the directory that contains the hl2 executable).")]
            [CommandOption("-d")]
            public string TF2Directory { get; set; }

            [Description("1: flatten TF2 and HL2 textures (default), 2: only flatten TF2 textures, 3: only flatten HL2 textures")]
            [CommandOption("-m|--mode")]
            [DefaultValue(1)]
            public int Mode { get; set; }

            [Description("1: single .vpk file (default), 2: split .vpk files, 3: material files")]
            [CommandOption("-o|--output-type")]
            [DefaultValue(1)]
            public int OutputType { get; set; }

            [Description("Upscales flattened textures to 1024x1024. Helps address compatibility problems with sv_pure. (warning: takes more file size and processing time)")]
            [CommandOption("--upscale")]
            public bool Upscale { get; set; }
        }

        public override async Task<int> ExecuteAsync(CommandContext context, Settings settings)
        {
            ValidateOS();
            ValidateTF2Directory(settings);

            var outputType = GetOutputType(settings.OutputType);
            var mode = GetMode(settings.Mode);
            await _flattenService.Flatten(settings.TF2Directory, settings.Upscale, outputType, mode);

            return 0;
        }

        private static FlattenMode GetMode(int mode)
        {
            return mode switch
            {
                1 => FlattenMode.All,
                2 => FlattenMode.TF2,
                3 => FlattenMode.HL2,
                _ => throw new ArgumentOutOfRangeException(nameof(mode), "Invalid argument."),
            };
        }

        private static FlattenOutputType GetOutputType(int outputType)
        {
            return outputType switch
            {
                1 => FlattenOutputType.SingleVPK,
                2 => FlattenOutputType.MultiChunkVPK,
                3 => FlattenOutputType.TextureFiles,
                _ => throw new ArgumentOutOfRangeException(nameof(outputType), "Invalid argument."),
            };
        }

        private void ValidateOS()
        {
            if (!_interop.IsOSPlatform(System.Runtime.InteropServices.OSPlatform.Windows))
            {
                throw new Exception("Windows is the only supported OS at this time.");
            }
        }

        private void ValidateTF2Directory(Settings settings)
        {
            if (string.IsNullOrWhiteSpace(settings.TF2Directory))
            {
                settings.TF2Directory = _defaults.TF2Directory();
            }

            if (!_directory.Exists(settings.TF2Directory))
            {
                throw new DirectoryNotFoundException("The given TF2 directory could not be found. Check the path and try again.");
            }
        }
    }
}
