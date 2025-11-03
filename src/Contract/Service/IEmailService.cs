namespace Contract.Service;

public interface IEmailService
{
    Task SendConfirmationEmailAsync(string toEmail, string token);
}
