using System;
using TrillBot.Common.Events;

namespace TrillBot.WebSub.EventArgs
{
    public sealed class WebSubContentAvailableEventArgs<TContent> : CancelableEventArgs
    {
        public WebSubContentAvailableEventArgs(Uri topic, TContent content)
        {
            Topic = topic;
            Content = content;
        }

        public Uri Topic { get; }
        public TContent Content { get; }
    }
}