using Microsoft.Extensions.Logging;
using SpeededERP.Modules.Business.CRM.Application.Ports;

namespace SpeededERP.Modules.Business.CRM.Infrastructure.Email;

/// <summary>
/// Default adapter for <see cref="IEmailSender"/> (architecture section 22). A
/// production deployment swaps this for SendGrid/SES by rebinding the interface — no
/// Application-layer code changes. Actual SMTP wiring (host/credentials) is intentionally
/// left to the hosting Program.cs via <c>System.Net.Mail</c> or a resilient HTTP client,
/// omitted here to keep this scaffold dependency-free; see deploy/docker/.env.example.
/// </summary>
public sealed class SmtpEmailSender(ILogger<SmtpEmailSender> logger) : IEmailSender
{
    public Task SendAsync(string toAddress, string subject, string htmlBody, CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Envoi d'un e-mail à {ToAddress} — {Subject}", toAddress, subject);

        // TODO(prod): brancher un vrai transport SMTP/SendGrid ici. Conservé en no-op
        // journalisé pour que le scaffold compile et s'exécute sans secret configuré.
        return Task.CompletedTask;
    }
}
