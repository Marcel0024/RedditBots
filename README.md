[![RedditBots.Web Deploy](https://github.com/Marcel0024/RedditBots/actions/workflows/angular-client_redditbots.yml/badge.svg?branch=master&event=push)](https://github.com/Marcel0024/RedditBots/actions/workflows/angular-client_redditbots.yml) ![RedditBots.Console (via Azure Pipelines)](https://dev.azure.com/marcelcroes24/RedditBots/_apis/build/status/docker%20build?label=RedditBots.Console%20Deploy) 

<hr/>

# RedditBots
This repo contains two projects. RedditBots.Console and RedditBots.Web.

### RedditBots.Console
RedditBots.Console is an .NET 5 Console application that is build in docker and deployed to a Raspberry Pi via Azure Pipelines.
It runs 3 Reddit bots and 1 Discord bot. All logs are sent via http to RedditBots.Web via a custom logger.

### RedditBots.Web
RedditBots.Web is an Angular 11 web application which is served by an .NET 5 application. It receives all incoming logs from Reddit.Console and pushes them to the clients via SignalR. It is deployed via Github Actions to Azure.
The resources on Azure are created with Bicep templates


You can view ReddiBots.Web (live stream of the logs) here: https://redditbots.azurewebsites.net/
