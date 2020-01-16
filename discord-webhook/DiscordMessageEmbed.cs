using JNogueira.Infraestrutura.NotifiqueMe;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;

namespace JNogueira.Discord.Webhook
{
    [JsonObject(MemberSerialization.OptIn)]
    public class DiscordMessageEmbed : Notificavel
    {
        /// <summary>
        /// Color code of the embed. You have to use Decimal numeral system, not Hexadecimal. You can use SpyColor (https://www.spycolor.com) for that. It has a decimal number converter.
        /// </summary>
        [JsonProperty("color")]
        public int? Color { get; set; }

        /// <summary>
        ///  Embed author object
        /// </summary>
        [JsonProperty("author")]
        public DiscordMessageEmbedAuthor Author { get; set; }

        /// <summary>
        /// Title of embed
        /// </summary>
        [JsonProperty("title")]
        public string Title { get; set; }

        /// <summary>
        /// Url of embed. If title was used, it becomes hyperlink
        /// </summary>
        [JsonProperty("url")]
        public string Url { get; set; }

        /// <summary>
        /// Description text
        /// </summary>
        [JsonProperty("description")]
        public string Description { get; set; }

        /// <summary>
        /// Array of embed field objects
        /// </summary>
        [JsonProperty("fields")]
        public DiscordMessageEmbedField[] Fields { get; set; }

        /// <summary>
        /// Embed thumbnail object
        /// </summary>
        [JsonProperty("thumbnail")]
        public DiscordMessageEmbedThumbnail Thumbnail { get; set; }

        /// <summary>
        /// Embed image object
        /// </summary>
        [JsonProperty("image")]
        public DiscordMessageEmbedImage Image { get; set; }

        /// <summary>
        /// Embed footer object
        /// </summary>
        [JsonProperty("footer")]
        public DiscordMessageEmbedFooter Footer { get; set; }

        [JsonConstructor]
        private DiscordMessageEmbed()
        {

        }

        public DiscordMessageEmbed(
            string title,
            int? color = null,
            DiscordMessageEmbedAuthor author = null,
            string url = null,
            string description = null,
            IEnumerable<DiscordMessageEmbedField> fields = null,
            DiscordMessageEmbedThumbnail thumbnail = null,
            DiscordMessageEmbedImage image = null,
            DiscordMessageEmbedFooter footer = null)
        {
            this.Color       = color;
            this.Author      = author;
            this.Title       = title;
            this.Url         = url?.ToLower();
            this.Description = description;
            this.Fields      = fields?.ToArray();
            this.Thumbnail   = thumbnail;
            this.Image       = image;
            this.Footer      = footer;

            Validate();
        }

        internal void Validate()
        {
            this.NotificarSeNuloOuVazio(this.Title, "The embed \"title\" field cannot be null or empty.");

            this.Fields?
                .ToList()
                .ForEach(x =>
                {
                    if (x.Invalido)
                        this.AdicionarNotificacoes(x.Notificacoes);
                });

            this.AdicionarNotificacoes(this.Author?.Notificacoes);
            this.AdicionarNotificacoes(this.Thumbnail?.Notificacoes);
            this.AdicionarNotificacoes(this.Image?.Notificacoes);
            this.AdicionarNotificacoes(this.Footer?.Notificacoes);
        }
    }

    /// <summary>
    /// Embed author object
    /// </summary>
    [JsonObject(MemberSerialization.OptIn)]
    public class DiscordMessageEmbedAuthor : Notificavel
    {
        /// <summary>
        /// Name of author
        /// </summary>
        [JsonProperty("name")]
        public string Name { get; set; }

        /// <summary>
        /// Url of author. If name was used, it becomes a hyperlink
        /// </summary>
        [JsonProperty("url")]
        public string Url { get; set; }

        /// <summary>
        /// Url of author icon
        /// </summary>
        [JsonProperty("icon_url")]
        public string IconUrl { get; set; }

        [JsonConstructor]
        private DiscordMessageEmbedAuthor()
        {

        }

        public DiscordMessageEmbedAuthor(string name, string url = null, string iconUrl = null)
        {
            this.Name    = name;
            this.Url     = url?.ToLower();
            this.IconUrl = iconUrl?.ToLower();

            Validate();
        }

        internal void Validate()
        {
            this.NotificarSeNuloOuVazio(this.Name, "The embed author \"name\" field cannot be null or empty.");
        }
    }

    /// <summary>
    /// Embed field objects
    /// </summary>
    [JsonObject(MemberSerialization.OptIn)]
    public class DiscordMessageEmbedField : Notificavel
    {
        /// <summary>
        /// The name of the field
        /// </summary>
        [JsonProperty("name")]
        public string Name { get; set; }

        /// <summary>
        /// The value of the field
        /// </summary>
        [JsonProperty("value")]
        public string Value { get; set; }

        /// <summary>
        /// If true, fields will be displayed in the same line, but there can only be 3 max in the same line or 2 max if you used thumbnail
        /// </summary>
        [JsonProperty("inline")]
        public bool InLine { get; set; }

        [JsonConstructor]
        private DiscordMessageEmbedField()
        {

        }

        public DiscordMessageEmbedField(string name, string value, bool inLine = false)
        {
            Name   = name;
            Value  = value;
            InLine = inLine;

            Validate();
        }

        public void Validate()
        {
            this
                .NotificarSeNuloOuVazio(this.Name, "The embed field \"name\" field cannot be null or empty.")
                .NotificarSeNuloOuVazio(this.Name, "The embed field \"value\" field cannot be null or empty.");
        }
    }

    /// <summary>
    /// Embed thumbnail object
    /// </summary>
    [JsonObject(MemberSerialization.OptIn)]
    public class DiscordMessageEmbedThumbnail : Notificavel
    {
        /// <summary>
        /// url of thumbnail
        /// </summary>
        public string Url { get; set; }

        [JsonConstructor]
        private DiscordMessageEmbedThumbnail()
        {

        }

        public DiscordMessageEmbedThumbnail(string url)
        {
            this.Url = url?.ToLower();

            Validate();
        }

        internal void Validate()
        {
            this.NotificarSeNuloOuVazio(this.Url, "The embed thumbnail \"url\" field cannot be null or empty.");
        }
    }

    /// <summary>
    /// Embed image object
    /// </summary>
    [JsonObject(MemberSerialization.OptIn)]
    public class DiscordMessageEmbedImage : Notificavel
    {
        /// <summary>
        /// url of image
        /// </summary>
        public string Url { get; set; }

        [JsonConstructor]
        private DiscordMessageEmbedImage()
        {

        }

        public DiscordMessageEmbedImage(string url)
        {
            this.Url = url?.ToLower();

            Validate();
        }

        internal void Validate()
        {
            this.NotificarSeNuloOuVazio(this.Url, "The embed image \"url\" field cannot be null or empty.");
        }
    }

    /// <summary>
    /// Embed footer object
    /// </summary>
    [JsonObject(MemberSerialization.OptIn)]
    public class DiscordMessageEmbedFooter : Notificavel
    {
        /// <summary>
        /// Footer text, doesn't support Markdown
        /// </summary>
        [JsonProperty("text")]
        public string Text { get; set; }

        /// <summary>
        /// Url of footer icon
        /// </summary>
        [JsonProperty("icon_url")]
        public string IconUrl { get; set; }

        [JsonConstructor]
        private DiscordMessageEmbedFooter()
        {

        }

        public DiscordMessageEmbedFooter(string text)
        {
            this.Text = text;

            Validate();
        }

        internal void Validate()
        {
            this.NotificarSeNuloOuVazio(this.Text, "The embed footer \"text\" field cannot be null or empty.");
        }
    }
}
