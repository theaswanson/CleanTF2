using CleanTF2.CLI.Commands;
using CleanTF2.Core;
using CleanTF2.Core.Utilities;
using CleanTF2.Core.Valve;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Spectre.Console.Cli;

namespace CleanTF2.CLI
{
    internal class Program
    {
        static IServiceCollection _services;

        static async Task Main(string[] args)
        {
            var host = Host.CreateDefaultBuilder(args)
                .ConfigureServices(RegisterServices)
                .Build();

            BuildApp().Run(args);

            await host.StopAsync();
        }

        private static CommandApp BuildApp()
        {
            var app = new CommandApp(new TypeRegistrar(_services));
            app.Configure(config =>
            {
                config.AddCommand<FlattenCommand>("flatten");
            });
            return app;
        }

        static void RegisterServices(HostBuilderContext hostContext, IServiceCollection services)
        {
            _services = services;
            _services
                .AddTransient<IProcessRunner, ProcessRunner>()
                .AddTransient<IFile, FileWrapper>()
                .AddTransient<IDirectory, DirectoryWrapper>()
                .AddTransient<IHLExtract, HLExtract>()
                .AddTransient<IVTFCmd, VTFCmd>()
                .AddTransient<IImageManipulator, ImageManipulator>()
                .AddTransient<IFlatTextureGenerator, FlatTextureGenerator>()
                .AddTransient<FlattenCommand>();
        }
    }
}
