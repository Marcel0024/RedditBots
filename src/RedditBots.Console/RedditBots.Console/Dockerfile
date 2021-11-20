
FROM mcr.microsoft.com/dotnet/runtime:6.0.0-bullseye-slim-arm32v7 AS base
COPY bin/Release/net6.0/linux-arm app/
WORKDIR /app
ENTRYPOINT ["dotnet", "RedditBots.Console.dll"]