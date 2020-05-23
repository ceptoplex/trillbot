namespace TrillBot.Twitch.Api.Options
{
    public sealed class TwitchApiOptions
    {
        public const string Key = "Twitch:Api";

        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
    }
}