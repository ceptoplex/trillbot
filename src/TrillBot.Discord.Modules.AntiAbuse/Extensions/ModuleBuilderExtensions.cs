using Microsoft.Extensions.DependencyInjection;
using TrillBot.Discord.Extensions;
using TrillBot.Discord.Modules.AntiAbuse.Confusables;

namespace TrillBot.Discord.Modules.AntiAbuse.Extensions
{
    public static class ModuleBuilderExtensions
    {
        public static ServiceCollectionExtensions.ModuleBuilder AddAntiAbuse(
            this ServiceCollectionExtensions.ModuleBuilder moduleBuilder)
        {
            moduleBuilder.Services
                .AddSingleton(new ConfusablesDetection(new ConfusablesCache()))
                .AddSingleton<BotImpersonationMonitoring>()
                .AddSingleton<JoinMonitoring>();

            return moduleBuilder
                .AddModule<DiscordAntiAbuseModule>();
        }
    }
}