using System.Text.Json;
using Logistics.Application.Abstractions;
using Logistics.Domain.Entities;
using Logistics.Domain.Persistence;
using Logistics.Shared.Models;
using Microsoft.EntityFrameworkCore;

namespace Logistics.Application.Modules.Operations.Loads.Queries;

internal sealed class GetLoadAuditLogsHandler(ITenantUnitOfWork tenantUow)
    : IAppRequestHandler<GetLoadAuditLogsQuery, Result<List<LoadAuditLogDto>>>
{
    private static readonly JsonSerializerOptions JsonOptions = new() { PropertyNameCaseInsensitive = true };

    public async Task<Result<List<LoadAuditLogDto>>> Handle(GetLoadAuditLogsQuery req, CancellationToken ct)
    {
        var logs = await tenantUow.Repository<LoadAuditLog>().Query()
            .Where(x => x.LoadId == req.LoadId)
            .OrderByDescending(x => x.ChangedAt)
            .ToListAsync(ct);

        var dtos = logs.Select(l => new LoadAuditLogDto
        {
            Id = l.Id,
            LoadId = l.LoadId,
            Action = l.Action,
            ChangedBy = l.ChangedBy,
            ChangedByName = l.ChangedByName,
            ChangedAt = l.ChangedAt,
            Changes = Deserialize(l.Changes)
        }).ToList();

        return Result<List<LoadAuditLogDto>>.Ok(dtos);
    }

    private static List<LoadAuditChangeDto> Deserialize(string json)
    {
        try
        {
            return JsonSerializer.Deserialize<List<LoadAuditChangeDto>>(json, JsonOptions) ?? [];
        }
        catch (JsonException)
        {
            return [];
        }
    }
}
