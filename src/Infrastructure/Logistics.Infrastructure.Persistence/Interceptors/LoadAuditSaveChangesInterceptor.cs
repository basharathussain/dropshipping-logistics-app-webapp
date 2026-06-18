using System.Security.Claims;
using System.Text.Json;
using Logistics.Domain.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace Logistics.Infrastructure.Persistence.Interceptors;

/// <summary>
/// Records a <see cref="LoadAuditLog"/> row for every Load creation and update, capturing the changed
/// fields (old -&gt; new), who changed them, and when. Runs in the same transaction as the save.
/// </summary>
public class LoadAuditSaveChangesInterceptor : SaveChangesInterceptor
{
    // Noise / audit-stamp columns we never want in the change history.
    private static readonly HashSet<string> Ignored =
    [
        "Id", "Number", "CreatedAt", "CreatedBy", "UpdatedAt", "UpdatedBy", "IsInProximity"
    ];

    private readonly HttpContext? httpContext;

    public LoadAuditSaveChangesInterceptor(IHttpContextAccessor? httpContextAccessor = null)
    {
        httpContext = httpContextAccessor?.HttpContext;
    }

    public override InterceptionResult<int> SavingChanges(
        DbContextEventData eventData, InterceptionResult<int> result)
    {
        Capture(eventData.Context);
        return base.SavingChanges(eventData, result);
    }

    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData, InterceptionResult<int> result, CancellationToken cancellationToken = default)
    {
        Capture(eventData.Context);
        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    private void Capture(DbContext? context)
    {
        if (context is null)
        {
            return;
        }

        var userId = httpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);
        var userName = httpContext?.User?.FindFirstValue("name")
                       ?? httpContext?.User?.FindFirstValue(ClaimTypes.Name)
                       ?? httpContext?.User?.FindFirstValue(ClaimTypes.Email);

        var logs = new List<LoadAuditLog>();

        foreach (var entry in context.ChangeTracker.Entries<Load>())
        {
            if (entry.State is not (EntityState.Added or EntityState.Modified))
            {
                continue;
            }

            var created = entry.State is EntityState.Added;
            var changes = new List<object>();

            // Scalar properties
            foreach (var p in entry.Properties)
            {
                if (Ignored.Contains(p.Metadata.Name))
                {
                    continue;
                }

                AddChange(changes, created, p.Metadata.Name, p.OriginalValue, p.CurrentValue, p.IsModified);
            }

            // Complex properties (DeliveryCost, OriginAddress, DestinationAddress, ...)
            foreach (var cp in entry.ComplexProperties)
            {
                foreach (var p in cp.Properties)
                {
                    AddChange(changes, created, $"{cp.Metadata.Name}.{p.Metadata.Name}",
                        p.OriginalValue, p.CurrentValue, p.IsModified);
                }
            }

            if (changes.Count == 0)
            {
                continue;
            }

            logs.Add(new LoadAuditLog
            {
                LoadId = entry.Entity.Id,
                Action = created ? "Created" : "Updated",
                ChangedBy = userId,
                ChangedByName = userName,
                ChangedAt = DateTime.UtcNow,
                Changes = JsonSerializer.Serialize(changes)
            });
        }

        if (logs.Count > 0)
        {
            context.Set<LoadAuditLog>().AddRange(logs);
        }
    }

    private static void AddChange(
        List<object> changes, bool created, string field, object? original, object? current, bool isModified)
    {
        if (created)
        {
            if (current is not null)
            {
                changes.Add(new { field, oldValue = (string?)null, newValue = Format(current) });
            }
        }
        else if (isModified && !Equals(original, current))
        {
            changes.Add(new { field, oldValue = Format(original), newValue = Format(current) });
        }
    }

    private static string? Format(object? value) => value switch
    {
        null => null,
        DateTime dt => dt.ToString("u"),
        decimal d => d.ToString("0.##"),
        double db => db.ToString("0.##"),
        _ => value.ToString()
    };
}
