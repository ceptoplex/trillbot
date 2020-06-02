using System.Threading;
using System.Threading.Tasks;

namespace TrillBot.Discord
{
    public interface IDiscordModule
    {
        Task InitializeAsync(CancellationToken cancellationToken = default);
    }
}