using System;

namespace TrillBot.WebApi.Options
{
    internal sealed class WebApiOptions
    {
        public const string Key = "WebApi";

        public Uri PublicUrl { get; set; }
    }
}