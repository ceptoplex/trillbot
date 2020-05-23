using System.Threading;
using System.Threading.Tasks;
using RestEase;
using TrillBot.Twitch.Api.Entities.Authentication;

namespace TrillBot.Twitch.Api.Api
{
    public interface ITwitchAuthenticationApi
    {
        [Query("client_id")]
        string ClientId { get; set; }

        [Query("client_secret")]
        string ClientSecret { get; set; }

        [Post("token?grant_type=client_credentials")]
        Task<TwitchAppAccessTokenDto> GetAppAccessTokenAsync(
            CancellationToken cancellationToken = default);
    }
}