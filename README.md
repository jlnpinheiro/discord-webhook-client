# Discord Webhook Client
[![AppVayor - Build status](https://ci.appveyor.com/api/projects/status/hqdwvdowbmifop4f?svg=true)](https://ci.appveyor.com/project/jlnpinheiro/discord-webhook-client) [![NuGet](https://img.shields.io/nuget/dt/discord-webhook-client.svg?style=flat-square)](https://www.nuget.org/packages/discord-webhook-client) [![NuGet](https://img.shields.io/nuget/v/discord-webhook-client.svg?style=flat-square)](https://www.nuget.org/packages/discord-webhook-client)

Simple .NET client to send **Discord** (https://discordapp.com/) messages using a webhook.

Reference documentation for creating this client: [Discord Webhooks Guide](https://birdie0.github.io/discord-webhooks-guide/) and [Discord Developer Portal](https://discordapp.com/developers/docs/intro).

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
- Create an instance of the class ```DiscordWebhookClient``` with your Discord webhook URL. For more information about how to create a Discord webhook, visit https://support.discordapp.com/hc/en-us/articles/228383668-Intro-to-Webhooks
- Create your ```DiscordMessage``` with all parameters of your message.
- Send the message!

```csharp
// Add reference to namespace JNogueira.Discord.Webhook.Client
using JNogueira.Discord.Webhook.Client;

// Create an instance of the class DiscordWebhookClient with your Discord webhook URL.
var client = new DiscordWebhookClient("Your Discord webhook URL here!");

// Create your DiscordMessage with all parameters of your message.
var message = new DiscordMessage(
    "Discord Webhook Client sent this message! " + DiscordEmoji.Grinning,
    username: "Username",
    avatarUrl: "https://avatars3.githubusercontent.com/u/24236993?s=460&v=4",
    tts: false,
    embeds: new[]
    {
        new DiscordMessageEmbed(
            "Embed title " + DiscordEmoji.Thumbsup,
            color: 0,
            author: new DiscordMessageEmbedAuthor("Embed 1 author name"),
            url: "https://github.com/jlnpinheiro/discord-webhook-client/",
            description: "This is a embed description.",
            fields: new[]
            {
                new DiscordMessageEmbedField("Field 1 name", "Field 1 value"),
                new DiscordMessageEmbedField("Field 2 name", "Field 2 value")
            },
            thumbnail: new DiscordMessageEmbedThumbnail("https://avatars3.githubusercontent.com/u/24236993?s=460&v=4"),
            image: new DiscordMessageEmbedImage("https://avatars3.githubusercontent.com/u/24236993?s=460&v=4"),
            footer: new DiscordMessageEmbedFooter("This is a embed footer text", "https://avatars3.githubusercontent.com/u/24236993?s=460&v=4")
        )
    }
);

// Send the message!
await client.SendToDiscord(message);
```
Look at to your Discord channel...your message is there!

![A messagem in your Discord channel](../assets/screenshot1.png?raw=true)

## Sending a message with file attachments

```csharp
var message = new DiscordMessage(
    ...
);

var file1 = new DiscordFile("test1.txt", Encoding.UTF8.GetBytes("This is the first file."));

var file2 = new DiscordFile("test2.txt", Encoding.UTF8.GetBytes("This is the secound file."));

await client.SendToDiscord(message, new[] { file1, file2 });
```

## Send message as file attachment on exception
When an exception is throwing on sending the message, a new message can be sent to Discord with attachments containing message data and exception information.

```csharp
var message = new DiscordMessage(
    content: new string('0', 7000), // <-- The content length limit is 2000 characters => Exception is throwing.
    ...
);

await _client.SendToDiscord(message, true);
```

![A messagem in your Discord channel](../assets/screenshot2.png?raw=true)
