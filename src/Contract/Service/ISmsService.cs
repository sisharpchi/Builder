namespace Contract.Service;

public interface ISmsService
{
    Task SendAsync(string phoneNumber, string token);
}