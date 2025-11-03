using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using OpenIddict.Abstractions;
using OpenIddict.Server.AspNetCore;
using System.Security.Claims;
using Core.Entities;
using Core.Persistence;
using static OpenIddict.Abstractions.OpenIddictConstants;

namespace Api.Controllers;

[Route("api/security/oauth")]
[ApiController]
public class OAuthAuthorizationController : ControllerBase
{
    private readonly ILogger<OAuthAuthorizationController> _logger;
    private readonly UserManager<User> _userManager;
    private readonly IMainRepository _mainRepository;
    private readonly IOpenIddictApplicationManager _applicationManager;
    private readonly IOpenIddictScopeManager _scopeManager;

    public OAuthAuthorizationController(
        ILogger<OAuthAuthorizationController> logger,
        UserManager<User> userManager,
        IMainRepository mainRepository,
        IOpenIddictApplicationManager applicationManager,
        IOpenIddictScopeManager scopeManager)
    {
        _logger = logger;
        _userManager = userManager;
        _mainRepository = mainRepository;
        _applicationManager = applicationManager;
        _scopeManager = scopeManager;
    }

    [HttpPost("token")]
    public async Task<IActionResult> Exchange()
    {
        var request = HttpContext.GetOpenIddictServerRequest()
            ?? throw new InvalidOperationException("Invalid OpenIddict request.");

        if (request.IsClientCredentialsGrantType())
        {
            var app = await _applicationManager.FindByClientIdAsync(request.ClientId!)
                ?? throw new InvalidOperationException("Invalid client ID.");

            var identity = new ClaimsIdentity(
                TokenValidationParameters.DefaultAuthenticationType,
                Claims.Name,
                Claims.Role);

            identity.SetClaim(Claims.Subject, await _applicationManager.GetClientIdAsync(app));
            identity.SetClaim(Claims.Name, await _applicationManager.GetDisplayNameAsync(app));

            identity.SetScopes(request.GetScopes());
            identity.SetDestinations(_ => new[] { Destinations.AccessToken });

            return SignIn(new ClaimsPrincipal(identity), OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
        }
        else if (request.IsPasswordGrantType())
        {
            var user = await _userManager.Users.FirstOrDefaultAsync(x => x.UserName == request.Username);
            if (user == null)
            {
                return Unauthorized(new OpenIddictResponse
                {
                    Error = Errors.InvalidGrant,
                    ErrorDescription = "User unauthorized."
                });
            }

            if (!await _userManager.CheckPasswordAsync(user, request.Password!))
            {
                return Unauthorized(new OpenIddictResponse
                {
                    Error = Errors.InvalidGrant,
                    ErrorDescription = "Invalid credentials."
                });
            }

            var identity = new ClaimsIdentity(
                TokenValidationParameters.DefaultAuthenticationType,
                Claims.Name,
                Claims.Role);

            identity.SetClaim(Claims.Subject, user.Id.ToString());
            identity.SetClaim(Claims.Name, user.UserName ?? user.PhoneNumber ?? "user");

            identity.SetScopes(request.GetScopes());
            identity.SetDestinations(_ => new[] { Destinations.AccessToken, Destinations.IdentityToken });

            return SignIn(new ClaimsPrincipal(identity), OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
        }
        else if (request.IsRefreshTokenGrantType())
        {
            var result = await HttpContext.AuthenticateAsync(OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
            var principal = result.Principal;

            if (principal is null || !principal.Identity?.IsAuthenticated == true)
            {
                return Unauthorized(new OpenIddictResponse
                {
                    Error = Errors.InvalidGrant,
                    ErrorDescription = "The refresh token is invalid."
                });
            }

            var userId = principal.GetClaim(OpenIddictConstants.Claims.Subject);
            if (string.IsNullOrWhiteSpace(userId))
                return Forbid(OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);

            var user = await _userManager.FindByIdAsync(userId);
            if (user is null)
                return Forbid(OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);

            var identity = new ClaimsIdentity(TokenValidationParameters.DefaultAuthenticationType, Claims.Name, Claims.Role);
            identity.SetClaim(Claims.Subject, user.Id.ToString());
            identity.SetDestinations(_ => new[] { Destinations.AccessToken, Destinations.IdentityToken });
            identity.SetScopes(principal.GetScopes());

            return SignIn(new ClaimsPrincipal(identity), OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
        }

        return BadRequest(new OpenIddictResponse
        {
            Error = Errors.UnsupportedGrantType,
            ErrorDescription = "This grant type is not supported."
        });
    }
}