using JNogueira.NotifiqueMe;

namespace JNogueira.Discord.WebhookClient;

public class DiscordFile : Notificavel
{
    /// <summary>
    /// File name
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// File content
    /// </summary>
    public byte[] Content { get; }

    public DiscordFile(string name, byte[] content)
    {
        Name = name;
        Content = content;

        this.NotificarSeNuloOuVazio(Name, "The file \"name\" cannot be null or empty.")
            .NotificarSeVerdadeiro(Content == null || Content.Length == 0, "The file \"content\" cannot be null or empty.");
    }
}
