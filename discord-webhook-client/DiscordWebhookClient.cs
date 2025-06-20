using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace JNogueira.Discord.WebhookClient;

public class DiscordWebhookClient(DiscordWebhookHttpClient client)
{
    public async Task<bool> SendToDiscordAsync(DiscordMessage message, DiscordFile[] files = null, bool sendMessageAsFileAttachmentOnError = false)
    {
        try
        {
            if (message is null)
                throw new DiscordWebhookClientException("Message object cannot be null.");

            if (message.IsInvalid)
                throw new DiscordWebhookClientException($"The message param is invalid: {string.Join(", ", message.Mensagens)}");

            if (files?.Length > 10)
                throw new DiscordWebhookClientException($"Files collection is limited at 10 items. (actual collection size is {files.Length})");

            HttpContent content;

            if (files?.Length > 0)
            {
                var multipart = new MultipartFormDataContent
                {
                    { new StringContent(message.ToJson(), Encoding.UTF8, "application/json"), "payload_json" }
                };

                var count = 1;

                foreach (var file in files)
                {
                    var fileContent = new ByteArrayContent(file.Content);

                    multipart.Add(fileContent, "file" + count, file.Name);

                    count++;
                }

                content = multipart;
            }
            else
            {
                content = new StringContent(message.ToJson(), Encoding.UTF8, "application/json");
            }

            var response = await client.Post(content, onRetryAsync: async (httpResponse, _, __, ___) =>
            {
                if ((int)httpResponse.Result.StatusCode == (int)HttpStatusCode.TooManyRequests)
                {
                    var jsonBody = await JsonSerializer.DeserializeAsync<DiscordTooManyRequestsResponse>(await httpResponse.Result.Content.ReadAsStreamAsync());

                    if (jsonBody is not null)
                        await Task.Delay(TimeSpan.FromSeconds(jsonBody.RetryAfter + 1));
                }
            });

            if (!response.IsSuccessStatusCode && response.StatusCode != HttpStatusCode.NoContent)
            {
                var responseContent = await response.Content.ReadAsStringAsync();

                throw new DiscordWebhookClientException($"An error occurred while sending the message: {responseContent} - HTTP status code {(int)response.StatusCode} - {response.StatusCode}", responseContent, response.StatusCode);
            }

            return true;
        }
        catch (DiscordWebhookClientException ex) when (sendMessageAsFileAttachmentOnError)
        {
            await SendToDiscordAsAttachment(message, ex);
            return false;
        }
        catch (DiscordWebhookClientException)
        {
            throw;
        }
        catch (Exception ex)
        {
            if (!sendMessageAsFileAttachmentOnError)
                throw new DiscordWebhookClientException("An error occurred while sending the message.", ex);

            await SendToDiscordAsAttachment(message, ex);
            return false;
        }
    }

    private async Task SendToDiscordAsAttachment(DiscordMessage originalMessage, Exception exception, DiscordFile[] files = null)
    {
        if (originalMessage is null || exception is null)
            return;
        
        try
        {
            var attachmentMessage = new DiscordMessage(
                $"{DiscordEmoji.Skull} **Discord Webhook Client error**: There was an error sending the message to Discord. The original message content and exception details are attached as file.",
                avatarUrl: "https://discord.com/assets/9f6f9cd156ce35e2d94c0e62e3eff462.png",
                tts: false,
                embeds:
                [
                    new DiscordMessageEmbed(
                        color: (int)DiscordColor.Red,
                        fields:
                        [
                            new DiscordMessageEmbedField("Exception", exception.GetBaseException().Message),
                            new DiscordMessageEmbedField("Type", exception.GetType().ToString()),
                        ]
                    )
                ]
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

            await SendToDiscordAsync(attachmentMessage, [.. attachmentFiles]);
        }
        catch (DiscordWebhookClientException)
        {
            throw;
        }
        catch
        {
            throw new DiscordWebhookClientException("An error occurred while sending the message.", exception);
        }
    }
}

public class DiscordTooManyRequestsResponse
{
    [JsonPropertyName("message")]
    public string Message { get; set; }

    [JsonPropertyName("retry_after")]
    public double RetryAfter { get; set; }

    [JsonPropertyName("global")]
    public bool Global { get; set; }
}
