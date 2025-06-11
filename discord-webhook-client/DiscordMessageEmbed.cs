using JNogueira.NotifiqueMe;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;

namespace JNogueira.Discord.WebhookClient;

public class DiscordMessageEmbed : BaseDiscordMessage
{
    /// <summary>
    /// Color code of the embed. You have to use Decimal numeral system, not Hexadecimal. You can use SpyColor (https://www.spycolor.com) for that. It has a decimal number converter.
    /// </summary>
    [JsonPropertyName("color")]
    public int? Color { get; }

    /// <summary>
    ///  Embed author object
    /// </summary>
    [JsonPropertyName("author")]
    public DiscordMessageEmbedAuthor Author { get; }

    /// <summary>
    /// Title of embed
    /// </summary>
    [JsonPropertyName("title")]
    public string Title { get; }

    /// <summary>
    /// Url of embed. If title was used, it becomes hyperlink
    /// </summary>
    [JsonPropertyName("url")]
    public string Url { get; }

    /// <summary>
    /// Description text
    /// </summary>
    [JsonPropertyName("description")]
    public string Description { get; }

    /// <summary>
    /// Array of embed field objects
    /// </summary>
    [JsonPropertyName("fields")]
    public DiscordMessageEmbedField[] Fields { get; }

    /// <summary>
    /// Embed thumbnail object
    /// </summary>
    [JsonPropertyName("thumbnail")]
    public DiscordMessageEmbedThumbnail Thumbnail { get; }

    /// <summary>
    /// Embed image object
    /// </summary>
    [JsonPropertyName("image")]
    public DiscordMessageEmbedImage Image { get; }

    /// <summary>
    /// Embed footer object
    /// </summary>
    [JsonPropertyName("footer")]
    public DiscordMessageEmbedFooter Footer { get; }

    [JsonConstructor]
    private DiscordMessageEmbed()
    {
    }

    public DiscordMessageEmbed(
        string title = null,
        int? color = null,
        DiscordMessageEmbedAuthor author = null,
        string url = null,
        string description = null,
        IEnumerable<DiscordMessageEmbedField> fields = null,
        DiscordMessageEmbedThumbnail thumbnail = null,
        DiscordMessageEmbedImage image = null,
        DiscordMessageEmbedFooter footer = null)
    {
        Color = color;
        Author = author;
        Title = title?.Trim();
        Url = url?.Trim();
        Description = description?.Trim();
        Fields = fields?.ToArray();
        Thumbnail = thumbnail;
        Image = image;
        Footer = footer;

        this.NotificarSeVerdadeiro(Title?.Length > 256, $"The embed \"title\" length limit is 256 characters (actual lenght is {Title?.Length}).")
            .NotificarSeVerdadeiro(Description?.Length > 2048, $"The embed \"description\" length limit is 2048 characters (actual lenght is {Description?.Length}).");

        if (Fields?.Length > 0)
        {
            this.NotificarSeVerdadeiro(Fields.Length > 25, $"The embed \"fields\" collection size limit is 25 elements. (actual size is {Fields.Length})");

            Fields.ToList().ForEach(x => AdicionarNotificacoes(x));
        }

        AdicionarNotificacoes(Author);
        AdicionarNotificacoes(Thumbnail);
        AdicionarNotificacoes(Image);
        AdicionarNotificacoes(Footer);
    }

    public string ToTxtFileContent()
    {
        var attachmentErrorMessage = new StringBuilder();

        if (Color.HasValue)
            attachmentErrorMessage.Append("Embed color: ").AppendLine(Color.ToString());

        if (Author != null)
            attachmentErrorMessage.AppendFormat(Author.ToTxtFileContent());

        if (!string.IsNullOrEmpty(Title))
            attachmentErrorMessage.Append("Embed title: ").AppendLine(Title);

        if (!string.IsNullOrEmpty(Url))
            attachmentErrorMessage.Append("Embed URL: ").AppendLine(Url);

        if (!string.IsNullOrEmpty(Description))
            attachmentErrorMessage.Append("Embed description: ").AppendLine(Description);

        if (Thumbnail != null)
            attachmentErrorMessage.Append(Thumbnail.ToTxtFileContent());

        if (Image != null)
            attachmentErrorMessage.Append(Image.ToTxtFileContent());

        if (Footer != null)
            attachmentErrorMessage.Append(Footer.ToTxtFileContent());

        if (Fields?.Length > 0)
        {
            int fieldIndex = 1;

            foreach (var field in Fields)
            {
                attachmentErrorMessage.Append("- Embed field #").AppendLine(fieldIndex.ToString());
                attachmentErrorMessage.Append(field.ToTxtFileContent());

                fieldIndex++;
            }
        }

        return attachmentErrorMessage.ToString();
    }
}

/// <summary>
/// Embed author object
/// </summary>
public class DiscordMessageEmbedAuthor : BaseDiscordMessage
{
    /// <summary>
    /// Name of author
    /// </summary>
    [JsonPropertyName("name")]
    public string Name { get; }

    /// <summary>
    /// Url of author. If name was used, it becomes a hyperlink
    /// </summary>
    [JsonPropertyName("url")]
    public string Url { get; }

    /// <summary>
    /// Url of author icon
    /// </summary>
    [JsonPropertyName("icon_url")]
    public string IconUrl { get; }

    [JsonConstructor]
    private DiscordMessageEmbedAuthor()
    {
    }

    public DiscordMessageEmbedAuthor(string name, string url = null, string iconUrl = null)
    {
        Name = name?.Trim();
        Url = url?.Trim();
        IconUrl = iconUrl?.Trim();

        this.NotificarSeNuloOuVazio(Name, "The embed author \"name\" cannot be null or empty.")
            .NotificarSeVerdadeiro(Name?.Length > 256, $"The embed author \"name\" length limit is 256 characters (actual lenght is {Name?.Length}))");
    }

    public string ToTxtFileContent()
    {
        var attachmentErrorMessage = new StringBuilder();

        if (!string.IsNullOrEmpty(Name))
            attachmentErrorMessage.Append("Embed author name: ").AppendLine(Name);

        if (!string.IsNullOrEmpty(Url))
            attachmentErrorMessage.Append("Embed author URL: ").AppendLine(Url);

        if (!string.IsNullOrEmpty(IconUrl))
            attachmentErrorMessage.Append("Embed author icon URL: ").AppendLine(IconUrl);

        return attachmentErrorMessage.ToString();
    }
}

/// <summary>
/// Embed field objects
/// </summary>
public class DiscordMessageEmbedField : BaseDiscordMessage
{
    /// <summary>
    /// The name of the field
    /// </summary>
    [JsonPropertyName("name")]
    public string Name { get; }

    /// <summary>
    /// The value of the field
    /// </summary>
    [JsonPropertyName("value")]
    public string Value { get; }

    /// <summary>
    /// If true, fields will be displayed in the same line, but there can only be 3 max in the same line or 2 max if you used thumbnail
    /// </summary>
    [JsonPropertyName("inline")]
    public bool InLine { get; }

    [JsonConstructor]
    private DiscordMessageEmbedField()
    {
    }

    public DiscordMessageEmbedField(string name, string value = null, bool inLine = false)
    {
        Name   = name;
        Value  = value;
        InLine = inLine;

        this.NotificarSeNuloOuVazio(Name, "The embed field \"name\" cannot be null or empty.")
            .NotificarSeVerdadeiro(Name?.Length > 256, $"The embed field \"name\" length limit is 256 characters (actual lenght is {Name?.Length}))")
            .NotificarSeVerdadeiro(Value?.Length > 1024, $"The embed field \"value\" length limit is 1024 characters (actual lenght is {Value?.Length}))");
    }

    public string ToTxtFileContent()
    {
        var attachmentErrorMessage = new StringBuilder();

        if (!string.IsNullOrEmpty(Name))
            attachmentErrorMessage.Append("Embed field name: ").AppendLine(Name);

        if (!string.IsNullOrEmpty(Value))
            attachmentErrorMessage.Append("Embed field value: ").AppendLine(Value);

        attachmentErrorMessage.Append("Embed field inline: ").AppendLine(InLine.ToString());

        return attachmentErrorMessage.ToString();
    }
}

/// <summary>
/// Embed thumbnail object
/// </summary>
public class DiscordMessageEmbedThumbnail : BaseDiscordMessage
{
    /// <summary>
    /// url of thumbnail
    /// </summary>
    [JsonPropertyName("url")]
    public string Url { get; }

    [JsonConstructor]
    private DiscordMessageEmbedThumbnail()
    {
    }

    public DiscordMessageEmbedThumbnail(string url)
    {
        Url = url?.Trim();

        this.NotificarSeNuloOuVazio(Url, "The embed thumbnail \"url\" cannot be null or empty.");
    }

    public string ToTxtFileContent()
    {
        var attachmentErrorMessage = new StringBuilder();

        if (!string.IsNullOrEmpty(Url))
            attachmentErrorMessage.Append("Embed thumbnail URL: ").AppendLine(Url);

        return attachmentErrorMessage.ToString();
    }
}

/// <summary>
/// Embed image object
/// </summary>
public class DiscordMessageEmbedImage : BaseDiscordMessage
{
    /// <summary>
    /// url of image
    /// </summary>
    [JsonPropertyName("url")]
    public string Url { get; }

    [JsonConstructor]
    private DiscordMessageEmbedImage()
    {
    }

    public DiscordMessageEmbedImage(string url)
    {
        Url = url?.Trim();

        this.NotificarSeNuloOuVazio(Url, "The embed image \"url\" cannot be null or empty.");
    }

    public string ToTxtFileContent()
    {
        var attachmentErrorMessage = new StringBuilder();

        if (!string.IsNullOrEmpty(Url))
            attachmentErrorMessage.Append("Embed image URL: ").AppendLine(Url);

        return attachmentErrorMessage.ToString();
    }
}

/// <summary>
/// Embed footer object
/// </summary>
public class DiscordMessageEmbedFooter : BaseDiscordMessage
{
    /// <summary>
    /// Footer text, doesn't support Markdown
    /// </summary>
    [JsonPropertyName("text")]
    public string Text { get; }

    /// <summary>
    /// Url of footer icon
    /// </summary>
    [JsonPropertyName("icon_url")]
    public string IconUrl { get; }

    [JsonConstructor]
    private DiscordMessageEmbedFooter()
    {
    }

    public DiscordMessageEmbedFooter(string text, string iconUrl = null)
    {
        Text = text?.Trim();
        IconUrl = iconUrl?.Trim();

        this.NotificarSeNuloOuVazio(Text, "The embed footer \"text\" cannot be null or empty.")
            .NotificarSeVerdadeiro(Text?.Length > 2048, $"The embed footer \"text\" length limit is 2048 characters (actual lenght is {Text?.Length}))");
    }

    public string ToTxtFileContent()
    {
        var attachmentErrorMessage = new StringBuilder();

        if (!string.IsNullOrEmpty(Text))
            attachmentErrorMessage.Append("Embed footer text: ").AppendLine(Text);

        if (!string.IsNullOrEmpty(IconUrl))
            attachmentErrorMessage.Append("Embed icon URL: ").AppendLine(IconUrl);

        return attachmentErrorMessage.ToString();
    }
}
