using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace JNogueira.Discord.Webhook
{
    public class DiscordWebhookClient
    {
        private string _urlWebhook;
        
        /// <summary>
        /// A client class to send messages using a Discord webhook.
        /// </summary>
        /// <param name="urlWebhook">The Discord webhook url</param>
        public DiscordWebhookClient(string urlWebhook)
        {
            if (string.IsNullOrEmpty(urlWebhook))
                throw new ArgumentNullException(nameof(urlWebhook), "The Discord webhook url cannot be null or empty.");

            _urlWebhook = urlWebhook;
        }

        /// <summary>
        /// Send a message to Discord using a webhook
        /// </summary>
        /// <param name="message">Message to be sent</param>
        public async Task SendToDiscord(DiscordMessage message)
        {
            try
            {
                if (message == null)
                    throw new ArgumentNullException(nameof(message), "The message cannot be null.");

                if (message.Invalido)
                    throw new DiscordWebhookClientException($"The message cannot be sent: {string.Join(", ", message.Mensagens)}");

                using (var content = new StringContent(message.ToJson(), Encoding.UTF8, "application/json"))
                using (var client = new HttpClient { Timeout = new TimeSpan(0, 0, 30) })
                {
                    var response = await client.PostAsync(_urlWebhook, content);

                    if (!response.IsSuccessStatusCode && response.StatusCode != HttpStatusCode.NoContent)
                    {
                        throw new DiscordWebhookClientException($"An error occurred in sending the message: {await response.Content.ReadAsStringAsync()} - HTTP status code {(int)response.StatusCode} - {response.StatusCode}");
                    }
                }
            }
            catch (DiscordWebhookClientException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new DiscordWebhookClientException("An error occurred in sending the message.", ex);
            }
        }
    }
}
