using Newtonsoft.Json;
using Polly;
using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace JNogueira.Discord.Webhook.Client
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
                    var response = await Policy
                        .HandleResult<HttpResponseMessage>(x => !x.IsSuccessStatusCode)
                        .RetryAsync(3, onRetry: (httpResponse, count) =>
                        {
                            if ((int)httpResponse.Result.StatusCode == 429) // TOO MANY REQUESTS
                            {
                                var jsonBody = JsonConvert.DeserializeObject<DiscordTooManyRequestsResponse>(httpResponse.Result.Content.ReadAsStringAsync().Result);

                                if (jsonBody != null)
                                    System.Threading.Thread.Sleep(jsonBody.RetryAfter + 1);
                            }
                        })
                        .ExecuteAsync(async () => await client.PostAsync(_urlWebhook, content));

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

        /// <summary>
        /// Send a message with files to Discord using a webhook
        /// </summary>
        /// <param name="message">Message to be sent</param>
        /// <param name="files">Files to be sent</param>
        public async Task SendToDiscord(DiscordMessage message, DiscordFile[] files)
        {
            try
            {
                if (files == null || files.Length == 0)
                    await SendToDiscord(message);

                if (message == null)
                    throw new ArgumentNullException(nameof(message), "The message cannot be null.");

                if (message.Invalido)
                    throw new DiscordWebhookClientException($"The message cannot be sent: {string.Join(", ", message.Mensagens)}");

                if (files.Any(x => x.Invalido))
                    throw new DiscordWebhookClientException($"The message cannot be sent: {string.Join(", ", files.Select(x => string.Join(" - ", x.Mensagens)).ToList())}");

                using (var formContent = new MultipartFormDataContent())
                {
                    formContent.Add(new StringContent(message.ToJson(), Encoding.UTF8), "payload_json");

                    var count = 1;

                    foreach (var file in files)
                    {
                        var fileContent = new ByteArrayContent(file.Content);

                        formContent.Add(fileContent, "file" + count, file.Name);

                        count++;
                    }

                    using (var client = new HttpClient { Timeout = new TimeSpan(0, 0, 30) })
                    {
                        var response = await client.PostAsync(_urlWebhook, formContent);

                        if (!response.IsSuccessStatusCode && response.StatusCode != HttpStatusCode.NoContent)
                        {
                            throw new DiscordWebhookClientException($"An error occurred in sending the message: {await response.Content.ReadAsStringAsync()} - HTTP status code {(int)response.StatusCode} - {response.StatusCode}");
                        }
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

    public class DiscordTooManyRequestsResponse
    {
        [JsonProperty("message")]
        public string Message { get; set; }

        [JsonProperty("retry_after")]
        public int RetryAfter { get; set; }

        [JsonProperty("global")]
        public bool Global { get; set; }
    }
}
