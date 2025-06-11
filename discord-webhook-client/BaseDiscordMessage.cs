using JNogueira.NotifiqueMe;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;

namespace JNogueira.Discord.WebhookClient;

public abstract class BaseDiscordMessage : INotificavel
{
    private readonly List<Notificacao> _notificacoes = [];

    [JsonIgnore]
    public bool IsInvalid => Invalido;

    [JsonIgnore]
    public bool Invalido => _notificacoes.Any();

    [JsonIgnore]
    public IReadOnlyCollection<string> Mensagens
    {
        get
        {
            return !_notificacoes.Any()
                ? []
                : (IReadOnlyCollection<string>)_notificacoes.ConvertAll(x => x.Mensagem);
        }
    }

    [JsonIgnore]
    public IReadOnlyCollection<Notificacao> Notificacoes => _notificacoes;

    public void AdicionarNotificacao(string mensagem) => _notificacoes.Add(new Notificacao(mensagem));

    public void AdicionarNotificacao(string mensagem, Dictionary<string, string> informacoesAdicionais) => _notificacoes.Add(new Notificacao(mensagem, informacoesAdicionais));

    public void AdicionarNotificacao(Notificacao notificacao)
    {
        if (notificacao is not null)
        {
            _notificacoes.Add(notificacao);
        }
    }

    public void AdicionarNotificacoes(IReadOnlyCollection<Notificacao> notificacoes)
    {
        if (notificacoes?.Any() == true)
        {
            _notificacoes.AddRange(notificacoes);
        }
    }

    public void AdicionarNotificacoes(Notificavel notificavel)
    {
        if (notificavel is not null)
        {
            _notificacoes.AddRange(notificavel.Notificacoes);
        }
    }

    public void AdicionarNotificacoes(INotificavel notificavel)
    {
        if (notificavel is not null)
        {
            _notificacoes.AddRange(notificavel.Notificacoes);
        }
    }

    public void AdicionarNotificacoes(params Notificavel[] notificaveis)
    {
        if (notificaveis?.Length > 0)
        {
            foreach (Notificavel notificavel in notificaveis)
            {
                AdicionarNotificacoes(notificavel);
            }
        }
    }
}
