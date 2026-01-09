using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Options;

namespace web.Services;

public class SmtpOptions
{
    public string Host { get; set; } = "";
    public int Port { get; set; } = 587;
    public bool EnableSsl { get; set; } = true;
    public string Username { get; set; } = "";
    public string Password { get; set; } = "";
    public string FromEmail { get; set; } = "";
    public string FromName { get; set; } = "StudyBuddy";
}

public class SmtpEmailSender : IEmailSender
{
    private readonly SmtpOptions _opt;

    public SmtpEmailSender(IOptions<SmtpOptions> opt)
    {
        _opt = opt.Value;
    }

    public async Task SendAsync(string toEmail, string subject, string htmlBody)
    {
        if (string.IsNullOrWhiteSpace(_opt.Host) || string.IsNullOrWhiteSpace(_opt.FromEmail))
            return; // fail-safe: u devu neće rušiti app ako nema SMTP

        using var client = new SmtpClient(_opt.Host, _opt.Port)
        {
            EnableSsl = _opt.EnableSsl,
            Credentials = new NetworkCredential(_opt.Username, _opt.Password)
        };

        var msg = new MailMessage
        {
            From = new MailAddress(_opt.FromEmail, _opt.FromName),
            Subject = subject,
            Body = htmlBody,
            IsBodyHtml = true
        };
        msg.To.Add(toEmail);

        await client.SendMailAsync(msg);
    }
}
