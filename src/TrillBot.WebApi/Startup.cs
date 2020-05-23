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
using TrillBot.Discord.Modules.Ping.Extensions;
using TrillBot.Discord.Options;
using TrillBot.WebApi.Services;

namespace TrillBot.WebApi
{
    internal sealed class Startup
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

            services
                .AddDiscord(builder =>
                    {
                        builder.AddAntiAbuse();
                        builder.AddElasticVoiceChannels(_configuration.GetSection(ElasticVoiceChannelsOptions.Key));
                        builder.AddPing();
                    },
                    _configuration.GetSection(DiscordOptions.Key))
                .AddHostedService<DiscordBotService>();
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