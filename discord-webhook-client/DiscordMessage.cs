using JNogueira.NotifiqueMe;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace JNogueira.Discord.WebhookClient;

public class DiscordMessage : BaseDiscordMessage
{
    /// <summary>
    /// Overrides the current username of the webhook
    /// </summary>
    [JsonPropertyName("username")]
    public string Username { get; }

    /// <summary>
    /// Overrides the default avatar of the webhook
    /// </summary>
    [JsonPropertyName("avatar_url")]
    public string AvatarUrl { get; }

    /// <summary>
    /// Simple message, the message contains (up to 2000 characters)
    /// </summary>
    [JsonPropertyName("content")]
    public string Content { get; }

    /// <summary>
    /// If true, the message will be pronounced in chat like tts message
    /// </summary>
    [JsonPropertyName("tts")]
    public bool Tts { get; }

    /// <summary>
    /// An array of embed objects. That means you put use more than one in the same body
    /// </summary>
    [JsonPropertyName("embeds")]
    public DiscordMessageEmbed[] Embeds { get; }

    [JsonConstructor]
    private DiscordMessage()
    {
    }

    public DiscordMessage(
        string content = null,
        string username = null,
        string avatarUrl = null,
        bool tts = false,
        DiscordMessageEmbed[] embeds = null)
    {
        Content = content?.Trim();
        Username = username?.Trim();
        AvatarUrl = avatarUrl?.Trim();
        Tts = tts;
        Embeds = embeds?.Where(x => x is not null)?.ToArray();

        this.NotificarSeVerdadeiro(!string.IsNullOrEmpty(Content) && Content?.Length > 2000, $"The \"content\" element length limit is 2000 characters (actual lenght is {Content?.Length}).");

        if (Embeds?.Length > 0)
        {
            this.NotificarSeVerdadeiro(Embeds?.Length > 10, $"The \"embeds\" collection size limit is 10 elements (actual size is {Embeds.Length}).");

            Embeds.ToList()
                .ForEach(x =>
                {
                    if (x?.Invalido == true)
                        AdicionarNotificacoes(x.Notificacoes);
                });
        }
    }

    public int GetMessageCaractersSize()
    {
        var sumCaractersInMessage = new StringBuilder();

        sumCaractersInMessage.Append(Embeds.Select(x => x.Title).ToArray());

        sumCaractersInMessage.Append(Embeds.Select(x => x.Description).ToArray());

        sumCaractersInMessage.Append((from embed in Embeds
                                      from field in embed.Fields
                                      select field.Name).ToArray());

        sumCaractersInMessage.Append((from embed in Embeds
                                      from field in embed.Fields
                                      select field.Value).ToArray());

        sumCaractersInMessage.Append(Embeds.Select(x => x.Footer.Text).ToArray());

        sumCaractersInMessage.Append(Embeds.Select(x => x.Author.Name).ToArray());

        return sumCaractersInMessage.Length;
    }

    public string ToJson() => this is null
        ? string.Empty
        : JsonSerializer.Serialize(this, new JsonSerializerOptions { WriteIndented = true, DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull });

    public string ToTxtFileContent()
    {
        if (this is null)
            return string.Empty;

        var attachmentErrorMessage = new StringBuilder();

        attachmentErrorMessage.AppendLine("####### MESSAGE #######");

        if (!string.IsNullOrEmpty(Username))
        {
            attachmentErrorMessage.Append("Message username: ").AppendLine(Username);
        }

        if (!string.IsNullOrEmpty(AvatarUrl))
        {
            attachmentErrorMessage.Append("Message avatar URL: ").AppendLine(AvatarUrl);
        }

        attachmentErrorMessage.Append("Message TTS: ").AppendLine(Tts.ToString());

        if (!string.IsNullOrEmpty(Content))
        {
            attachmentErrorMessage.AppendLine("Message content:");
            attachmentErrorMessage.AppendLine(Content);
        }

        if (Embeds?.Length > 0)
        {
            int embedIndex = 1;

            attachmentErrorMessage.AppendLine();
            attachmentErrorMessage.AppendLine("-- Message embeds --");
            attachmentErrorMessage.AppendLine();

            foreach (var embed in Embeds)
            {
                attachmentErrorMessage.Append("-- Embed #").AppendLine(embedIndex.ToString());
                attachmentErrorMessage.AppendLine(embed.ToTxtFileContent());

                embedIndex++;
            }
        }

        return attachmentErrorMessage.ToString();
    }
}