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

            [Description("1: .vpk files (default), 2: material files")]
            [CommandOption("--output-type")]
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

            var outputType = FlattenOutputType.MultiChunkVPK;
            if (settings.OutputType == 2)
            {
                outputType = FlattenOutputType.TextureFiles;
            }
            await _flattenService.Flatten(settings.TF2Directory, settings.Upscale, outputType);

            return 0;
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
