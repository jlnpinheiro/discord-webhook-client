using JNogueira.Discord.Webhook.Client;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace discord_webhook_client_test
{
    [TestClass]
    public class SendMessageTests
    {
        private DiscordWebhookClient _client;

        public SendMessageTests()
        {
            var config = new ConfigurationBuilder()
               .AddJsonFile("testSettings.json")
               .Build();

            _client = new DiscordWebhookClient(config["UrlWebhook"]);
        }

        [TestMethod]
        public async Task Should_Send_Hello_World_Message()
        {
            var message = new DiscordMessage("Hello World!");

            await _client.SendToDiscord(message);

            Assert.IsTrue(true);
        }

        [TestMethod]
        public async Task Should_Send_Embed_Without_Content()
        {
            var message = new DiscordMessage(
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

            await _client.SendToDiscord(message);
            Assert.IsTrue(true);
        }

        [TestMethod]
        public async Task Should_Send_Message_With_Embeds()
        {
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

            await _client.SendToDiscord(message);

            Assert.IsTrue(true);
        }

        [TestMethod]
        public async Task Should_Send_Message_With_Files()
        {
            var message = new DiscordMessage(
                "Test Should_Send_Message_With_Files " + DiscordEmoji.Grinning,
                username: "Username",
                avatarUrl: "https://avatars3.githubusercontent.com/u/24236993?s=460&v=4",
                tts: false,
                embeds: new[]
                {
                    new DiscordMessageEmbed(
                        "Title embed " + DiscordEmoji.Heart,
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
                        footer: new DiscordMessageEmbedFooter("This is a embed footer text " + DiscordEmoji.Pray, "https://avatars3.githubusercontent.com/u/24236993?s=460&v=4")
                    )
                }
            );

            var file1 = new DiscordFile("test1.txt", Encoding.UTF8.GetBytes("This is the first file."));

            var file2 = new DiscordFile("test2.txt", Encoding.UTF8.GetBytes("This is the secound file."));

            await _client.SendToDiscord(message, new[] { file1, file2 });

            Assert.IsTrue(true);
        }

        [TestMethod]
        public async Task Should_Not_Send_Message_With_Content_Length_Limit_Exceeded()
        {
            try
            {
                await _client.SendToDiscord(new DiscordMessage(new string('0', 2001)));

                Assert.Fail("Message successfully sent.");
            }
            catch (DiscordWebhookClientException ex)
            {
                Assert.IsTrue(ex.Message.Contains("The \"content\" element length limit is 2000 characters"));
            }
        }

        [TestMethod]
        public async Task Should_Not_Send_Message_With_Embed_Title_Length_Limit_Exceeded()
        {
            try
            {
                await _client.SendToDiscord(new DiscordMessage("Content", embeds: new DiscordMessageEmbed[] { new DiscordMessageEmbed(new string('0', 257)) }));

                Assert.Fail("Message successfully sent.");
            }
            catch (DiscordWebhookClientException ex)
            {
                Assert.IsTrue(ex.Message.Contains("The embed \"title\" length limit is 256 characters"));
            }
        }

        [TestMethod]
        public async Task Should_Not_Send_Message_With_Embed_Description_Length_Limit_Exceeded()
        {
            try
            {
                await _client.SendToDiscord(new DiscordMessage("Content", embeds: new DiscordMessageEmbed[] { new DiscordMessageEmbed("Title", description: new string('0', 2049)) }));

                Assert.Fail("Message successfully sent.");
            }
            catch (DiscordWebhookClientException ex)
            {
                Assert.IsTrue(ex.Message.Contains("The embed \"description\" length limit is 2048 characters"));
            }
        }

        [TestMethod]
        public async Task Should_Not_Send_Message_With_Embed_Author_Name_Empty()
        {
            try
            {
                await _client.SendToDiscord(new DiscordMessage("Content", embeds: new DiscordMessageEmbed[] { new DiscordMessageEmbed("Title", author: new DiscordMessageEmbedAuthor(string.Empty)) }));

                Assert.Fail("Message successfully sent.");
            }
            catch (DiscordWebhookClientException ex)
            {
                Assert.IsTrue(ex.Message.Contains("The embed author \"name\" cannot be null or empty."));
            }
        }

        [TestMethod]
        public async Task Should_Not_Send_Message_With_Embed_Author_Name_Length_Limit_Exceeded()
        {
            try
            {
                await _client.SendToDiscord(new DiscordMessage("Content", embeds: new DiscordMessageEmbed[] { new DiscordMessageEmbed("Title", author: new DiscordMessageEmbedAuthor(new string('0', 257))) }));

                Assert.Fail("Message successfully sent.");
            }
            catch (DiscordWebhookClientException ex)
            {
                Assert.IsTrue(ex.Message.Contains("The embed author \"name\" length limit is 256 characters"));
            }
        }

        [TestMethod]
        public async Task Should_Not_Send_Message_With_Embed_Fields_Size_Limit_Exceeded()
        {
            var fields = new List<DiscordMessageEmbedField>();

            for (var i = 1; i < 27; i++)
            {
                fields.Add(new DiscordMessageEmbedField("Field name", "Field value"));
            }

            try
            {
                await _client.SendToDiscord(new DiscordMessage("Content", embeds: new DiscordMessageEmbed[] { new DiscordMessageEmbed("Title", fields: fields) }));

                Assert.Fail("Message successfully sent.");
            }
            catch (DiscordWebhookClientException ex)
            {
                Assert.IsTrue(ex.Message.Contains("The embed \"fields\" collection size limit is 25 elements."));
            }
        }

        [TestMethod]
        public async Task Should_Not_Send_Message_With_Embed_Field_Name_Empty()
        {
            try
            {
                await _client.SendToDiscord(new DiscordMessage("Content", embeds: new DiscordMessageEmbed[] { new DiscordMessageEmbed("Title", fields: new DiscordMessageEmbedField[] { new DiscordMessageEmbedField(string.Empty) }) }));

                Assert.Fail("Message successfully sent.");
            }
            catch (DiscordWebhookClientException ex)
            {
                Assert.IsTrue(ex.Message.Contains("The embed field \"name\" cannot be null or empty."));
            }
        }

        [TestMethod]
        public async Task Should_Not_Send_Message_With_Embed_Field_Name_Length_Limit_Exceeded()
        {
            try
            {
                await _client.SendToDiscord(new DiscordMessage("Content", embeds: new DiscordMessageEmbed[] { new DiscordMessageEmbed("Title", fields: new DiscordMessageEmbedField[] { new DiscordMessageEmbedField(new string('0', 257)) }) }));

                Assert.Fail("Message successfully sent.");
            }
            catch (DiscordWebhookClientException ex)
            {
                Assert.IsTrue(ex.Message.Contains("The embed field \"name\" length limit is 256 characters"));
            }
        }

        [TestMethod]
        public async Task Should_Not_Send_Message_With_Embed_Field_Value_Length_Limit_Exceeded()
        {
            try
            {
                await _client.SendToDiscord(new DiscordMessage("Content", embeds: new DiscordMessageEmbed[] { new DiscordMessageEmbed("Title", fields: new DiscordMessageEmbedField[] { new DiscordMessageEmbedField("Field name", new string('0', 1025)) }) }));

                Assert.Fail("Message successfully sent.");
            }
            catch (DiscordWebhookClientException ex)
            {
                Assert.IsTrue(ex.Message.Contains("The embed field \"value\" length limit is 1024 characters"));
            }
        }

        [TestMethod]
        public async Task Should_Not_Send_Message_With_Embed_Thumbnail_Url_Empty()
        {
            try
            {
                await _client.SendToDiscord(new DiscordMessage("Content", embeds: new DiscordMessageEmbed[] { new DiscordMessageEmbed("Title", thumbnail: new DiscordMessageEmbedThumbnail(string.Empty)) }));

                Assert.Fail("Message successfully sent.");
            }
            catch (DiscordWebhookClientException ex)
            {
                Assert.IsTrue(ex.Message.Contains("The embed thumbnail \"url\" cannot be null or empty."));
            }
        }

        [TestMethod]
        public async Task Should_Not_Send_Message_With_Embed_Image_Url_Empty()
        {
            try
            {
                await _client.SendToDiscord(new DiscordMessage("Content", embeds: new DiscordMessageEmbed[] { new DiscordMessageEmbed("Title", image: new DiscordMessageEmbedImage(string.Empty)) }));

                Assert.Fail("Message successfully sent.");
            }
            catch (DiscordWebhookClientException ex)
            {
                Assert.IsTrue(ex.Message.Contains("The embed image \"url\" cannot be null or empty."));
            }
        }

        [TestMethod]
        public async Task Should_Not_Send_Message_With_Embed_Footer_Text_Empty()
        {
            try
            {
                await _client.SendToDiscord(new DiscordMessage("Content", embeds: new DiscordMessageEmbed[] { new DiscordMessageEmbed("Title", footer: new DiscordMessageEmbedFooter(string.Empty)) }));

                Assert.Fail("Message successfully sent.");
            }
            catch (DiscordWebhookClientException ex)
            {
                Assert.IsTrue(ex.Message.Contains("The embed footer \"text\" cannot be null or empty."));
            }
        }

        [TestMethod]
        public async Task Should_Not_Send_Message_With_Embed_Footer_Text_Length_Limit_Exceeded()
        {
            try
            {
                await _client.SendToDiscord(new DiscordMessage("Content", embeds: new DiscordMessageEmbed[] { new DiscordMessageEmbed("Title", footer: new DiscordMessageEmbedFooter(new string('0', 2049))) }));

                Assert.Fail("Message successfully sent.");
            }
            catch (DiscordWebhookClientException ex)
            {
                Assert.IsTrue(ex.Message.Contains("The embed footer \"text\" length limit is 2048 characters"));
            }
        }

        [TestMethod]
        public async Task Should_Send_Messages_With_No_Too_Many_Requests_Error()
        {
            for (int i = 1; i <= 10; i++)
            {
                await _client.SendToDiscord(new DiscordMessage("test " + i));
            }

            Assert.IsTrue(1 == 1);
        }

        [TestMethod]
        public async Task Should_Send_Message_As_Attachment_On_Error()
        {
            var message = new DiscordMessage(
                "Should_Send_Message_As_Attachment_On_Error" + new string('0', 7000),
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
                    ),
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

            await _client.SendToDiscord(message, true);

            Assert.IsTrue(true);
        }
    }
}
