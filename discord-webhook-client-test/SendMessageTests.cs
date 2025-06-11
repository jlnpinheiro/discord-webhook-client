using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Text;

namespace JNogueira.Discord.WebhookClient.Test;

public class Tests
{
    protected ServiceProvider _serviceProvider;
    protected IServiceCollection _serviceCollection;
    protected DiscordWebhookClient _discordClient;

    [SetUp]
    public void SetupTest()
    {
        var configuration = new ConfigurationBuilder()
            .AddJsonFile("testSettings.json")
            .Build();

        _serviceCollection = new ServiceCollection()
            .AddSingleton(configuration)
            .AddDiscordWebhookClient(configuration);

        _serviceProvider = _serviceCollection.BuildServiceProvider();

        _discordClient = _serviceProvider.GetService<DiscordWebhookClient>();
    }

    [Test]
    public async Task Should_Send_Hello_World_Message()
    {
        var message = new DiscordMessage("Hello World!");

        var sendMessageOk = await _discordClient.SendToDiscordAsync(message);

        Assert.That(sendMessageOk);
    }

    [Test]
    public async Task Should_Send_Embed_Without_Content()
    {
        var message = new DiscordMessage(
           username: "Username",
           avatarUrl: "https://avatars3.githubusercontent.com/u/24236993?s=460&v=4",
           tts: false,
           embeds:
           [
                new DiscordMessageEmbed(
                    "Embed title " + DiscordEmoji.Thumbsup,
                    color: 0,
                    author: new DiscordMessageEmbedAuthor("Embed 1 author name"),
                    url: "https://github.com/jlnpinheiro/discord-webhook-client/",
                    description: "This is a embed description.",
                    fields:
                    [
                        new DiscordMessageEmbedField("Field 1 name", "Field 1 value"),
                        new DiscordMessageEmbedField("Field 2 name", "Field 2 value")
                    ],
                    thumbnail: new DiscordMessageEmbedThumbnail("https://avatars3.githubusercontent.com/u/24236993?s=460&v=4"),
                    image: new DiscordMessageEmbedImage("https://avatars3.githubusercontent.com/u/24236993?s=460&v=4"),
                    footer: new DiscordMessageEmbedFooter("This is a embed footer text", "https://avatars3.githubusercontent.com/u/24236993?s=460&v=4")
                )
           ]
        );

        var sendMessageOk = await _discordClient.SendToDiscordAsync(message);

        Assert.That(sendMessageOk);
    }

    [Test]
    public async Task Should_Send_Message_With_Embeds()
    {
        var message = new DiscordMessage(
            "Discord Webhook Client sent this message! " + DiscordEmoji.Grinning,
            username: "Username",
            avatarUrl: "https://avatars3.githubusercontent.com/u/24236993?s=460&v=4",
            tts: false,
            embeds:
            [
                new DiscordMessageEmbed(
                    "Embed title " + DiscordEmoji.Thumbsup,
                    color: 0,
                    author: new DiscordMessageEmbedAuthor("Embed 1 author name"),
                    url: "https://github.com/jlnpinheiro/discord-webhook-client/",
                    description: "This is a embed description.",
                    fields:
                    [
                        new DiscordMessageEmbedField("Field 1 name", "Field 1 value"),
                        new DiscordMessageEmbedField("Field 2 name", "Field 2 value")
                    ],
                    thumbnail: new DiscordMessageEmbedThumbnail("https://avatars3.githubusercontent.com/u/24236993?s=460&v=4"),
                    image: new DiscordMessageEmbedImage("https://avatars3.githubusercontent.com/u/24236993?s=460&v=4"),
                    footer: new DiscordMessageEmbedFooter("This is a embed footer text", "https://avatars3.githubusercontent.com/u/24236993?s=460&v=4")
                )
            ]
        );

        var sendMessageOk = await _discordClient.SendToDiscordAsync(message);

        Assert.That(sendMessageOk);
    }

    [Test]
    public async Task Should_Send_Message_With_Files()
    {
        var message = new DiscordMessage(
            "Test Should_Send_Message_With_Files " + DiscordEmoji.Grinning,
            username: "Username",
            avatarUrl: "https://avatars3.githubusercontent.com/u/24236993?s=460&v=4",
            tts: false,
            embeds:
            [
                new DiscordMessageEmbed(
                    "Title embed " + DiscordEmoji.Heart,
                    color: 0,
                    author: new DiscordMessageEmbedAuthor("Embed 1 author name"),
                    url: "https://github.com/jlnpinheiro/discord-webhook-client/",
                    description: "This is a embed description.",
                    fields:
                    [
                        new DiscordMessageEmbedField("Field 1 name", "Field 1 value"),
                        new DiscordMessageEmbedField("Field 2 name", "Field 2 value")
                    ],
                    thumbnail: new DiscordMessageEmbedThumbnail("https://avatars3.githubusercontent.com/u/24236993?s=460&v=4"),
                    image: new DiscordMessageEmbedImage("https://avatars3.githubusercontent.com/u/24236993?s=460&v=4"),
                    footer: new DiscordMessageEmbedFooter("This is a embed footer text " + DiscordEmoji.Pray, "https://avatars3.githubusercontent.com/u/24236993?s=460&v=4")
                )
            ]
        );

        var file1 = new DiscordFile("test1.txt", Encoding.UTF8.GetBytes("This is the first file."));

        var file2 = new DiscordFile("test2.txt", Encoding.UTF8.GetBytes("This is the secound file."));

        var sendMessageOk = await _discordClient.SendToDiscordAsync(message, [file1, file2]);

        Assert.That(sendMessageOk);
    }

    [Test]
    public async Task Should_Not_Send_Message_With_Content_Length_Limit_Exceeded()
    {
        try
        {
            var sendMessageOk = await _discordClient.SendToDiscordAsync(new DiscordMessage(new string('0', 2001)));

            Assert.Fail("Message successfully sent.");
        }
        catch (DiscordWebhookClientException ex)
        {
            Assert.That(ex.Message, Does.Contain("The \"content\" element length limit is 2000 characters"));
        }
    }

    [Test]
    public async Task Should_Not_Send_Message_With_Embed_Title_Length_Limit_Exceeded()
    {
        try
        {
            var sendMessageOk = await _discordClient.SendToDiscordAsync(new DiscordMessage("Content", embeds: [new(new string('0', 257))]));

            Assert.Fail("Message successfully sent.");
        }
        catch (DiscordWebhookClientException ex)
        {
            Assert.That(ex.Message, Does.Contain("The embed \"title\" length limit is 256 characters"));
        }
    }

    [Test]
    public async Task Should_Not_Send_Message_With_Embed_Description_Length_Limit_Exceeded()
    {
        try
        {
            var sendMessageOk = await _discordClient.SendToDiscordAsync(new DiscordMessage("Content", embeds: [new("Title", description: new string('0', 2049))]));

            Assert.Fail("Message successfully sent.");
        }
        catch (DiscordWebhookClientException ex)
        {
            Assert.That(ex.Message, Does.Contain("The embed \"description\" length limit is 2048 characters"));
        }
    }

    [Test]
    public async Task Should_Not_Send_Message_With_Embed_Author_Name_Empty()
    {
        try
        {
            var sendMessageOk = await _discordClient.SendToDiscordAsync(new DiscordMessage("Content", embeds: [new("Title", author: new(string.Empty))]));

            Assert.Fail("Message successfully sent.");
        }
        catch (DiscordWebhookClientException ex)
        {
            Assert.That(ex.Message, Does.Contain("The embed author \"name\" cannot be null or empty."));
        }
    }

    [Test]
    public async Task Should_Not_Send_Message_With_Embed_Author_Name_Length_Limit_Exceeded()
    {
        try
        {
            var sendMessageOk = await _discordClient.SendToDiscordAsync(new DiscordMessage("Content", embeds: [new("Title", author: new(new string('0', 257)))]));

            Assert.Fail("Message successfully sent.");
        }
        catch (DiscordWebhookClientException ex)
        {
            Assert.That(ex.Message, Does.Contain("The embed author \"name\" length limit is 256 characters"));
        }
    }

    [Test]
    public async Task Should_Not_Send_Message_With_Embed_Fields_Size_Limit_Exceeded()
    {
        var fields = new List<DiscordMessageEmbedField>();

        for (var i = 1; i < 27; i++)
        {
            fields.Add(new DiscordMessageEmbedField("Field name", "Field value"));
        }

        try
        {
            var sendMessageOk = await _discordClient.SendToDiscordAsync(new DiscordMessage("Content", embeds: [new("Title", fields: fields)]));

            Assert.Fail("Message successfully sent.");
        }
        catch (DiscordWebhookClientException ex)
        {
            Assert.That(ex.Message, Does.Contain("The embed \"fields\" collection size limit is 25 elements."));
        }
    }

    [Test]
    public async Task Should_Not_Send_Message_With_Embed_Field_Name_Empty()
    {
        try
        {
            var sendMessageOk = await _discordClient.SendToDiscordAsync(new DiscordMessage("Content", embeds: [new("Title", fields: [new(string.Empty)])]));

            Assert.Fail("Message successfully sent.");
        }
        catch (DiscordWebhookClientException ex)
        {
            Assert.That(ex.Message, Does.Contain("The embed field \"name\" cannot be null or empty."));
        }
    }

    [Test]
    public async Task Should_Not_Send_Message_With_Embed_Field_Name_Length_Limit_Exceeded()
    {
        try
        {
            var sendMessageOk = await _discordClient.SendToDiscordAsync(new DiscordMessage("Content", embeds: [new("Title", fields: [new(new string('0', 257))])]));

            Assert.Fail("Message successfully sent.");
        }
        catch (DiscordWebhookClientException ex)
        {
            Assert.That(ex.Message, Does.Contain("The embed field \"name\" length limit is 256 characters"));
        }
    }

    [Test]
    public async Task Should_Not_Send_Message_With_Embed_Field_Value_Length_Limit_Exceeded()
    {
        try
        {
            var sendMessageOk = await _discordClient.SendToDiscordAsync(new DiscordMessage("Content", embeds: [new("Title", fields: [new("Field name", new string('0', 1025))])]));

            Assert.Fail("Message successfully sent.");
        }
        catch (DiscordWebhookClientException ex)
        {
            Assert.That(ex.Message, Does.Contain("The embed field \"value\" length limit is 1024 characters"));
        }
    }

    [Test]
    public async Task Should_Not_Send_Message_With_Embed_Thumbnail_Url_Empty()
    {
        try
        {
            var sendMessageOk = await _discordClient.SendToDiscordAsync(new DiscordMessage("Content", embeds: [new("Title", thumbnail: new(string.Empty))]));

            Assert.Fail("Message successfully sent.");
        }
        catch (DiscordWebhookClientException ex)
        {
            Assert.That(ex.Message, Does.Contain("The embed thumbnail \"url\" cannot be null or empty."));
        }
    }

    [Test]
    public async Task Should_Not_Send_Message_With_Embed_Image_Url_Empty()
    {
        try
        {
            var sendMessageOk = await _discordClient.SendToDiscordAsync(new DiscordMessage("Content", embeds: [new("Title", image: new(string.Empty))]));

            Assert.Fail("Message successfully sent.");
        }
        catch (DiscordWebhookClientException ex)
        {
            Assert.That(ex.Message, Does.Contain("The embed image \"url\" cannot be null or empty."));
        }
    }

    [Test]
    public async Task Should_Not_Send_Message_With_Embed_Footer_Text_Empty()
    {
        try
        {
            var sendMessageOk = await _discordClient.SendToDiscordAsync(new DiscordMessage("Content", embeds: [new("Title", footer: new(string.Empty))]));

            Assert.Fail("Message successfully sent.");
        }
        catch (DiscordWebhookClientException ex)
        {
            Assert.That(ex.Message, Does.Contain("The embed footer \"text\" cannot be null or empty."));
        }
    }

    [Test]
    public async Task Should_Not_Send_Message_With_Embed_Footer_Text_Length_Limit_Exceeded()
    {
        try
        {
            var sendMessageOk = await _discordClient.SendToDiscordAsync(new DiscordMessage("Content", embeds: [new("Title", footer: new(new string('0', 2049)))]));

            Assert.Fail("Message successfully sent.");
        }
        catch (DiscordWebhookClientException ex)
        {
            Assert.That(ex.Message, Does.Contain("The embed footer \"text\" length limit is 2048 characters"));
        }
    }

    [Test]
    public async Task Should_Send_Messages_With_No_Too_Many_Requests_Error()
    {
        for (int i = 1; i <= 30; i++)
        {
            await _discordClient.SendToDiscordAsync(new DiscordMessage("test " + i));
        }

        Assert.Pass();
    }

    [Test]
    public async Task Should_Send_Message_As_Attachment_On_Error()
    {
        var message = new DiscordMessage(
            "Should_Send_Message_As_Attachment_On_Error" + new string('0', 7000),
            username: "Username",
            avatarUrl: "https://avatars3.githubusercontent.com/u/24236993?s=460&v=4",
            tts: false,
            embeds:
            [
                new DiscordMessageEmbed(
                    "Embed title " + DiscordEmoji.Thumbsup,
                    color: 0,
                    author: new DiscordMessageEmbedAuthor("Embed 1 author name"),
                    url: "https://github.com/jlnpinheiro/discord-webhook-client/",
                    description: "This is a embed description.",
                    fields:
                    [
                        new DiscordMessageEmbedField("Field 1 name", "Field 1 value"),
                        new DiscordMessageEmbedField("Field 2 name", "Field 2 value")
                    ],
                    thumbnail: new DiscordMessageEmbedThumbnail("https://avatars3.githubusercontent.com/u/24236993?s=460&v=4"),
                    image: new DiscordMessageEmbedImage("https://avatars3.githubusercontent.com/u/24236993?s=460&v=4"),
                    footer: new DiscordMessageEmbedFooter("This is a embed footer text", "https://avatars3.githubusercontent.com/u/24236993?s=460&v=4")
                ),
                new DiscordMessageEmbed(
                    "Embed title " + DiscordEmoji.Thumbsup,
                    color: 0,
                    author: new DiscordMessageEmbedAuthor("Embed 1 author name"),
                    url: "https://github.com/jlnpinheiro/discord-webhook-client/",
                    description: "This is a embed description.",
                    fields:
                    [
                        new DiscordMessageEmbedField("Field 1 name", "Field 1 value"),
                        new DiscordMessageEmbedField("Field 2 name", "Field 2 value")
                    ],
                    thumbnail: new DiscordMessageEmbedThumbnail("https://avatars3.githubusercontent.com/u/24236993?s=460&v=4"),
                    image: new DiscordMessageEmbedImage("https://avatars3.githubusercontent.com/u/24236993?s=460&v=4"),
                    footer: new DiscordMessageEmbedFooter("This is a embed footer text", "https://avatars3.githubusercontent.com/u/24236993?s=460&v=4")
                )
            ]
        );

        var sendMessageOk = await _discordClient.SendToDiscordAsync(message, sendMessageAsFileAttachmentOnError: true);

        Assert.That(!sendMessageOk);
    }
}