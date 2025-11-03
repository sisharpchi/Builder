using FluentEmail.Core;
using Contract.Service;

namespace Application.Service;

public class EmailService : IEmailService
{
    private readonly IFluentEmail _fluentEmail;

    public EmailService(IFluentEmail fluentEmail)
    {
        _fluentEmail = fluentEmail;
    }

    public async Task SendConfirmationEmailAsync(string toEmail, string token)
    {
        var confirmationLink = $"https://localhost:7069/api/auth/confirm-email?email={toEmail}&token={token}";

        var subject = "Confirm your email address";
        var body = $@"
                <p>Welcome!</p>
                <p>Please confirm your email address by clicking the link below:</p>
                <p><a href='{confirmationLink}'>Confirm Email</a></p>
                <p>If you did not request this, please ignore this message.</p>
            ";

        await _fluentEmail
            .To(toEmail)
            .Subject(subject)
            .Body(body, isHtml: true)
            .SendAsync();
    }
}
