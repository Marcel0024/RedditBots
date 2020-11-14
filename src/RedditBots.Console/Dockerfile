#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/runtime:5.0.0-buster-slim-arm32v7 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:5.0-buster AS build
WORKDIR /src
COPY ["src/RedditBots.Console/RedditBots.Console.csproj", "src/RedditBots.Console/"]
COPY ["src/Bots/RedditBots.Framework/RedditBots.Framework.csproj", "src/Bots/RedditBots.Framework/"]
COPY ["src/Libraries/RedditBots.Libraries.Logging/RedditBots.Libraries.Logging.csproj", "src/Libraries/RedditBots.Libraries.Logging/"]
COPY ["src/Bots/RedditBots.CheerfulBot/RedditBots.CheerfulBot.csproj", "src/Bots/RedditBots.CheerfulBot/"]
COPY ["src/Bots/RedditBots.HanzeMemesBot/RedditBots.HanzeMemesBot.csproj", "src/Bots/RedditBots.HanzeMemesBot/"]
COPY ["src/Bots/RedditBots.PeriodicallyBot/RedditBots.PeriodicallyBot.csproj", "src/Bots/RedditBots.PeriodicallyBot/"]
COPY ["src/Bots/RedditBots.PapiamentoBot/RedditBots.PapiamentoBot.csproj", "src/Bots/RedditBots.PapiamentoBot/"]
RUN dotnet restore "src/RedditBots.Console/RedditBots.Console.csproj"
COPY . .
WORKDIR "/src/src/RedditBots.Console"
RUN dotnet build "RedditBots.Console.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "RedditBots.Console.csproj" -c Release -r linux-arm -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "RedditBots.Console.dll"]