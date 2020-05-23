using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace TrillBot.WebApi
{
    public sealed class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        private static IHostBuilder CreateHostBuilder(string[] args)
        {
            return Host
                .CreateDefaultBuilder(args)
                .ConfigureAppConfiguration((context, appBuilder) =>
                {
                    appBuilder
                        .SetBasePath(context.HostingEnvironment.ContentRootPath)
                        .AddYamlFile("appsettings.yml", false, true)
                        .AddYamlFile($"appsettings.{context.HostingEnvironment.EnvironmentName}.yml", true, true)
                        .AddEnvironmentVariables("TrillBot:")
                        .Build();
                })
                .ConfigureWebHostDefaults(webBuilder => { webBuilder.UseStartup<Startup>(); })
                .ConfigureLogging((context, loggingBuilder) =>
                {
                    loggingBuilder.AddConfiguration(context.Configuration.GetSection("Logging"));
                });
        }
    }
}