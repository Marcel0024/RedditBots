
FROM mcr.microsoft.com/dotnet/runtime:7.0.1-bullseye-slim-arm32v7 AS base
COPY /linux-arm app/
WORKDIR /app
ENTRYPOINT ["dotnet", "RedditBots.Console.dll"]