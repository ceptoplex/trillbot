# Build

FROM mcr.microsoft.com/dotnet/core/sdk:3.1 AS build-env
WORKDIR /app

COPY *.sln ./
COPY ./TrillBot.Discord.App/*.csproj ./TrillBot.Discord.App/
COPY ./TrillBot.Discord.Modules/*.csproj ./TrillBot.Discord.Modules/
COPY ./TrillBot.Discord.Modules.AntiAbuse/*.csproj ./TrillBot.Discord.Modules.AntiAbuse/
COPY ./TrillBot.Discord.Modules.ElasticVoiceChannels/*.csproj ./TrillBot.Discord.Modules.ElasticVoiceChannels/
COPY ./TrillBot.Discord.Modules.Ping/*.csproj ./TrillBot.Discord.Modules.Ping/
RUN dotnet restore

COPY . ./
RUN dotnet publish --configuration Release --output out

# Run

FROM mcr.microsoft.com/dotnet/core/aspnet:3.1
WORKDIR /app

COPY --from=build-env /app/out .
ENTRYPOINT ["dotnet", "TrillBot.Discord.App.dll"]