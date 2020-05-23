using System;

namespace TrillBot.WebSub.Options
{
    public sealed class WebSubOptions
    {
        public const string Key = "WebSub";

        public Uri CallbackPublicUrl { get; set; }
        public TimeSpan? Lease { get; set; }
        public TimeSpan? LeaseRenewalBuffer { get; set; }
    }
}