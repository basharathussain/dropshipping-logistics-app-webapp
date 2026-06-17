using Logistics.Domain.Primitives.Enums;
using Logistics.Domain.Primitives.ValueObjects;

namespace Logistics.Shared.Models;

public class CustomerDto
{
    public Guid Id { get; set; }
    public required string Name { get; set; }
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public Address? Address { get; set; }
    public CustomerStatus Status { get; set; }
    public string? Notes { get; set; }
    public string? TaxId { get; set; }
    public bool IsVatExempt { get; set; }
    public string? Website { get; set; }
    public string? ContactPerson { get; set; }
    public string? McNumber { get; set; }
    public string? DotNumber { get; set; }
    public DateTime CreatedAt { get; set; }
}
