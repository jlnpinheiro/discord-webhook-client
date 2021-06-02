using JNogueira.NotifiqueMe;
using Newtonsoft.Json;
using System.Linq;
using System.Text;

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
            this.Content   = content?.Trim();
            this.Username  = username?.Trim();
            this.AvatarUrl = avatarUrl?.Trim();
            this.Tts       = tts;
            this.Embeds    = embeds?.Where(x => x != null)?.ToArray();

            Validate();
        }

        internal void Validate()
        {
            this.NotificarSeNuloOuVazio(this.Content, "The \"content\" element cannot be null or empty.")
                .NotificarSeVerdadeiro(!string.IsNullOrEmpty(this.Content) && this.Content?.Length > 2000, $"The \"content\" element length limit is 2000 characters (actual lenght is {this.Content?.Length}).");

            if (this.Embeds?.Length > 0)
            {
                this.NotificarSeVerdadeiro(this.Embeds?.Length > 10, $"The \"embeds\" collection size limit is 10 elements (actual size is {this.Embeds.Length}).");

                this.Embeds
                    .ToList()
                    .ForEach(x =>
                    {
                        if (x?.Invalido == true)
                            this.AdicionarNotificacoes(x.Notificacoes);
                    });
            }
        }

        public int GetMessageCaractersSize()
        {
            var sumCaractersInMessage = new StringBuilder();

            sumCaractersInMessage.Append(this.Embeds.Select(x => x.Title).ToArray());

            sumCaractersInMessage.Append(this.Embeds.Select(x => x.Description).ToArray());

            sumCaractersInMessage.Append((from embed in this.Embeds
                                          from field in embed.Fields
                                          select field.Name).ToArray());

            sumCaractersInMessage.Append((from embed in this.Embeds
                                          from field in embed.Fields
                                          select field.Value).ToArray());

            sumCaractersInMessage.Append(this.Embeds.Select(x => x.Footer.Text).ToArray());

            sumCaractersInMessage.Append(this.Embeds.Select(x => x.Author.Name).ToArray());

            return sumCaractersInMessage.Length;
        }

        public string ToJson() => this == null
            ? string.Empty
            : JsonConvert.SerializeObject(this, Formatting.Indented, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore, ReferenceLoopHandling = ReferenceLoopHandling.Ignore });

        public string ToTxtFileContent()
        {
            if (this == null)
                return string.Empty;
            
            var attachmentErrorMessage = new StringBuilder();

            attachmentErrorMessage.AppendLine("####### MESSAGE #######");

            if (!string.IsNullOrEmpty(this.Username))
            {
                attachmentErrorMessage.Append("Message username: ").AppendLine(this.Username);
            }

            if (!string.IsNullOrEmpty(this.AvatarUrl))
            {
                attachmentErrorMessage.Append("Message avatar URL: ").AppendLine(this.AvatarUrl);
            }

            attachmentErrorMessage.Append("Message TTS: ").AppendLine(this.Tts.ToString());

            if (!string.IsNullOrEmpty(this.Content))
            {
                attachmentErrorMessage.AppendLine("Message content:");
                attachmentErrorMessage.AppendLine(this.Content);
            }            

            if (this.Embeds?.Length > 0)
            {
                int embedIndex = 1;

                attachmentErrorMessage.AppendLine();
                attachmentErrorMessage.AppendLine("-- Message embeds --");
                attachmentErrorMessage.AppendLine();

                foreach (var embed in this.Embeds)
                {
                    attachmentErrorMessage.Append("-- Embed #").AppendLine(embedIndex.ToString());
                    attachmentErrorMessage.AppendLine(embed.ToTxtFileContent());

                    embedIndex++;
                }
            }

            return attachmentErrorMessage.ToString();
        }
    }
}