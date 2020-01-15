using JNogueira.Infraestrutura.NotifiqueMe;
using Newtonsoft.Json;
using System.Linq;

namespace JNogueira.Discord.Webhook
{
    [JsonObject(MemberSerialization.OptIn)]
    public class DiscordMessage : Notificavel
    {
        /// <summary>
        /// Overrides the current username of the webhook
        /// </summary>
        [JsonProperty("username")]
        public string Username { get; set; }

        /// <summary>
        /// Overrides the default avatar of the webhook
        /// </summary>
        [JsonProperty("avatar_url")]
        public string AvatarUrl { get; set; }

        /// <summary>
        /// Simple message, the message contains (up to 2000 characters)
        /// </summary>
        [JsonProperty("content")]
        public string Content { get; set; }

        /// <summary>
        /// If true, the message will be pronounced in chat like tts message
        /// </summary>
        [JsonProperty("tts")]
        public bool Tts { get; set; }

        /// <summary>
        /// An array of embed objects. That means you put use more than one in the same body
        /// </summary>
        [JsonProperty("embeds")]
        public DiscordMessageEmbed[] Embeds { get; set; }

        [JsonConstructor]
        private DiscordMessage()
        {

        }

        public DiscordMessage(string content, bool tts = false, DiscordMessageEmbed[] embeds = null)
        {
            this.Content = content;
            this.Tts     = tts;
            this.Embeds  = embeds;

            Validate();
        }

        internal void Validate()
        {
            this
                .NotificarSeNuloOuVazio(this.Content, "The \"content\" field cannot be null or empty.")
                .NotificarSeVerdadeiro(!string.IsNullOrEmpty(this.Content) && this.Content.Length > 2000, $"The \"content\" field length limit is 2000 characters (actual lenght is {this.Content.Length}).")
                .NotificarSeVerdadeiro(this.Embeds?.Any(x => x == null) == true, "The \"embeds\" field cannot have null elements in the array.");

            this.Embeds?
                .ToList()
                .ForEach(x =>
                {
                    if (x.Invalido)
                        this.AdicionarNotificacoes(x.Notificacoes);
                });
        }

        internal string ToJson() => this == null
            ? string.Empty
            : JsonConvert.SerializeObject(this, Formatting.Indented, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore, ReferenceLoopHandling = ReferenceLoopHandling.Ignore });
    }
}
