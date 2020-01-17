using JNogueira.Discord.Webhook;
using Microsoft.Extensions.Configuration;
using NUnit.Framework;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace discord_webhook_tests
{
    public class Tests
    {
        private DiscordWebhookClient _proxy;
        
        [SetUp]
        public void Setup()
        {
            var config = new ConfigurationBuilder()
               .AddJsonFile("testSettings.json")
               .Build();

            _proxy = new DiscordWebhookClient(config["UrlWebhook"]);
        }

        [Test]
        public async Task Must_Send_Hello_World_Message()
        {
            var message = new DiscordMessage("Test Must_Send_Hello_World_Message");

            await _proxy.SendToDiscord(message);

            Assert.Pass();
        }

        [Test]
        public async Task Must_Send_Message_With_Embeds()
        {
            var message = new DiscordMessage(
                "Test Must_Send_Message_With_Embeds",
                username: "Username",
                avatarUrl: "https://i.imgur.com/oBPXx0D.png",
                tts: false,
                embeds: new[]
                {
                    new DiscordMessageEmbed(
                        "Title embed",
                        color: 0,
                        author: new DiscordMessageEmbedAuthor("Embed 1 author name"),
                        url: "https://www.google.com",
                        description: "This is a embed description.",
                        fields: new[]
                        {
                            new DiscordMessageEmbedField("Field 1 name", "Field 1 value"),
                            new DiscordMessageEmbedField("Field 2 name", "Field 2 value")
                        },
                        footer: new DiscordMessageEmbedFooter("This is a embed footer text")
                    )
                }
            );
            
            await _proxy.SendToDiscord(message);

            Assert.Pass();
        }
        
        [Test]
        public async Task Must_Send_Exception_Details_Message()
        {
            try
            {
                var i = 0;

                var x = 5 / i; //<--- force an exception here!
            }
            catch (System.Exception ex)
            {
                var embedFields = new List<DiscordMessageEmbedField>()
                {
                    new DiscordMessageEmbedField("Sender", "Name of the sender responsible for the exception."),
                    new DiscordMessageEmbedField("Environment", "Development"),
                };

                if (!string.IsNullOrEmpty(ex.Source))
                {
                    embedFields.Add(new DiscordMessageEmbedField("Source", ex.Source));
                }

                embedFields.Add(new DiscordMessageEmbedField("Base exception", ex.GetBaseException()?.Message));

                var message = new DiscordMessage("Test Must_Send_Exception_Details_Message")
                {
                    Embeds = new[]
                    {
                        new DiscordMessageEmbed(
                            ex.GetBaseException()?.Message,
                            color: 16711680,
                            description: ex.StackTrace,
                            fields: embedFields
                        )
                    }
                };

                await _proxy.SendToDiscord(message);

                Assert.Pass();
            }
        }

        [Test]
        public async Task Must_Not_Send_Message_With_Content_Empty()
        {
            try
            {
                await _proxy.SendToDiscord(new DiscordMessage(string.Empty));

                Assert.Fail("Message successfully sent.");
            }
            catch (DiscordWebhookClientException ex)
            {
                Assert.IsTrue(ex.Message.Contains("The \"content\" cannot be null or empty."));
            }
        }

        [Test]
        public async Task Must_Not_Send_Message_With_Content_Length_Limit_Exceeded()
        {
            try
            {
                await _proxy.SendToDiscord(new DiscordMessage(new string('0', 2001)));

                Assert.Fail("Message successfully sent.");
            }
            catch (DiscordWebhookClientException ex)
            {
                Assert.IsTrue(ex.Message.Contains("The \"content\" length limit is 2000 characters"));
            }
        }

        [Test]
        public async Task Must_Not_Send_Message_With_Embeds_Null_Element()
        {
            try
            {
                await _proxy.SendToDiscord(new DiscordMessage("Content", embeds: new DiscordMessageEmbed[] { null }));

                Assert.Fail("Message successfully sent.");
            }
            catch (DiscordWebhookClientException ex)
            {
                Assert.IsTrue(ex.Message.Contains("The \"embeds\" cannot have null elements in the array."));
            }
        }

        [Test]
        public async Task Must_Not_Send_Message_With_Embed_Title_Empty()
        {
            try
            {
                await _proxy.SendToDiscord(new DiscordMessage("Content", embeds: new DiscordMessageEmbed[] { new DiscordMessageEmbed(string.Empty) }));

                Assert.Fail("Message successfully sent.");
            }
            catch (DiscordWebhookClientException ex)
            {
                Assert.IsTrue(ex.Message.Contains("The embed \"title\" cannot be null or empty."));
            }
        }

        [Test]
        public async Task Must_Not_Send_Message_With_Embed_Title_Length_Limit_Exceeded()
        {
            try
            {
                await _proxy.SendToDiscord(new DiscordMessage("Content", embeds: new DiscordMessageEmbed[] { new DiscordMessageEmbed(new string('0', 257)) }));

                Assert.Fail("Message successfully sent.");
            }
            catch (DiscordWebhookClientException ex)
            {
                Assert.IsTrue(ex.Message.Contains("The embed \"title\" length limit is 256 characters"));
            }
        }

        [Test]
        public async Task Must_Not_Send_Message_With_Embed_Description_Length_Limit_Exceeded()
        {
            try
            {
                await _proxy.SendToDiscord(new DiscordMessage("Content", embeds: new DiscordMessageEmbed[] { new DiscordMessageEmbed("Title", description: new string('0', 2049)) }));

                Assert.Fail("Message successfully sent.");
            }
            catch (DiscordWebhookClientException ex)
            {
                Assert.IsTrue(ex.Message.Contains("The embed \"description\" length limit is 2048 characters"));
            }
        }

        [Test]
        public async Task Must_Not_Send_Message_With_Embed_Author_Name_Empty()
        {
            try
            {
                await _proxy.SendToDiscord(new DiscordMessage("Content", embeds: new DiscordMessageEmbed[] { new DiscordMessageEmbed("Title", author: new DiscordMessageEmbedAuthor(string.Empty)) }));

                Assert.Fail("Message successfully sent.");
            }
            catch (DiscordWebhookClientException ex)
            {
                Assert.IsTrue(ex.Message.Contains("The embed author \"name\" cannot be null or empty."));
            }
        }

        [Test]
        public async Task Must_Not_Send_Message_With_Embed_Author_Name_Length_Limit_Exceeded()
        {
            try
            {
                await _proxy.SendToDiscord(new DiscordMessage("Content", embeds: new DiscordMessageEmbed[] { new DiscordMessageEmbed("Title", author: new DiscordMessageEmbedAuthor(new string('0', 257))) }));

                Assert.Fail("Message successfully sent.");
            }
            catch (DiscordWebhookClientException ex)
            {
                Assert.IsTrue(ex.Message.Contains("The embed author \"name\" length limit is 256 characters"));
            }
        }

        [Test]
        public async Task Must_Not_Send_Message_With_Embed_Fields_Size_Limit_Exceeded()
        {
            var fields = new List<DiscordMessageEmbedField>();

            for (var i = 1; i < 27; i++)
            {
                fields.Add(new DiscordMessageEmbedField("Field name", "Field value"));
            }
            
            try
            {
                await _proxy.SendToDiscord(new DiscordMessage("Content", embeds: new DiscordMessageEmbed[] { new DiscordMessageEmbed("Title", fields: fields) }));

                Assert.Fail("Message successfully sent.");
            }
            catch (DiscordWebhookClientException ex)
            {
                Assert.IsTrue(ex.Message.Contains("The embed \"fields\" collection size limit is 25 objects."));
            }
        }

        [Test]
        public async Task Must_Not_Send_Message_With_Embed_Field_Name_Empty()
        {
            try
            {
                await _proxy.SendToDiscord(new DiscordMessage("Content", embeds: new DiscordMessageEmbed[] { new DiscordMessageEmbed("Title", fields: new DiscordMessageEmbedField[] { new DiscordMessageEmbedField(string.Empty) }) }));

                Assert.Fail("Message successfully sent.");
            }
            catch (DiscordWebhookClientException ex)
            {
                Assert.IsTrue(ex.Message.Contains("The embed field \"name\" cannot be null or empty."));
            }
        }

        [Test]
        public async Task Must_Not_Send_Message_With_Embed_Field_Name_Length_Limit_Exceeded()
        {
            try
            {
                await _proxy.SendToDiscord(new DiscordMessage("Content", embeds: new DiscordMessageEmbed[] { new DiscordMessageEmbed("Title", fields: new DiscordMessageEmbedField[] { new DiscordMessageEmbedField(new string('0', 257)) }) }));

                Assert.Fail("Message successfully sent.");
            }
            catch (DiscordWebhookClientException ex)
            {
                Assert.IsTrue(ex.Message.Contains("The embed field \"name\" length limit is 256 characters"));
            }
        }

        [Test]
        public async Task Must_Not_Send_Message_With_Embed_Field_Value_Length_Limit_Exceeded()
        {
            try
            {
                await _proxy.SendToDiscord(new DiscordMessage("Content", embeds: new DiscordMessageEmbed[] { new DiscordMessageEmbed("Title", fields: new DiscordMessageEmbedField[] { new DiscordMessageEmbedField("Field name", new string('0', 1025)) }) }));

                Assert.Fail("Message successfully sent.");
            }
            catch (DiscordWebhookClientException ex)
            {
                Assert.IsTrue(ex.Message.Contains("The embed field \"value\" length limit is 1024 characters"));
            }
        }

        [Test]
        public async Task Must_Not_Send_Message_With_Embed_Thumbnail_Url_Empty()
        {
            try
            {
                await _proxy.SendToDiscord(new DiscordMessage("Content", embeds: new DiscordMessageEmbed[] { new DiscordMessageEmbed("Title", thumbnail: new DiscordMessageEmbedThumbnail(string.Empty)) }));

                Assert.Fail("Message successfully sent.");
            }
            catch (DiscordWebhookClientException ex)
            {
                Assert.IsTrue(ex.Message.Contains("The embed thumbnail \"url\" cannot be null or empty."));
            }
        }

        [Test]
        public async Task Must_Not_Send_Message_With_Embed_Image_Url_Empty()
        {
            try
            {
                await _proxy.SendToDiscord(new DiscordMessage("Content", embeds: new DiscordMessageEmbed[] { new DiscordMessageEmbed("Title", image: new DiscordMessageEmbedImage(string.Empty)) }));

                Assert.Fail("Message successfully sent.");
            }
            catch (DiscordWebhookClientException ex)
            {
                Assert.IsTrue(ex.Message.Contains("The embed image \"url\" cannot be null or empty."));
            }
        }

        [Test]
        public async Task Must_Not_Send_Message_With_Embed_Footer_Text_Empty()
        {
            try
            {
                await _proxy.SendToDiscord(new DiscordMessage("Content", embeds: new DiscordMessageEmbed[] { new DiscordMessageEmbed("Title", footer: new DiscordMessageEmbedFooter(string.Empty)) }));

                Assert.Fail("Message successfully sent.");
            }
            catch (DiscordWebhookClientException ex)
            {
                Assert.IsTrue(ex.Message.Contains("The embed footer \"text\" cannot be null or empty."));
            }
        }

        [Test]
        public async Task Must_Not_Send_Message_With_Embed_Footer_Text_Length_Limit_Exceeded()
        {
            try
            {
                await _proxy.SendToDiscord(new DiscordMessage("Content", embeds: new DiscordMessageEmbed[] { new DiscordMessageEmbed("Title", footer: new DiscordMessageEmbedFooter(new string('0', 2049))) }));

                Assert.Fail("Message successfully sent.");
            }
            catch (DiscordWebhookClientException ex)
            {
                Assert.IsTrue(ex.Message.Contains("The embed footer \"text\" length limit is 2048 characters"));
            }
        }
    }
}