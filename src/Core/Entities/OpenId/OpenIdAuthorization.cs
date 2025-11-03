using OpenIddict.EntityFrameworkCore.Models;

namespace Core.Entities.OpenId;

public class OpenIdAuthorization : OpenIddictEntityFrameworkCoreAuthorization<long, OpenIdApplication, OpenIdToken>
{
}
