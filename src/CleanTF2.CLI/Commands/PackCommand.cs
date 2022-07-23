using System.ComponentModel;
using CleanTF2.Core;
using CleanTF2.Core.Utilities;
using Spectre.Console;
using Spectre.Console.Cli;

namespace CleanTF2.CLI.Commands
{
    internal sealed class PackCommand : AsyncCommand<PackCommand.Settings>
    {
        private readonly IVPKGenerator _vpkGenerator;
        private readonly IDirectory _directory;
        private readonly IInterop _interop;

        public PackCommand(IVPKGenerator vpkGenerator, IDirectory directory, IInterop interop)
        {
            _vpkGenerator = vpkGenerator;
            _directory = directory;
            _interop = interop;
        }

        public sealed class Settings : CommandSettings
        {
            [Description("The directory to pack into a .vpk.")]
            [CommandArgument(0, "<directoryToPack>")]
            public string DirectoryToPack { get; set; }
            [Description("Full path to the Team Fortress 2 directory (the directory that contains the hl2 executable).")]
            [CommandOption("-d")]
            public string TF2Directory { get; set; }

            [Description("Splits the vpk into multiple files. Useful when packing several files.")]
            [CommandOption("-m|--multi-chunk")]
            public bool MultiChunk { get; set; }
        }

        public override async Task<int> ExecuteAsync(CommandContext context, Settings settings)
        {
            ValidateOS();
            ValidateTF2Directory(settings);
            ValidateDirectoryToPack(settings);

            AnsiConsole.WriteLine("Packing files...");
            var vpks = await _vpkGenerator.Generate(settings.TF2Directory, settings.DirectoryToPack, settings.MultiChunk);

            if (!vpks.Any())
            {
                AnsiConsole.WriteLine("No .vpks were generated.");
            }
            else
            {
                AnsiConsole.WriteLine("Done!");
                foreach (var vpk in vpks)
                {
                    AnsiConsole.WriteLine(vpk);
                }
            }

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
                settings.TF2Directory = DefaultTF2Directory();
            }

            if (!_directory.Exists(settings.TF2Directory))
            {
                throw new DirectoryNotFoundException("The given TF2 directory could not be found. Check the path and try again.");
            }
        }

        private void ValidateDirectoryToPack(Settings settings)
        {
            if (!_directory.Exists(settings.DirectoryToPack))
            {
                throw new DirectoryNotFoundException("The given directory to pack could not be found. Check the path and try again.");
            }
        }

        private string DefaultTF2Directory() => @"C:\Program Files (x86)\Steam\steamapps\common\Team Fortress 2";
    }
}
