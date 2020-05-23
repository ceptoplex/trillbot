using System;
using System.Threading;
using System.Threading.Tasks;
using RestEase;

namespace TrillBot.WebSub.Api
{
    internal interface IHubApi
    {
        [Post("/?hub.mode=subscribe")]
        Task SubscribeAsync(
            [Query("hub.callback")] Uri callback,
            [Query("hub.topic")] Uri topic,
            [Query("hub.lease_seconds", QuerySerializationMethod.Serialized)]
            TimeSpan lease,
            [Query("hub.secret")] string secret,
            CancellationToken cancellationToken = default);

        [Post("/?hub.mode=unsubscribe")]
        Task UnsubscribeAsync(
            [Query("hub.callback")] Uri callback,
            [Query("hub.topic")] Uri topic,
            CancellationToken cancellationToken = default);
    }
}