using Logistics.Application.Abstractions.Ai;
using Logistics.Application.Abstractions.AiDispatch;
using Logistics.Application.Abstractions.SystemSettings;
using Logistics.Domain.Primitives.Enums;
using Logistics.Infrastructure.AI.Models;
using Logistics.Infrastructure.AI.Options;
using Logistics.Infrastructure.AI.Providers;
using Logistics.Shared.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Logistics.Infrastructure.AI.Services;

/// <summary>
/// One-shot LLM entry point. Resolves the global model, sends a single tool-less request
/// (optionally with inline documents), and reports token usage and estimated cost.
/// </summary>
internal sealed class LlmClient(
    LlmModelResolver modelResolver,
    LlmProviderFactory providerFactory,
    ISystemSettingsService systemSettings,
    IOptions<LlmOptions> options,
    ILogger<LlmClient> logger) : ILlmClient
{
    public async Task<Result<LlmCompletionResult>> CompleteAsync(
        LlmCompletionRequest request,
        CancellationToken ct = default)
    {
        var config = options.Value;
        var selection = await modelResolver.ResolveAsync(config, ct);

        if (string.IsNullOrWhiteSpace(selection.ProviderConfig.ApiKey))
            return Result<LlmCompletionResult>.Fail($"LLM API key for provider '{selection.Provider}' is not configured.");

        var content = new List<LlmContentBlock> { new LlmTextBlock(request.UserText) };
        foreach (var document in request.Documents)
        {
            content.Add(new LlmDocumentBlock(document.MediaType, Convert.ToBase64String(document.Data)));
        }

        var llmRequest = new LlmRequest
        {
            SystemPrompt = request.SystemPrompt,
            Messages = [new LlmMessage(LlmRole.User, content)],
            Tools = [],
            Model = selection.Model,
            MaxTokens = request.MaxTokens,
            Temperature = 0m
        };

        try
        {
            var provider = providerFactory.Create(selection.Provider, selection.ProviderConfig);
            var response = await provider.SendAsync(llmRequest, ct);

            var usage = response.Usage;
            var cost = LlmPricing.Calculate(
                selection.Model, usage.InputTokens, usage.OutputTokens, usage.CacheReadTokens, usage.CacheCreationTokens);

            logger.LogInformation(
                "LLM completion: model {Model}, input {InputTokens} tok, output {OutputTokens} tok, est ${Cost:F4}",
                selection.Model, usage.InputTokens, usage.OutputTokens, cost);

            return Result<LlmCompletionResult>.Ok(new LlmCompletionResult(
                response.TextContent ?? string.Empty,
                selection.Model,
                usage.InputTokens,
                usage.OutputTokens,
                cost));
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "LLM completion failed for model {Model}", selection.Model);
            return Result<LlmCompletionResult>.Fail($"LLM request failed: {ex.Message}");
        }
    }

    public async Task<Result<LlmConnectionTestResult>> TestConnectionAsync(
        string? modelId, string? apiKey, CancellationToken ct = default)
    {
        var config = options.Value;

        LlmProvider provider;
        string model;
        LlmProviderOptions baseConfig;

        // If a specific model was requested, derive its provider from the catalog; otherwise resolve the
        // globally configured model.
        var info = LlmModelCatalog.Find(modelId);
        if (info is not null)
        {
            provider = info.Provider;
            model = info.Id;
            baseConfig = config.GetProviderConfig(provider);
        }
        else
        {
            var selection = await modelResolver.ResolveAsync(config, ct);
            provider = selection.Provider;
            model = selection.Model;
            baseConfig = selection.ProviderConfig;
        }

        // Key precedence: explicit (typed in the form) → saved admin key → appsettings/env.
        var key = apiKey?.Trim();
        if (string.IsNullOrWhiteSpace(key))
            key = await systemSettings.GetAsync(AiSettingsKeys.ApiKeyFor(provider), ct);
        if (string.IsNullOrWhiteSpace(key))
            key = baseConfig.ApiKey;

        if (string.IsNullOrWhiteSpace(key))
            return Result<LlmConnectionTestResult>.Ok(
                new LlmConnectionTestResult(false, $"No API key configured for provider '{provider}'.", model));

        var testConfig = new LlmProviderOptions { ApiKey = key, Model = model, BaseUrl = baseConfig.BaseUrl };

        var llmRequest = new LlmRequest
        {
            SystemPrompt = "You are a connectivity test. Reply with the single word: OK.",
            Messages = [new LlmMessage(LlmRole.User, [new LlmTextBlock("ping")])],
            Tools = [],
            Model = model,
            MaxTokens = 5,
            Temperature = 0m
        };

        try
        {
            var provider2 = providerFactory.Create(provider, testConfig);
            await provider2.SendAsync(llmRequest, ct);
            return Result<LlmConnectionTestResult>.Ok(
                new LlmConnectionTestResult(true, $"Connection successful ({model}).", model));
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "AI key test failed for model {Model}", model);
            var msg = ex.Message.Length > 200 ? ex.Message[..200] : ex.Message;
            return Result<LlmConnectionTestResult>.Ok(new LlmConnectionTestResult(false, msg, model));
        }
    }
}
