using Logistics.Application.Abstractions;
using Logistics.Shared.Models;

namespace Logistics.Application.Modules.Operations.Loads.Queries;

/// <summary>
/// Returns the full audit trail for a load (creation + every update), newest first.
/// </summary>
public sealed record GetLoadAuditLogsQuery(Guid LoadId) : IQuery<Result<List<LoadAuditLogDto>>>;
