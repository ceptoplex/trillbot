# TrillBot for Discord

[![GitHub Actions: Build & Deploy](https://github.com/ceptoplex/trillbot-discord/workflows/Build%20&%20Deploy/badge.svg)](https://github.com/ceptoplex/trillbot-discord/actions)
[![Discord](https://discordapp.com/api/guilds/314010693084905494/widget.png)](https://discord.gg/trilluxe)
[![Twitter: Follow @ceptoplex](https://img.shields.io/twitter/follow/ceptoplex?style=social)](https://twitter.com/ceptoplex)

This is a Discord bot that is running on the official [TrilluXe Community](https://discord.gg/trilluxe) server.
The author of this bot is [ceptoplex](https://twitter.com/ceptoplex) who is also managing the server.
The owner of the server is [TrilluXe](https://twitter.com/trilluxe).
Found a bug or got an idea idea for a new feature? [Community contributions](https://github.com/ceptoplex/trillbot-discord/blob/master/CONTRIBUTING.md) are possible as well.

## 1. Features

- __Elastic Voice Channels:__ Voice channels are replicated automatically as soon as users join them.
  This ensures that there is always one empty voice channel available of each existing type
  (where different types means different names or different user limits).

## 2. Configuration

All configuration values that do not have any defaults
and are required to run the application can be found in `appsettings.yml`.
They can be set or overwritten using one or both of the following two mechanisms.

### Environment-specific Configuration File

Besides `appsettings.yml` and `appsettings.Production.yml`,
there can be additional configuration files, named using the scheme `appsettings.{Environment}.yml`.
These additional configuration files are excluded from version control by default.
The decision which of these additional files is loaded is based on the environment
which can be set using the system environment variable `NETCORE_ENVIRONMENT`.
It defaults to `Development` if it is not set explicitly.

Therefore, it is e.g. possible to use a file named `appsettings.Development.yml` during development
where you can put in secrets that are not relevant for production and that should
not be added to version control.

### System Environment Variables

Additionally, system environment variables can be used to overwrite specific settings.
They have to use the prefix `TRILLBOT__`.
The translation of configuration keys to environment variable names is documented [here](https://docs.microsoft.com/en-us/aspnet/core/fundamentals/configuration/).
An example would be the system environment variable `TRILLBOT__DISCORD__TOKEN` that sets
or overwrites the value of the configuration item with the key `Discord:Token`.

## 3. Build & Run

To build and run the application in a Linux production environment, use the following commands:

    ~$ dotnet restore
    ~$ dotnet build
    ~$ set NETCORE_ENVIRONMENT=Production
    ~$ set TRILLBOT__DISCORD__TOKEN={token}
    ~$ dotnet run --project=TrillBot.Discord.App

 Or else, you can also use the provided `Dockerfile`:
 
    ~$ docker build -t trillbot-discord .
    ~$ docker run -d -e NETCORE_ENVIRONMENT=Production -e TRILLBOT__DISCORD__TOKEN={token} trillbot-discord

## 4. Invitation & Discord Permissions

To invite the bot to a server, the following URL can be used (after filling in the value for `client_id`):

    https://discordapp.com/api/oauth2/authorize?client_id={client_id}&permissions=8&scope=bot

It includes the OAuth2 scope `bot` and the `Administrator` permission that is required by the bot.