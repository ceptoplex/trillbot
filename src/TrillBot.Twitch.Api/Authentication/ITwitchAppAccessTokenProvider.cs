using System.Threading;
using System.Threading.Tasks;

namespace TrillBot.Twitch.Api.Authentication
{
    public interface ITwitchAppAccessTokenProvider
    {
        Task<string> GetAccessTokenAsync(CancellationToken cancellationToken = default);
    }
}