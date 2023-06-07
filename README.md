[![RedditBots.Web Deploy](https://github.com/Marcel0024/RedditBots/actions/workflows/angular-client_redditbots.yml/badge.svg?branch=main&event=push)](https://github.com/Marcel0024/RedditBots/actions/workflows/angular-client_redditbots.yml) ![RedditBots.Console (via Azure Pipelines)](https://dev.azure.com/marcelcroes24/RedditBots/_apis/build/status/docker%20build?label=RedditBots.Console%20Deploy) 

<hr/>

# RedditBots
This repo contains two projects. RedditBots.Console and RedditBots.Web.

### RedditBots.Console
RedditBots.Console is an .NET 6 Console application runs 3 Reddit bots and 1 Discord bot. It is build in Azure Pipeline into a docker image and then deployed to a Raspberry Pi.
All logs are sent via http to RedditBots.Web via a custom logger.

### RedditBots.Web
RedditBots.Web receives all logs from the console and streams them to the browser and stores them in CosmosDb.
RedditBots.Web is an Angular 13 web application which is served by an .NET 6 application. All incoming logs from Reddit.Console are streamed to the clients via SignalR. It is deployed via Github Actions to Azure.
The resources on Azure are created with Bicep templates.


You can view ReddiBots.Web (live stream of the logs) here: https://redditbots.azurewebsites.net/
