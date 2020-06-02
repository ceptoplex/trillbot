# Build

FROM mcr.microsoft.com/dotnet/core/sdk:3.1 AS build-env
WORKDIR /app

COPY ./*.sln ./
COPY ./src/TrillBot.Common/*.csproj ./src/TrillBot.Common/
COPY ./src/TrillBot.Discord/*.csproj ./src/TrillBot.Discord/
COPY ./src/TrillBot.Discord.Modules.AntiAbuse/*.csproj ./src/TrillBot.Discord.Modules.AntiAbuse/
COPY ./src/TrillBot.Discord.Modules.ElasticVoiceChannels/*.csproj ./src/TrillBot.Discord.Modules.ElasticVoiceChannels/
COPY ./src/TrillBot.Discord.Modules.Ping/*.csproj ./src/TrillBot.Discord.Modules.Ping/
COPY ./src/TrillBot.Twitch.Api/*.csproj ./src/TrillBot.Twitch.Api/
COPY ./src/TrillBot.Twitch.Webhooks/*.csproj ./src/TrillBot.Twitch.Webhooks/
COPY ./src/TrillBot.WebApi/*.csproj ./src/TrillBot.WebApi/
COPY ./src/TrillBot.WebSub/*.csproj ./src/TrillBot.WebSub/
COPY ./src/TrillBot.WebSub.Abstractions/*.csproj ./src/TrillBot.WebSub.Abstractions/
RUN dotnet restore

COPY . ./
RUN dotnet publish --configuration Release --output out

# Run

FROM mcr.microsoft.com/dotnet/core/aspnet:3.1
WORKDIR /app

COPY --from=build-env /app/out .

EXPOSE 5000
HEALTHCHECK --interval=60s --timeout=5s --retries=3 \
    CMD curl -f "http://localhost:5000/health" || exit 1
ENTRYPOINT ["dotnet", "TrillBot.WebApi.dll"]