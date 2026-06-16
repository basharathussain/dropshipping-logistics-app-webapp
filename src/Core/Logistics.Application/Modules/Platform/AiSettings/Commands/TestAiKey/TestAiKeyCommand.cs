using Logistics.Application.Abstractions;
using Logistics.Shared.Models;

namespace Logistics.Application.Modules.Platform.AiSettings.Commands;

/// <summary>
/// Validates an LLM API key by making a minimal live call to the provider. Admin only.
/// When <see cref="ApiKey"/> is omitted, the saved/configured key for the model's provider is tested.
/// </summary>
public sealed class TestAiKeyCommand : IMasterCommand<Result<AiKeyTestResultDto>>
{
    /// <summary>The model id to test. Defaults to the globally selected model when null.</summary>
    public string? Model { get; set; }

    /// <summary>The API key to test. When null, the saved/env key for the model's provider is used.</summary>
    public string? ApiKey { get; set; }
}
