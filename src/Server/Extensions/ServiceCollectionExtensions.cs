using OpenIddict.Abstractions;
using Core.Entities.OpenId;
using Infrastructure.Data;

namespace Api.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection RegisterOpenIddict(this IServiceCollection services)
    {
        services.AddOpenIddict()
            .AddCore(options =>
            {
                options.UseEntityFrameworkCore()
                       .UseDbContext<MainDbContext>()
                       .ReplaceDefaultEntities<OpenIdApplication, OpenIdAuthorization, OpenIdScope, OpenIdToken, long>();
            })

            .AddServer(options =>
            {
                options.SetTokenEndpointUris("/security/oauth/token");

                options.AllowPasswordFlow();
                options.AllowClientCredentialsFlow();
                options.AllowRefreshTokenFlow();

                options.RegisterScopes(
                    OpenIddictConstants.Scopes.OpenId,
                    OpenIddictConstants.Scopes.Profile,
                    OpenIddictConstants.Scopes.OfflineAccess,
                    "read", "write"
                );

                options.AddDevelopmentEncryptionCertificate()
                       .AddDevelopmentSigningCertificate();

                options.SetAccessTokenLifetime(TimeSpan.FromHours(24));
                options.SetRefreshTokenLifetime(TimeSpan.FromDays(30));

                options.UseAspNetCore()
                       .EnableTokenEndpointPassthrough()
                       .DisableTransportSecurityRequirement();
            })

            .AddValidation(options =>
            {
                options.UseLocalServer();
                options.UseAspNetCore();
            });

        return services;
    }
}