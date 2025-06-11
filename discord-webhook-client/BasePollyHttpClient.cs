using Polly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace JNogueira.Discord.WebhookClient;

public abstract class BasePollyHttpClient(HttpClient client)
{
    public HttpClient Client { get; } = client ?? throw new ArgumentNullException(nameof(client), "HttpClient cannot be null.");

    public async Task<TResponse> Post<TResponse>(string url, HttpContent bodyContent, Action<DelegateResult<HttpResponseMessage>, TimeSpan, int, Context> onRetryAsync = null, TimeSpan[] sleepDurations = null, IEnumerable<HttpStatusCode> acceptablesHttpStatusCodes = null)
    {
        return await DeserializeHttpResponseMessage<TResponse>(await GetHttpResponseMessage(async () => await Client.PostAsync(url, bodyContent), sleepDurations, onRetryAsync, acceptablesHttpStatusCodes));
    }

    public async Task<HttpResponseMessage> Post(string url, HttpContent bodyContent, Action<DelegateResult<HttpResponseMessage>, TimeSpan, int, Context> onRetryAsync = null, TimeSpan[] sleepDurations = null, IEnumerable<HttpStatusCode> acceptablesHttpStatusCodes = null)
    {
        return await Post<HttpResponseMessage>(url, bodyContent, onRetryAsync, sleepDurations, acceptablesHttpStatusCodes);
    }

    public async Task<TResponse> Get<TResponse>(string url, Action<DelegateResult<HttpResponseMessage>, TimeSpan, int, Context> onRetryAsync = null, TimeSpan[] sleepDurations = null, StringContent bodyContent = null, IEnumerable<HttpStatusCode> acceptablesHttpStatusCodes = null)
    {
        var response = bodyContent is null
            ? await GetHttpResponseMessage(async () => await Client.GetAsync(url), sleepDurations, onRetryAsync, acceptablesHttpStatusCodes)
            : await GetHttpResponseMessage(async () => await Client.SendAsync(CreateHttpRequestMessage(url, bodyContent, method: HttpMethod.Get)), sleepDurations, onRetryAsync, acceptablesHttpStatusCodes);

        return await DeserializeHttpResponseMessage<TResponse>(response);
    }

    public async Task<HttpResponseMessage> Get(string url, Action<DelegateResult<HttpResponseMessage>, TimeSpan, int, Context> onRetryAsync = null, TimeSpan[] sleepDurations = null, StringContent bodyContent = null, IEnumerable<HttpStatusCode> acceptablesHttpStatusCodes = null)
    {
        return await Get<HttpResponseMessage>(url, onRetryAsync, sleepDurations, bodyContent, acceptablesHttpStatusCodes);
    }

    private static async Task<HttpResponseMessage> GetHttpResponseMessage(Func<Task<HttpResponseMessage>> requestAction, TimeSpan[] sleepDurations = null, Action<DelegateResult<HttpResponseMessage>, TimeSpan, int, Context> onRetryAsync = null, IEnumerable<HttpStatusCode> acceptablesHttpStatusCodes = null)
    {
        try
        {
            acceptablesHttpStatusCodes ??= [HttpStatusCode.NotFound];

            var policyBuilder = Policy.HandleResult<HttpResponseMessage>(x => !x.IsSuccessStatusCode && !acceptablesHttpStatusCodes.Contains(x.StatusCode));

            var retryPolicy = onRetryAsync != null
                ? policyBuilder.WaitAndRetryAsync(sleepDurations ?? [TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(2), TimeSpan.FromSeconds(3)], onRetryAsync)
                : policyBuilder.WaitAndRetryAsync(sleepDurations ?? [TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(2), TimeSpan.FromSeconds(3)]);

            return await retryPolicy.ExecuteAsync(requestAction);
        }
        catch (Exception ex)
        {
            throw new BasePollyHttpClientException(ex.Message, ex);
        }
    }

    public static async Task<TRetorno> DeserializeHttpResponseMessage<TRetorno>(HttpResponseMessage response)
    {
        if (response is null)
            throw new ArgumentNullException(nameof(response), "O parâmetro response não pode ser nulo.");

        try
        {
            if (typeof(TRetorno) == typeof(HttpResponseMessage))
                return (TRetorno)Convert.ChangeType(response, typeof(TRetorno));

            await using var stream = await response.Content.ReadAsStreamAsync();

            return await JsonSerializer.DeserializeAsync<TRetorno>(stream, new JsonSerializerOptions { AllowTrailingCommas = true, PropertyNameCaseInsensitive = true });
        }
        catch (JsonException ex)
        {
            var pollyException = new BasePollyHttpClientException($"Failed to deserialize returned JSON into {typeof(TRetorno).Name} type: {ex.GetBaseException().Message}", ex);

            pollyException.Data.Add("ResponseContent", await response?.Content?.ReadAsStringAsync());

            throw pollyException;
        }
        catch (Exception ex)
        {
            var pollyException = new BasePollyHttpClientException($"Request failed to {response?.RequestMessage?.RequestUri}: {ex.GetBaseException().Message}", ex);

            pollyException.Data.Add("ResponseContent", await response?.Content?.ReadAsStringAsync());

            throw pollyException;
        }
    }

    private HttpRequestMessage CreateHttpRequestMessage(string url, StringContent bodyContent, HttpMethod method) => new()
    {
        Method = method,
        RequestUri = new Uri(Client.BaseAddress, url),
        Content = bodyContent
    };
}

[Serializable]
public class BasePollyHttpClientException : Exception
{
    public BasePollyHttpClientException() { }
    public BasePollyHttpClientException(string message) : base(message) { }
    public BasePollyHttpClientException(string message, Exception inner) : base(message, inner) { }
    protected BasePollyHttpClientException(
      System.Runtime.Serialization.SerializationInfo info,
      System.Runtime.Serialization.StreamingContext context) { }
}