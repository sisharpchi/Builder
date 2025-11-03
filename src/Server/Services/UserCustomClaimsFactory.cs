using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using Core.Entities;

namespace Api.Services;

public class UserCustomClaimsFactory : UserClaimsPrincipalFactory<User, AppRole>
{
    private readonly RoleManager<AppRole> _roleManager;
    private readonly UserManager<User> _userManager;

    public UserCustomClaimsFactory(
        UserManager<User> userManager,
        RoleManager<AppRole> roleManager,
        IOptions<IdentityOptions> options)
        : base(userManager, roleManager, options)
    {
        _roleManager = roleManager;
        _userManager = userManager;
    }

    protected override async Task<ClaimsIdentity> GenerateClaimsAsync(User user)
    {
        var identity = await base.GenerateClaimsAsync(user);

        if (!string.IsNullOrEmpty(user.Email))
            identity.AddClaim(new Claim(ClaimTypes.Email, user.Email));

        if (!string.IsNullOrEmpty(user.UserName))
            identity.AddClaim(new Claim(ClaimTypes.Name, user.UserName));

        await AddAppRoleClaimsAsync(identity, user);

        return identity;
    }

    private async Task AddAppRoleClaimsAsync(ClaimsIdentity claimsIdentity, User user)
    {
        var appRoles = await UserManager.GetRolesAsync(user);

        foreach (var roleName in appRoles)
        {
            claimsIdentity.AddClaim(new Claim(ClaimTypes.Role, roleName));

            var role = await RoleManager.FindByNameAsync(roleName);
            if (role == null)
                continue;

            var roleClaims = await RoleManager.GetClaimsAsync(role);
            claimsIdentity.AddClaims(roleClaims);
        }
    }
}