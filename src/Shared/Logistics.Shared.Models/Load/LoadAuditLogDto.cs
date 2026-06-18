namespace Logistics.Shared.Models;

public record LoadAuditLogDto
{
    public Guid Id { get; set; }
    public Guid LoadId { get; set; }
    public string Action { get; set; } = "";
    public string? ChangedBy { get; set; }
    public string? ChangedByName { get; set; }
    public DateTime ChangedAt { get; set; }
    public List<LoadAuditChangeDto> Changes { get; set; } = [];
}

public record LoadAuditChangeDto
{
    public string Field { get; set; } = "";
    public string? OldValue { get; set; }
    public string? NewValue { get; set; }
}
