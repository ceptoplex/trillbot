using TrillBot.Common.Events;

namespace TrillBot.Twitch.Webhooks.EventArgs
{
    public sealed class TwitchStreamOfflineEventArgs : CancelableEventArgs
    {
        public TwitchStreamOfflineEventArgs(string userId)
        {
            UserId = userId;
        }

        public string UserId { get; }
    }
}