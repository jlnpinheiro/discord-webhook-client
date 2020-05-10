using JNogueira.NotifiqueMe;
using Newtonsoft.Json;
using System.Linq;

namespace JNogueira.Discord.Webhook.Client
{
    [JsonObject(MemberSerialization.OptIn)]
    public class DiscordMessage : Notificavel
    {
        /// <summary>
        /// Overrides the current username of the webhook
        /// </summary>
        [JsonProperty("username")]
        public string Username { get; private set; }

        /// <summary>
        /// Overrides the default avatar of the webhook
        /// </summary>
        [JsonProperty("avatar_url")]
        public string AvatarUrl { get; private set; }

        /// <summary>
        /// Simple message, the message contains (up to 2000 characters)
        /// </summary>
        [JsonProperty("content")]
        public string Content { get; private set; }

        /// <summary>
        /// If true, the message will be pronounced in chat like tts message
        /// </summary>
        [JsonProperty("tts")]
        public bool Tts { get; private set; }

        /// <summary>
        /// An array of embed objects. That means you put use more than one in the same body
        /// </summary>
        [JsonProperty("embeds")]
        public DiscordMessageEmbed[] Embeds { get; private set; }

        [JsonConstructor]
        private DiscordMessage()
        {

        }

        public DiscordMessage(
            string content,
            string username = null,
            string avatarUrl = null,
            bool tts = false,
            DiscordMessageEmbed[] embeds = null)
        {
            this.Content   = content;
            this.Username  = username;
            this.AvatarUrl = avatarUrl;
            this.Tts       = tts;
            this.Embeds    = embeds;

            Validate();
        }

        internal void Validate()
        {
            this
                .NotificarSeNuloOuVazio(this.Content, "The \"content\" cannot be null or empty.")
                .NotificarSeVerdadeiro(!string.IsNullOrEmpty(this.Content) && this.Content?.Length > 2000, $"The \"content\" length limit is 2000 characters (actual lenght is {this.Content?.Length}).");

            if (this.Embeds?.Any() == true)
            {
                this.NotificarSeVerdadeiro(this.Embeds.Any(x => x == null), "The \"embeds\" cannot have null elements in the array.");

                this.Embeds
                    .ToList()
                    .ForEach(x =>
                    {
                        if (x?.Invalido == true)
                            this.AdicionarNotificacoes(x.Notificacoes);
                    });
            }
        }

        public string ToJson() => this == null
            ? string.Empty
            : JsonConvert.SerializeObject(this, Formatting.Indented, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore, ReferenceLoopHandling = ReferenceLoopHandling.Ignore });
    }
}