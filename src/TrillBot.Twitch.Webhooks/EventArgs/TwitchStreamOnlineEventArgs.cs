using TrillBot.Common.Events;

namespace TrillBot.Twitch.Webhooks.EventArgs
{
    public sealed class TwitchStreamOnlineEventArgs : CancelableEventArgs
    {
        public TwitchStreamOnlineEventArgs(string userId, string title)
        {
            UserId = userId;
            Title = title;
        }

        public string UserId { get; }
        public string Title { get; }
    }
}