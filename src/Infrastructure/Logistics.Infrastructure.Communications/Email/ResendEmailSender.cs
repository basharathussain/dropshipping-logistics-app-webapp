using Logistics.Application.Abstractions.Email;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Resend;

namespace Logistics.Infrastructure.Communications.Email;

internal sealed class ResendEmailSender(
    IResend resend,
    IOptions<ResendOptions> options,
    ILogger<ResendEmailSender> logger) : IEmailSender
{
    public async Task<bool> SendEmailAsync(string recipient, string subject, string htmlBody)
    {
        ArgumentException.ThrowIfNullOrEmpty(recipient);
        ArgumentException.ThrowIfNullOrEmpty(subject);
        ArgumentException.ThrowIfNullOrEmpty(htmlBody);

        try
        {
            // Test/staging override: when Resend:OverrideRecipient is set, redirect
            // ALL outgoing mail to that address (the real recipient is preserved in
            // the subject). Leave it unset in production. Remove the env to revert.
            var overrideTo = options.Value.OverrideRecipient;
            var redirecting = !string.IsNullOrWhiteSpace(overrideTo);
            var actualRecipient = redirecting ? overrideTo! : recipient;
            var finalSubject = redirecting ? $"[to: {recipient}] {subject}" : subject;

            var message = new EmailMessage
            {
                From = $"{options.Value.SenderName} <{options.Value.SenderEmail}>",
                Subject = finalSubject,
                HtmlBody = htmlBody
            };
            message.To.Add(actualRecipient);

            await resend.EmailSendAsync(message);
            logger.LogInformation("Email has been sent to {Mail} (intended {Intended}), subject: '{Subject}'",
                actualRecipient, recipient, subject);
            return true;
        }
        catch (Exception ex)
        {
            logger.LogWarning(
                "Could not send email to {Mail}, subject: '{Subject}'. \nThrown exception: {Exception}",
                recipient, subject, ex.ToString());
            return false;
        }
    }
}
