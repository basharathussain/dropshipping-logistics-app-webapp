using Logistics.Domain.Primitives.Enums;
using Logistics.Infrastructure.AI.Options;
using Microsoft.Extensions.Options;

namespace Logistics.Infrastructure.AI.Providers;

/// <summary>
/// Creates <see cref="ILlmProvider"/> instances based on the configured or requested provider type.
/// Anthropic uses its native SDK; all other providers use the OpenAI-compatible SDK.
/// </summary>
internal sealed class LlmProviderFactory(IOptions<LlmOptions> options)
{
    public ILlmProvider Create(LlmProvider? providerOverride = null)
    {
        var type = providerOverride ?? options.Value.DefaultProvider;
        return Create(type, options.Value.GetProviderConfig(type));
    }

    /// <summary>Creates a provider using an explicit config (e.g. with an admin-set API key override).</summary>
    public ILlmProvider Create(LlmProvider type, LlmProviderOptions config) =>
        type switch
        {
            LlmProvider.Anthropic => new AnthropicLlmProvider(config),
            _ => new OpenAiLlmProvider(config)
        };
}
