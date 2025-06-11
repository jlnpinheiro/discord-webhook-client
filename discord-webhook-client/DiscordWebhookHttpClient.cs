using Polly;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace JNogueira.Discord.WebhookClient;

public class DiscordWebhookHttpClient : BasePollyHttpClient
{
    public DiscordWebhookHttpClient(HttpClient client) : base(client)
    {
        Client.Timeout = TimeSpan.FromSeconds(30);
    }

    public async Task<HttpResponseMessage> Post(HttpContent bodyContent, Action<DelegateResult<HttpResponseMessage>, TimeSpan, int, Context> onRetryAsync = null, TimeSpan[] sleepDurations = null, IEnumerable<HttpStatusCode> acceptablesHttpStatusCodes = null)
    {
        return await base.Post<HttpResponseMessage>(Client.BaseAddress.ToString(), bodyContent, onRetryAsync, sleepDurations, acceptablesHttpStatusCodes);
    }
}