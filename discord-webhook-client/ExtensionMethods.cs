using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace JNogueira.Discord.WebhookClient;

public static class ExtensionMethods
{
    public static IServiceCollection AddDiscordWebhookClient(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddHttpClient<DiscordWebhookHttpClient>(nameof(DiscordWebhookHttpClient), client =>
        {
            client.BaseAddress = !string.IsNullOrEmpty(configuration["DiscordWebhookUrl"])
                ? new Uri(configuration["DiscordWebhookUrl"])
                : throw new ArgumentNullException("DiscordWebhookUrl", "Base URL for Discord Webhook is not configured.");
            client.Timeout = TimeSpan.FromSeconds(30);
        });

        services.AddScoped<DiscordWebhookClient>();

        return services;
    }

    public static IServiceCollection AddDiscordWebhookClient(this IServiceCollection services, string urlWebhook)
    {
        services.AddHttpClient<DiscordWebhookHttpClient>(nameof(DiscordWebhookHttpClient), client =>
        {
            client.BaseAddress = !string.IsNullOrEmpty(urlWebhook)
                ? new Uri(urlWebhook)
                : throw new ArgumentNullException(nameof(urlWebhook), "Base URL for Discord Webhook is not configured.");
            client.Timeout = TimeSpan.FromSeconds(30);
        });

        services.AddScoped<DiscordWebhookClient>();

        return services;
    }
}
