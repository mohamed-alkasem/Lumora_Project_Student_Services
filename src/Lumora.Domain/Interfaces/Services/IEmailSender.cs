namespace Lumora.Domain.Interfaces.Services;

/// Outgoing email abstraction. The SMTP implementation no-ops (with a log entry)
/// when no SMTP host is configured, so features never break in development.
public interface IEmailSender
{
    Task SendAsync(string toEmail, string subject, string htmlBody);
}
