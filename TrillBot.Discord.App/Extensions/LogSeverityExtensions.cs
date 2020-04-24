using Discord;
using Microsoft.Extensions.Logging;

namespace TrillBot.Discord.App.Extensions
{
    public static class LogSeverityExtensions
    {
        public static LogLevel ToLogLevel(this LogSeverity logSeverity)
        {
            // "Log severity 0 to 5" is equal to "log level 5 to 0".
            return (LogLevel) ((LogSeverity) 5 - logSeverity);
        }
    }
}