using System;

namespace JNogueira.Discord.Webhook.Client
{

    [Serializable]
    public class DiscordWebhookClientException : Exception
    {
        public DiscordWebhookClientException() { }
        public DiscordWebhookClientException(string message) : base(message) { }
        public DiscordWebhookClientException(string message, Exception inner) : base(message, inner) { }
        protected DiscordWebhookClientException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}
