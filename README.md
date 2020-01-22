# Discord Webhook Client
[![AppVayor - Build status](https://ci.appveyor.com/api/projects/status/hqdwvdowbmifop4f?svg=true)](https://ci.appveyor.com/project/jlnpinheiro/discord-webhook-client) [![NuGet](https://img.shields.io/nuget/dt/discord-webhook-client.svg?style=flat-square)](https://www.nuget.org/packages/discord-webhook-client) [![NuGet](https://img.shields.io/nuget/v/discord-webhook-client.svg?style=flat-square)](https://www.nuget.org/packages/discord-webhook-client)

Simple .NET client to send **Discord** (https://discordapp.com/) messages using a webhook.

Reference documentation for creating this client: [Discord Webhooks Client](https://birdie0.github.io/discord-webhooks-guide/) and [Discord Developer Portal](https://discordapp.com/developers/docs/intro).

## Target
.NET Standard 2.0+

For more information about suported versions visit https://docs.microsoft.com/pt-br/dotnet/standard/net-standard

## Installation

### NuGet
```
Install-Package discord-webhook-client
```
### .NET CLI
```
dotnet add package discord-webhook-client
```

## Sending a message...
Sending a message is very simple!
- Add ```using JNogueira.Discord.Webhook.Client;```
- Create an instance of the class ```DiscordWebhookClient``` with your Discord webhook URL.
- Create your ```DiscordMessage``` with all parameters of your message.
- Send message!
