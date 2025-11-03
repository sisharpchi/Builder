using FluentValidation;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Net;
using System.Net.Mail;
using Application.Service;
using Application.Validation;
using Contract.Dto.Auth;
using Contract.Service;

namespace Application;

public static class Register
{
    public static IServiceCollection AddApplicationLayer(this IServiceCollection services)
    {
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IEmailService, EmailService>();
        services.AddScoped<ISmsService, SmsService>();
        services.AddScoped<IValidator<CreateUserDto>, CreateUserDtoValidator>();

        return services;
    }

    public static IServiceCollection AddEmailConfiguration(this IServiceCollection services, IConfiguration configuration)
    {
        var smtpSettings = configuration.GetSection("SmtpSettings");

        var fromEmail = smtpSettings["FromEmail"];
        var fromName = smtpSettings["FromName"];
        var host = smtpSettings["Host"];
        var port = int.Parse(smtpSettings["Port"]!);
        var username = smtpSettings["Username"];
        var password = smtpSettings["Password"];
        var enableSsl = bool.Parse(smtpSettings["EnableSsl"]!);

        services
            .AddFluentEmail(fromEmail, fromName)
            .AddSmtpSender(new SmtpClient(host)
            {
                Port = port,
                Credentials = new NetworkCredential(username, password),
                EnableSsl = enableSsl
            });

        return services;
    }
}