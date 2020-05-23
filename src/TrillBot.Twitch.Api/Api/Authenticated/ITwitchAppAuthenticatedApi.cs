namespace TrillBot.Twitch.Api.Api.Authenticated
{
    public interface ITwitchAppAuthenticatedApi : ITwitchAuthenticatedApi
    {
        public const string AuthenticationScheme = "Bearer";
    }
}