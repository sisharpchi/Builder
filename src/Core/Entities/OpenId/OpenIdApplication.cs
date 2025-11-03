using OpenIddict.EntityFrameworkCore.Models;

namespace Core.Entities.OpenId;

public class OpenIdApplication : OpenIddictEntityFrameworkCoreApplication<long, OpenIdAuthorization, OpenIdToken>
{
}
