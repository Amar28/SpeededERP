namespace SpeededERP.Modules.Business.CRM.Application.Ports;

/// <summary>Outbound port for transactional email — adapter lives in Infrastructure (SMTP/SendGrid, section 22).</summary>
public interface IEmailSender
{
    Task SendAsync(string toAddress, string subject, string htmlBody, CancellationToken cancellationToken = default);
}
