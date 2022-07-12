using CleanTF2.CLI.Commands;
using CleanTF2.Core;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Spectre.Console.Cli;

namespace CleanTF2.CLI
{
    internal class Program
    {
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
            var app = new CommandApp();
            app.Configure(config =>
            {
                config.AddCommand<FlattenCommand>("flatten");
            });
            return app;
        }

        static void RegisterServices(HostBuilderContext _, IServiceCollection services)
        {
            services
                .AddSingleton<IProcessRunner, ProcessRunner>()
                .AddSingleton<IFile, FileWrapper>()
                .AddSingleton<IDirectory, DirectoryWrapper>()
                .AddSingleton<IHLExtract, HLExtract>()
                .AddSingleton<FlatTextureGenerator>();
        }
    }
}
