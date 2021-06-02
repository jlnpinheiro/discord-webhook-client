using System;
using System.Net;

namespace JNogueira.Discord.Webhook.Client
{

    [Serializable]
    public class DiscordWebhookClientException : Exception
    {
        public DiscordWebhookClientException() : base()
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
                this.Data["Discord response content"] = responseContent;

            this.Data["Discord response status code"] = $"{(int)responseHttpStatusCode} - {responseHttpStatusCode}";
        }
    }
}
