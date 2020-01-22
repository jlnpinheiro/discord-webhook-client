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
- Add reference to namespace ```JNogueira.Discord.Webhook.Client```
- Create an instance of the class ```DiscordWebhookClient``` with your Discord webhook URL.
- Create your ```DiscordMessage``` with all parameters of your message.
- Send the message!

```csharp
// Add reference to namespace JNogueira.Discord.Webhook.Client
using JNogueira.Discord.Webhook.Client;

// Create an instance of the class DiscordWebhookClient with your Discord webhook URL.
var client = new DiscordWebhookClient("Your Discord webhook URL herer!");

// Create your DiscordMessage with all parameters of your message.
var message = new DiscordMessage(
    "This is my message " + DiscordEmoji.Grinning,
    username: "Username",
    avatarUrl: "https://i.imgur.com/oBPXx0D.png",
    tts: false,
    embeds: new[]
    {
        new DiscordMessageEmbed(
            "Discord Webhook Client sent his message! " + DiscordEmoji.Heart,
            color: 0,
            author: new DiscordMessageEmbedAuthor("Embed 1 author name"),
            url: "https://www.google.com",
            description: "This is a embed description.",
            fields: new[]
            {
                new DiscordMessageEmbedField("Field 1 name", "Field 1 value"),
                new DiscordMessageEmbedField("Field 2 name", "Field 2 value")
            },
            thumbnail: new DiscordMessageEmbedThumbnail("https://i.imgur.com/oBPXx0D.png"),
            image: new DiscordMessageEmbedImage("https://i.imgur.com/oBPXx0D.png"),
            footer: new DiscordMessageEmbedFooter("This is a embed footer text " + DiscordEmoji.Pray, "https://i.imgur.com/oBPXx0D.png")
        )
    }
);

// Send the message!
await _client.SendToDiscord(message);
```
