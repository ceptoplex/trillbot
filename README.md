# TrillBot

[![GitHub Actions: Build & Deploy](https://github.com/ceptoplex/trillbot/workflows/Build%20&%20Deploy/badge.svg)](https://github.com/ceptoplex/trillbot/actions)

This bot is used to manage services for and provided by [TrilluXe](https://twitter.com/trilluxe).
The author and maintainer is [ceptoplex](https://twitter.com/ceptoplex).
Found a bug or got an idea for a new feature? [Community contributions](https://github.com/ceptoplex/trillbot/blob/master/CONTRIBUTING.md) are possible.

[![Twitter: Follow @trilluxe](https://img.shields.io/twitter/follow/trilluxe?style=social)](https://twitter.com/trilluxe)
[![Twitter: Follow @ceptoplex](https://img.shields.io/twitter/follow/ceptoplex?style=social)](https://twitter.com/ceptoplex)

## 1. Services & Features

### Discord

[![Discord](https://discordapp.com/api/guilds/314010693084905494/widget.png)](https://discord.gg/trilluxe)

This bot is present on the [TrilluXe Community](https://discord.gg/trilluxe) Discord server.
It offers the following functionality:

- __Anti-Abuse:__ Abuse is detected and mitigated:
    - Users that try to impersonate the bot are kicked from the server.
- __Elastic Voice Channels:__ Voice channels are replicated automatically as soon as users join them.
  This ensures that there is always one empty voice channel available of each existing type
  (where different types means different names or different user limits).

## 2. Configuration

All configuration values that do not have any defaults
and are required to run the application can be found in `src/TrillBot.WebApi/appsettings.yml`.
They can be set or overwritten using one or both of the following two mechanisms.

### Environment-specific Configuration File

Besides the `appsettings.yml` and the `appsettings.Production.yml` file,
there can be additional configuration files in this directory,
named using the scheme `appsettings.{Environment}.yml`.
These additional configuration files are excluded from version control by default.
The decision which of these additional files is loaded is based on the environment
which can be set using the system environment variable `ASPNETCORE_ENVIRONMENT`.

Therefore, it is e.g. possible to use a file named `appsettings.Development.yml` during development
where you can put in secrets that are different from those used in production and that should
not be added to version control.

### System Environment Variables

Additionally, system environment variables can be used to overwrite specific settings.
They have to start with the prefix `TRILLBOT__`.
The translation of configuration keys to environment variable names is documented [here](https://docs.microsoft.com/en-us/aspnet/core/fundamentals/configuration/).
An example would be the system environment variable `TRILLBOT__DISCORD__TOKEN` that sets
or overwrites the value of the configuration item with the key `Discord:Token`.

## 3. Build & Run

To build and run the application in a Linux production environment, use the following commands:

    ~$ dotnet restore
    ~$ dotnet build
    ~$ set ASPNETCORE_ENVIRONMENT=Production
    ~$ set TRILLBOT__DISCORD__TOKEN={token}
    ~$ set TRILLBOT__TWITCH__API__CLIENTSECRET={secret}
    ~$ dotnet run --project=src/TrillBot.WebApi

 Or else, you can also use the provided `Dockerfile`, e.g., like this:
 
    ~$ docker build -t trillbot .
    ~$ docker run \
        -d \
        -e ASPNETCORE_ENVIRONMENT=Production \
        -e TRILLBOT__DISCORD__TOKEN={token} \
        -e TRILLBOT__TWITCH__API__CLIENTSECRET={secret} \
        trillbot

Here, secrets and tokens have been passed via environment variables because they may not be stored
in an unprotected configuration file.

### Discord: Invitation & Permissions

To invite the bot to a Discord server, the following URL should be used (after filling in the value for `client_id`):

    https://discordapp.com/api/oauth2/authorize?client_id={client_id}&permissions=8&scope=bot

It includes the OAuth2 scope `bot` and the `Administrator` permission that is required by the bot.