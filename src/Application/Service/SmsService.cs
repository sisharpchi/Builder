using Contract.Service;

namespace Application.Service;

public class SmsService : ISmsService
{
    public Task SendAsync(string phoneNumber, string token)
    {
        return Task.CompletedTask;
    }
}