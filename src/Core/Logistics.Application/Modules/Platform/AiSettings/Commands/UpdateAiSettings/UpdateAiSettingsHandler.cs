using Logistics.Application.Abstractions;
using Logistics.Application.Abstractions.AiDispatch;
using Logistics.Application.Abstractions.SystemSettings;
using Logistics.Domain.Entities;
using Logistics.Domain.Persistence;
using Logistics.Shared.Models;

namespace Logistics.Application.Modules.Platform.AiSettings.Commands;

internal sealed class UpdateAiSettingsHandler(
    ISystemSettingsService systemSettings,
    IMasterUnitOfWork masterUow) : IAppRequestHandler<UpdateAiSettingsCommand, Result>
{
    public async Task<Result> Handle(UpdateAiSettingsCommand req, CancellationToken ct)
    {
        var modelInfo = LlmModelCatalog.Find(req.Model);
        if (modelInfo is null)
            return Result.Fail($"Unknown AI model '{req.Model}'.");

        // Persist the global model selection (provider is derived from the model via the catalog).
        await systemSettings.SetAsync(AiSettingsKeys.Model, modelInfo.Id,
            "Platform-wide AI dispatch model", ct);
        await systemSettings.SetAsync(AiSettingsKeys.ExtendedThinking, req.ExtendedThinking.ToString(),
            "Whether extended thinking is enabled for the dispatch agent", ct);

        // Persist the API key for the selected model's provider (overrides appsettings/env).
        // Ignore blanks and the masked placeholder the UI may echo back.
        var apiKey = req.ApiKey?.Trim();
        if (!string.IsNullOrWhiteSpace(apiKey) && !apiKey.Contains('\u2022'))
        {
            await systemSettings.SetAsync(AiSettingsKeys.ApiKeyFor(modelInfo.Provider), apiKey,
                $"AI dispatch API key for provider {modelInfo.Provider}", ct);
        }

        // Update per-plan weekly quotas (null = unlimited).
        var planRepo = masterUow.Repository<SubscriptionPlan>();
        var changed = false;
        foreach (var planUpdate in req.Plans)
        {
            var plan = await planRepo.GetByIdAsync(planUpdate.PlanId, ct);
            if (plan is null || plan.WeeklyAiRequestQuota == planUpdate.WeeklyAiRequestQuota)
                continue;

            plan.WeeklyAiRequestQuota = planUpdate.WeeklyAiRequestQuota;
            planRepo.Update(plan);
            changed = true;
        }

        if (changed)
            await masterUow.SaveChangesAsync(ct);

        return Result.Ok();
    }
}
