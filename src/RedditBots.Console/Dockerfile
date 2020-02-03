#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/core/runtime:3.1-buster-slim AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
WORKDIR /src
COPY ["src/RedditBots.Console/RedditBots.Console.csproj", "src/RedditBots.Console/"]
COPY ["src/Libraries/RedditBots.Libraries.Logging/RedditBots.Libraries.Logging.csproj", "src/Libraries/RedditBots.Libraries.Logging/"]
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