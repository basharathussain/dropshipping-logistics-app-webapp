namespace Logistics.Shared.Models;

/// <summary>Result of an admin AI API-key validation test (a minimal live call to the provider).</summary>
public record AiKeyTestResultDto
{
    /// <summary>Whether the key authenticated and the model responded.</summary>
    public bool Valid { get; set; }

    /// <summary>A human-readable status / error message.</summary>
    public string Message { get; set; } = string.Empty;

    /// <summary>The model id that was tested.</summary>
    public string? Model { get; set; }
}
