using Logistics.Application.Abstractions;
using Logistics.Shared.Models;

namespace Logistics.Application.Modules.Platform.AiSettings.Commands;

/// <summary>
/// Sets the platform-wide AI dispatch model and per-plan weekly quotas. Admin only.
/// </summary>
public sealed class UpdateAiSettingsCommand : IMasterCommand
{
    /// <summary>The globally selected model id (must exist in the model catalog).</summary>
    public string Model { get; set; } = string.Empty;

    /// <summary>Whether extended thinking is enabled. Only honored by models that support it.</summary>
    public bool ExtendedThinking { get; set; }

    /// <summary>
    /// API key for the selected model's provider; overrides the appsettings/env key. When null/blank or
    /// the masked placeholder echoed by the UI, the existing key is left unchanged.
    /// </summary>
    public string? ApiKey { get; set; }

    /// <summary>Per-plan weekly quota updates. A null quota means unlimited.</summary>
    public List<PlanQuotaDto> Plans { get; set; } = [];
}
