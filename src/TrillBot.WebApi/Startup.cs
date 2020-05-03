using System.Globalization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using TrillBot.Discord.Extensions;
using TrillBot.Discord.Modules.AntiAbuse.Extensions;
using TrillBot.Discord.Modules.ElasticVoiceChannels.Extensions;
using TrillBot.Discord.Modules.ElasticVoiceChannels.Options;
using TrillBot.Discord.Modules.Options;
using TrillBot.Discord.Modules.Ping.Extensions;
using TrillBot.Discord.Options;
using TrillBot.WebApi.Services;

namespace TrillBot.WebApi
{
    internal class Startup
    {
        private readonly IConfiguration _configuration;

        public Startup(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();

            services.AddLocalization(options => options.ResourcesPath = "Resources");
            var culture = new CultureInfo("de-DE");
            CultureInfo.DefaultThreadCurrentCulture = culture;
            CultureInfo.DefaultThreadCurrentUICulture = culture;

            var discordSection = _configuration
                .GetSection(DiscordOptions.Name);
            var elasticVoiceChannelsSection = discordSection
                .GetSection(IModuleOptions.Name)
                .GetSection(ElasticVoiceChannelsOptions.Name);
            services
                .AddDiscord(builder =>
                    {
                        builder.AddAntiAbuse();
                        builder.AddElasticVoiceChannels(elasticVoiceChannelsSection);
                        builder.AddPing();
                    },
                    discordSection)
                .AddHostedService<DiscordService>();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment()) app.UseDeveloperExceptionPage();

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
        }
    }
}