using JNogueira.Discord.Webhook;
using NUnit.Framework;
using System;
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
            _proxy = new DiscordWebhookProxy("");
        }

        [Test]
        public async Task Must_Send_Hello_World_Message()
        {
            var message = new DiscordMessage("Must_Send_Hello_World_Message");

            await _proxy.Send(message);

            Assert.Pass();
        }

        [Test]
        public async Task Must_Send_Message_With_Embeds()
        {
            var message = new DiscordMessage("Must_Send_Message_With_Embeds!")
            {
                Embeds = new[]
                {
                    new DiscordMessageEmbed("Embed 1", author: new DiscordMessageEmbedAuthor("Embed 1 author name"))
                }
            };
            
            await _proxy.Send(message);

            Assert.Pass();
        }
    }
}