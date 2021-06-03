using Newtonsoft.Json;
using Polly;
using System;
using System.Collections;
using System.Collections.Generic;
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
        /// <param name="sendMessageAsFileAttachmentOnError">When an error occurs in sending the message, the client sends the message content as an file attachment.</param>
        public async Task SendToDiscord(DiscordMessage message, bool sendMessageAsFileAttachmentOnError = false)
        {
            try
            {
                if (message == null)
                    throw new ArgumentNullException(nameof(message), "The message cannot be null.");

                if (message.Invalido)
                    throw new DiscordWebhookClientException($"The message param is invalid: {string.Join(", ", message.Mensagens)}");

                using (var content = new StringContent(message.ToJson(), Encoding.UTF8, "application/json"))
                using (var client = new HttpClient { Timeout = new TimeSpan(0, 0, 30) })
                {
                    var response = await Policy
                        .HandleResult<HttpResponseMessage>(x => !x.IsSuccessStatusCode)
                        .RetryAsync(3, onRetry: (httpResponse, _) =>
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
                        var responseContent = await response.Content.ReadAsStringAsync();

                        throw new DiscordWebhookClientException($"An error occurred in sending the message: {responseContent} - HTTP status code {(int)response.StatusCode} - {response.StatusCode}", responseContent, response.StatusCode);
                    }
                }
            }
            catch (DiscordWebhookClientException ex)
            {
                if (!sendMessageAsFileAttachmentOnError)
                    throw;

                await SendToDiscordAsAttachment(message, ex);
            }
            catch (Exception ex)
            {
                if (!sendMessageAsFileAttachmentOnError)
                    throw new DiscordWebhookClientException("An error occurred in sending the message.", ex);

                await SendToDiscordAsAttachment(message, ex);
            }
        }

        /// <summary>
        /// Send a message with files to Discord using a webhook
        /// </summary>
        /// <param name="message">Message to be sent</param>
        /// <param name="files">Files to be sent</param>
        /// <param name="sendMessageAsFileAttachmentOnError">When an error occurs in sending the message, the client sends the message content as an file attachment.</param>
        public async Task SendToDiscord(DiscordMessage message, DiscordFile[] files, bool sendMessageAsFileAttachmentOnError = false)
        {
            try
            {
                if (files == null || files.Length == 0)
                    await SendToDiscord(message);

                if (files.Length > 10)
                    throw new DiscordWebhookClientException($"Files collection size limit is 10 objects. (actual size is {files.Length})");

                if (message == null)
                    throw new DiscordWebhookClientException("The message parameter cannot be null.");

                message.AdicionarNotificacoes(message.Embeds);
                
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
                        var response = await Policy
                            .HandleResult<HttpResponseMessage>(x => !x.IsSuccessStatusCode)
                            .RetryAsync(3, onRetry: (httpResponse, _) =>
                            {
                                if ((int)httpResponse.Result.StatusCode == 429) // TOO MANY REQUESTS
                                {
                                    var jsonBody = JsonConvert.DeserializeObject<DiscordTooManyRequestsResponse>(httpResponse.Result.Content.ReadAsStringAsync().Result);

                                    if (jsonBody != null)
                                        System.Threading.Thread.Sleep(jsonBody.RetryAfter + 1);
                                }
                            })
                            .ExecuteAsync(async () => await client.PostAsync(_urlWebhook, formContent));

                        if (!response.IsSuccessStatusCode && response.StatusCode != HttpStatusCode.NoContent)
                        {
                            var responseContent = await response.Content.ReadAsStringAsync();

                            throw new DiscordWebhookClientException($"An error occurred in sending the message: {responseContent} - HTTP status code {(int)response.StatusCode} - {response.StatusCode}", responseContent, response.StatusCode);
                        }
                    }
                }
            }
            catch (DiscordWebhookClientException ex)
            {
                if (!sendMessageAsFileAttachmentOnError)
                    throw;

                await SendToDiscordAsAttachment(message, ex, files);
            }
            catch (Exception ex)
            {
                if (!sendMessageAsFileAttachmentOnError)
                    throw new DiscordWebhookClientException("An error occurred in sending the message.", ex);

                await SendToDiscordAsAttachment(message, ex, files);
            }
        }

        private async Task SendToDiscordAsAttachment(DiscordMessage originalMessage, Exception exception, DiscordFile[] files = null)
        {
            if (originalMessage == null || exception == null)
                return;
            
            try
            {
                var attachmentMessage = new DiscordMessage(
                    $"{DiscordEmoji.Skull} **Discord Webhook Client error**: There was an error sending the message to Discord. The original message content and exception details are attached as file.",
                    avatarUrl: "https://discord.com/assets/9f6f9cd156ce35e2d94c0e62e3eff462.png",
                    tts: false,
                    embeds: new[]
                    {
                        new DiscordMessageEmbed(
                            color: (int)DiscordColor.Red,
                            fields: new []
                            {
                                new DiscordMessageEmbedField("Exception", exception.GetBaseException().Message),
                                new DiscordMessageEmbedField("Type", exception.GetType().ToString()),
                            }
                        )
                    }
                );

                var originalMessageAttachment = new DiscordFile("original-message.txt", Encoding.UTF8.GetBytes(originalMessage.ToTxtFileContent()));

                var exceptionInfo = new StringBuilder();
                exceptionInfo.Append("Message: ").AppendLine(exception.Message);
                exceptionInfo.Append("Exception type: ").AppendLine(exception.GetType().ToString());
                exceptionInfo.Append("Source: ").AppendLine(exception.Source);
                exceptionInfo.Append("Base exception: ").AppendLine(exception.GetBaseException()?.Message);

                foreach (DictionaryEntry data in exception.Data)
                    exceptionInfo.AppendLine($"{data.Key}: {data.Value}");

                exceptionInfo.Append("Stack trace: ").Append(exception.StackTrace);

                var exceptionAttachment = new DiscordFile("exception.txt", Encoding.UTF8.GetBytes(exceptionInfo.ToString()));

                var attachmentFiles = new List<DiscordFile>();

                if (files?.Length > 0)
                    attachmentFiles.AddRange(files);

                attachmentFiles.Add(originalMessageAttachment);
                attachmentFiles.Add(exceptionAttachment);

                await SendToDiscord(attachmentMessage, attachmentFiles.ToArray());
            }
            catch (DiscordWebhookClientException)
            {
                throw;
            }
            catch
            {
                throw new DiscordWebhookClientException("An error occurred in sending the message.", exception);
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
