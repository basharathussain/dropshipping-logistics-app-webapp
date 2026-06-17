using Logistics.Domain.Core;
using Logistics.Domain.Primitives.Enums;
using Logistics.Domain.Primitives.ValueObjects;

namespace Logistics.Domain.Entities;

/// <summary>
/// Company's customer (e.g. broker, shipper, etc.).
/// </summary>
public class Customer : AuditableEntity, ITenantEntity
{
    public required string Name { get; set; }
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public Address? Address { get; set; }
    public CustomerStatus Status { get; set; } = CustomerStatus.Active;
    public string? Notes { get; set; }

    /// <summary>
    /// VAT ID (EU), EIN (US), GSTIN (IN), ABN (AU), etc. Required for EU B2B reverse-charge.
    /// </summary>
    public string? TaxId { get; set; }

    /// <summary>
    /// When true, the tax calculator skips computing tax for this customer (charity, gov, NGO).
    /// </summary>
    public bool IsVatExempt { get; set; }

    /// <summary>Customer company website URL.</summary>
    public string? Website { get; set; }

    /// <summary>Primary contact person at the customer.</summary>
    public string? ContactPerson { get; set; }

    /// <summary>US Motor Carrier number (FMCSA), if the customer is a carrier/broker.</summary>
    public string? McNumber { get; set; }

    /// <summary>US DOT number, if applicable.</summary>
    public string? DotNumber { get; set; }

    public virtual List<LoadInvoice> Invoices { get; set; } = [];
}
