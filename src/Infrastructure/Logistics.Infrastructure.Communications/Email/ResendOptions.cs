namespace Logistics.Infrastructure.Communications.Email;

public record ResendOptions
{
    public const string SectionName = "Resend";
    public string ApiKey { get; set; } = default!;
    public string SenderEmail { get; set; } = default!;
    public string SenderName { get; set; } = "LogisticsX";

    /// <summary>
    /// Test/staging only: when set, ALL outgoing email is redirected to this
    /// address (the intended recipient is preserved in the subject). Leave unset
    /// in production. Controlled by env var Resend__OverrideRecipient.
    /// </summary>
    public string? OverrideRecipient { get; set; }
}
