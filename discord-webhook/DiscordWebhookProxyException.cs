using System;

namespace JNogueira.Discord.Webhook
{

    [Serializable]
    public class DiscordWebhookProxyException : Exception
    {
        public DiscordWebhookProxyException() { }
        public DiscordWebhookProxyException(string message) : base(message) { }
        public DiscordWebhookProxyException(string message, Exception inner) : base(message, inner) { }
        protected DiscordWebhookProxyException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}
