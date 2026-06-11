using System.Linq;
using Logistics.TelegramBot.Authentication;
using Logistics.TelegramBot.Commands;
using Logistics.TelegramBot.Handlers;
using Logistics.TelegramBot.Options;
using Logistics.TelegramBot.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Telegram.Bot;
using Logistics.Application.Abstractions.Notifications;

namespace Logistics.TelegramBot;

public static class Registrar
{
    /// <summary>
    ///   Add Telegram Bot services and handlers
    /// </summary>
    /// <param name="services">The service collection</param>
    /// <param name="configuration">The configuration</param>
    /// <param name="isDevelopment">If true, uses long polling instead of webhook (for local development)</param>
    public static IServiceCollection AddTelegramBotInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration, bool isDevelopment = false)
    {
        var section = configuration.GetSection(TelegramBotOptions.SectionName);
        services.Configure<TelegramBotOptions>(section);

        var botOptions = section.Get<TelegramBotOptions>();
        // Skip if missing OR an unreplaced placeholder (e.g. "<Telegram bot token>").
        // Real tokens look like "<digits>:<hash>"; an invalid value would make
        // TelegramBotClient throw and crash app startup. Still register a no-op
        // notifier so the many handlers that depend on ITelegramNotificationService
        // can be resolved.
        if (string.IsNullOrEmpty(botOptions?.BotToken) || !IsValidBotToken(botOptions.BotToken))
        {
            services.AddSingleton<ITelegramNotificationService, NoOpTelegramNotificationService>();
            return services;
        }

        // Telegram bot client (singleton - thread-safe)
        services.AddSingleton<ITelegramBotClient>(new TelegramBotClient(botOptions.BotToken));

        // Authentication
        services.AddScoped<TelegramAuthService>();

        // Handlers
        services.AddScoped<TelegramUpdateDispatcher>();
        services.AddScoped<CommandRouter>();
        services.AddScoped<CallbackQueryHandler>();

        // Commands
        services.AddScoped<ITelegramCommand, StartCommand>();
        services.AddScoped<ITelegramCommand, DisconnectCommand>();
        services.AddScoped<ITelegramCommand, LoadsCommand>();
        services.AddScoped<ITelegramCommand, TrucksCommand>();
        services.AddScoped<ITelegramCommand, TripsCommand>();
        services.AddScoped<ITelegramCommand, HosCommand>();
        services.AddScoped<ITelegramCommand, NotifyCommand>();
        services.AddScoped<ITelegramCommand, HelpCommand>();

        // Services
        services.AddSingleton<ITelegramNotificationService, TelegramNotificationService>();

        // Background services
        services.AddHostedService<TelegramChatCacheWarmer>();

        // Development mode: use long polling
        if (isDevelopment)
        {
            services.AddHostedService<TelegramPollingService>();
        }

        return services;
    }

    /// <summary>Real Telegram bot tokens are "&lt;botId&gt;:&lt;hash&gt;" with a numeric bot id.</summary>
    private static bool IsValidBotToken(string token)
    {
        var colon = token.IndexOf(':');
        return colon > 0 && token[..colon].All(char.IsDigit);
    }

    public static WebApplication MapTelegramWebhook(this WebApplication app)
    {
        var options = app.Services.GetRequiredService<IOptions<TelegramBotOptions>>().Value;

        // Skip if the bot isn't really configured (missing or placeholder token).
        // In Production with a WebhookUrl set, this method resolves ITelegramBotClient,
        // which AddTelegramBotInfrastructure only registers for a valid token — so the
        // guard here must match, or the API crashes on startup.
        if (string.IsNullOrEmpty(options.BotToken) || !IsValidBotToken(options.BotToken))
            return app;

        // Production mode: register webhook with Telegram
        if (app.Environment.IsProduction() && !string.IsNullOrEmpty(options.WebhookUrl))
        {
            var capturedOptions = options;
            app.MapPost("/webhooks/telegram",
                (HttpContext context, IServiceScopeFactory scopeFactory) =>
                    TelegramWebhookHandler.HandleAsync(context, scopeFactory, capturedOptions))
                .AllowAnonymous();

            // Set webhook on startup
            var bot = app.Services.GetRequiredService<ITelegramBotClient>();
            _ = bot.SetWebhook(
                options.WebhookUrl,
                secretToken: options.SecretToken);

            // Delete webhook on shutdown
            var lifetime = app.Services.GetRequiredService<IHostApplicationLifetime>();
            lifetime.ApplicationStopping.Register(() =>
            {
                bot.DeleteWebhook().GetAwaiter().GetResult();
            });
        }

        return app;
    }
}
