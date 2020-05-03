# Build

FROM mcr.microsoft.com/dotnet/core/sdk:3.1 AS build-env
WORKDIR /app

COPY ./*.sln ./
COPY ./src/TrillBot.Discord/*.csproj ./src/TrillBot.Discord/
COPY ./src/TrillBot.Discord.Modules/*.csproj ./src/TrillBot.Discord.Modules/
COPY ./src/TrillBot.Discord.Modules.AntiAbuse/*.csproj ./src/TrillBot.Discord.Modules.AntiAbuse/
COPY ./src/TrillBot.Discord.Modules.ElasticVoiceChannels/*.csproj ./src/TrillBot.Discord.Modules.ElasticVoiceChannels/
COPY ./src/TrillBot.Discord.Modules.Ping/*.csproj ./src/TrillBot.Discord.Modules.Ping/
COPY ./src/TrillBot.WebApi/*.csproj ./src/TrillBot.WebApi/
RUN dotnet restore

COPY . ./
RUN dotnet publish --configuration Release --output out

# Run

FROM mcr.microsoft.com/dotnet/core/aspnet:3.1
WORKDIR /app

COPY --from=build-env /app/out .
ENTRYPOINT ["dotnet", "TrillBot.WebApi.dll"]