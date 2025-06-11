using System;
using System.Net;

namespace JNogueira.Discord.WebhookClient;

[Serializable]
public class DiscordWebhookClientException : Exception
{
    public DiscordWebhookClientException()
    {
    }

    public DiscordWebhookClientException(string message) : base(message)
    {
    }

    public DiscordWebhookClientException(string message, Exception inner) : base(message, inner)
    {
    }

    public DiscordWebhookClientException(string message, string responseContent, HttpStatusCode responseHttpStatusCode) : base(message)
    {
        if (!string.IsNullOrEmpty(responseContent))
            Data["Discord response content"] = responseContent;

        Data["Discord response status code"] = $"{(int)responseHttpStatusCode} - {responseHttpStatusCode}";
    }
}
