using System.Globalization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using TrillBot.Common.Extensions;
using TrillBot.Discord.Extensions;
using TrillBot.Discord.Modules.AntiAbuse.Extensions;
using TrillBot.Discord.Modules.ElasticVoiceChannels.Extensions;
using TrillBot.Discord.Modules.ElasticVoiceChannels.Options;
using TrillBot.Discord.Modules.Ping.Extensions;
using TrillBot.Discord.Options;
using TrillBot.Twitch.Api.Extensions;
using TrillBot.Twitch.Api.Options;
using TrillBot.Twitch.Webhooks.Extensions;
using TrillBot.WebApi.Health;
using TrillBot.WebApi.Options;
using TrillBot.WebApi.Services;

namespace TrillBot.WebApi
{
    internal sealed class Startup
    {
        private const string TwitchWebhooksCallbackControllerRouteTemplate = "websub/twitch";

        private readonly IConfiguration _configuration;

        public Startup(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddLocalization(options => options.ResourcesPath = "Resources");
            var culture = new CultureInfo("de-DE");
            CultureInfo.DefaultThreadCurrentCulture = culture;
            CultureInfo.DefaultThreadCurrentUICulture = culture;

            services
                .AddTwitchApi(_configuration.GetSection(TwitchApiOptions.Key))
                .AddTwitchWebhooks(
                    TwitchWebhooksCallbackControllerRouteTemplate,
                    options => { },
                    options => options.CallbackPublicUrl = _configuration
                        .GetSection(WebApiOptions.Key)
                        .Get<WebApiOptions>()
                        .PublicUrl
                        .Append(TwitchWebhooksCallbackControllerRouteTemplate));

            services
                .AddDiscord(
                    builder => builder
                        .AddAntiAbuse()
                        .AddElasticVoiceChannels(
                            _configuration.GetSection(DiscordElasticVoiceChannelsModuleOptions.Key))
                        .AddPing(),
                    _configuration.GetSection(DiscordOptions.Key))
                .AddHostedService<DiscordBotService>();

            services
                .AddControllers()
                .AddNewtonsoftJson();
            services
                .AddHealthChecks()
                .AddCheck<DiscordBotHealthCheck>(nameof(DiscordBotHealthCheck));
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            // This call has to be as early as possible due to the request body buffering.
            app.UseTwitchWebhooks();

            if (env.IsDevelopment())
                app.UseDeveloperExceptionPage();

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapHealthChecks("health");
            });
        }
    }
}