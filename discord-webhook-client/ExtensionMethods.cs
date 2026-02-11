using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace JNogueira.Discord.WebhookClient;

public static class ExtensionMethods
{
    public static IServiceCollection AddDiscordWebhookClient(this IServiceCollection services, IConfiguration configuration, ServiceLifetime serviceLifetime = ServiceLifetime.Scoped)
    {
        services.AddHttpClient<DiscordWebhookHttpClient>(nameof(DiscordWebhookHttpClient), client =>
        {
            client.BaseAddress = !string.IsNullOrEmpty(configuration["DiscordWebhookUrl"])
                ? new Uri(configuration["DiscordWebhookUrl"])
                : throw new ArgumentNullException("DiscordWebhookUrl", "Base URL for Discord Webhook is not configured.");
            client.Timeout = TimeSpan.FromSeconds(30);
        });

        switch (serviceLifetime)
        {
            case ServiceLifetime.Singleton:
                services.AddSingleton<DiscordWebhookClient>();
                break;
            case ServiceLifetime.Transient:
                services.AddTransient<DiscordWebhookClient>();
                break;
            default:
                services.AddScoped<DiscordWebhookClient>();
                break;
        }

        return services;
    }

    public static IServiceCollection AddDiscordWebhookClient(this IServiceCollection services, string urlWebhook, ServiceLifetime serviceLifetime = ServiceLifetime.Scoped)
    {
        services.AddHttpClient<DiscordWebhookHttpClient>(nameof(DiscordWebhookHttpClient), client =>
        {
            client.BaseAddress = !string.IsNullOrEmpty(urlWebhook)
                ? new Uri(urlWebhook)
                : throw new ArgumentNullException(nameof(urlWebhook), "Base URL for Discord Webhook is not configured.");
            client.Timeout = TimeSpan.FromSeconds(30);
        });

        switch (serviceLifetime)
        {
            case ServiceLifetime.Singleton:
                services.AddSingleton<DiscordWebhookClient>();
                break;
            case ServiceLifetime.Transient:
                services.AddTransient<DiscordWebhookClient>();
                break;
            default:
                services.AddScoped<DiscordWebhookClient>();
                break;
        }

        return services;
    }
}
