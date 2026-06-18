using Logistics.Domain.Core;

namespace Logistics.Domain.Entities;

/// <summary>
/// Immutable audit record for a Load: which fields changed (old -&gt; new), who changed them, and when.
/// One row per SaveChanges that touched a tracked Load field (creation included).
/// </summary>
public class LoadAuditLog : Entity, ITenantEntity
{
    public Guid LoadId { get; set; }

    /// <summary>"Created" or "Updated".</summary>
    public string Action { get; set; } = "Updated";

    /// <summary>User id of the editor (null for system/background changes).</summary>
    public string? ChangedBy { get; set; }

    /// <summary>Display name of the editor, denormalized for the audit UI.</summary>
    public string? ChangedByName { get; set; }

    public DateTime ChangedAt { get; set; } = DateTime.UtcNow;

    /// <summary>JSON array of { field, oldValue, newValue }.</summary>
    public string Changes { get; set; } = "[]";
}
