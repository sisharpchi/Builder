using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Core.Entities;
using Core.Persistence;
using Infrastructure.Data;
using Infrastructure.Persistence;

namespace Infrastructure;

public static class Register
{
    public static IServiceCollection AddInfrastructureLayer(
        this IServiceCollection services)
    {
        services.AddScoped(typeof(UnitOfWork<>));
        services.AddScoped<IMainRepository, MainRepository>();
        services.AddScoped<IUnitOfWork, UnitOfWork<MainDbContext>>();

        return services;
    }

    public static IServiceCollection AddMainDatabase(
        this IServiceCollection services,
        IConfiguration configuration,
        string mainDbConnectionSection = "MainDatabase")
    {
        var connectionString = configuration.GetConnectionString(mainDbConnectionSection);

        services.AddDbContext<MainDbContext>(options =>
        {
            options.UseNpgsql(connectionString, sql =>
            {
                sql.MigrationsAssembly(typeof(MainDbContext).Assembly.FullName);
            });
        });

        services.AddIdentityCore<User>(options =>
        {
            options.SignIn.RequireConfirmedAccount = true;

            options.Password.RequiredLength = 8;
            options.Password.RequireUppercase = true;
            options.Password.RequireNonAlphanumeric = false;
            options.Password.RequireLowercase = true;
            options.Password.RequireDigit = true;

            options.User.AllowedUserNameCharacters =
                "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789_.@+";
            options.User.RequireUniqueEmail = true;

            options.SignIn.RequireConfirmedEmail = true;
            options.SignIn.RequireConfirmedPhoneNumber = true;

            options.Tokens.EmailConfirmationTokenProvider = TokenOptions.DefaultEmailProvider;
            options.Tokens.ChangePhoneNumberTokenProvider = TokenOptions.DefaultPhoneProvider;
        })
        .AddRoles<AppRole>()
        .AddEntityFrameworkStores<MainDbContext>()
        .AddTokenProvider<EmailTokenProvider<User>>(TokenOptions.DefaultEmailProvider)
        .AddTokenProvider<PhoneNumberTokenProvider<User>>(TokenOptions.DefaultPhoneProvider);

        return services;
    }
}