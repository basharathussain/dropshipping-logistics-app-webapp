using System;
using System.Threading;
using System.Threading.Tasks;
using Logistics.Application.Abstractions.Notifications;
using Logistics.Domain.Primitives.Enums;

namespace Logistics.TelegramBot.Services;

/// <summary>
/// No-op notification service used when the Telegram bot is not configured
/// (missing or placeholder token). Keeps DI valid for the many event handlers
/// that depend on <see cref="ITelegramNotificationService"/> without sending anything.
/// </summary>
internal sealed class NoOpTelegramNotificationService : ITelegramNotificationService
{
    public Task SendNotificationAsync(
        Guid tenantId,
        string title,
        string message,
        TelegramChatRole? targetRole = null,
        CancellationToken ct = default)
        => Task.CompletedTask;
}
