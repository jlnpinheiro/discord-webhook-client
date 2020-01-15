using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace JNogueira.Discord.Webhook
{
    public class DiscordWebhookProxy
    {
        private string _urlWebhook;
        
        /// <summary>
        /// A proxy class to send messages using a Discord webhook.
        /// </summary>
        /// <param name="urlWebhook"></param>
        public DiscordWebhookProxy(string urlWebhook)
        {
            _urlWebhook = urlWebhook;
        }

        /// <summary>
        /// Send a message to Discord using a webhook
        /// </summary>
        /// <param name="message">Message to be sent</param>
        public async Task Send(DiscordMessage message)
        {
            try
            {
                if (message == null)
                    throw new ArgumentNullException(nameof(message));

                if (message.Invalido)
                    throw new DiscordWebhookProxyException($"The message cannot be sent: {string.Join(", ", message.Mensagens)}");

                using (var content = new StringContent(message.ToJson(), Encoding.UTF8, "application/json"))
                using (var client = new HttpClient { Timeout = new TimeSpan(0, 0, 30) })
                {
                    var response = await client.PostAsync(_urlWebhook, content);

                    if (!response.IsSuccessStatusCode && response.StatusCode != HttpStatusCode.NoContent)
                    {
                        throw new DiscordWebhookProxyException($"An error occurred in sending the message: {await response.Content.ReadAsStringAsync()} - HTTP status code {(int)response.StatusCode} - {response.StatusCode}");
                    }
                }
            }
            catch (DiscordWebhookProxyException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new DiscordWebhookProxyException("An error occurred in sending the message.", ex);
            }
        }
    }
}
