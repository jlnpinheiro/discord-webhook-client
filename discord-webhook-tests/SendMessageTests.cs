using JNogueira.Discord.Webhook;
using NUnit.Framework;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace discord_webhook_tests
{
    public class Tests
    {
        private DiscordWebhookProxy _proxy;
        
        [SetUp]
        public void Setup()
        {
            _proxy = new DiscordWebhookProxy("https://discordapp.com/api/webhooks/666680930995404842/Octd4AZIFqtQes859s4Q3pg3OvxaNKjWOlHllUzyJ1FCpuMIaPBj3WaJxf3CD16VsPxk");
        }

        [Test]
        public async Task Must_Send_Hello_World_Message()
        {
            var message = new DiscordMessage("Must_Send_Hello_World_Message");

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
                        "Title embed :heavyblackheart:",
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

                var message = new DiscordMessage("An exception is handled!")
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
    }
}