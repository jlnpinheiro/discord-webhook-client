using JNogueira.NotifiqueMe;

namespace JNogueira.Discord.Webhook.Client
{
    public class DiscordFile : Notificavel
    {
        /// <summary>
        /// File name
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// File content
        /// </summary>
        public byte[] Content { get; private set; }

        public DiscordFile(string name, byte[] content)
        {
            Name = name;
            Content = content;

            Validate();
        }
        internal void Validate()
        {
            this
                .NotificarSeNuloOuVazio(this.Name, "The file \"name\" cannot be null or empty.")
                .NotificarSeVerdadeiro(this.Content == null || this.Content.Length == 0, "The file \"content\" cannot be null or empty.");
        }
    }
}
