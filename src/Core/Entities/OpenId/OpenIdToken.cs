using OpenIddict.EntityFrameworkCore.Models;

namespace Core.Entities.OpenId;

public class OpenIdToken : OpenIddictEntityFrameworkCoreToken<long, OpenIdApplication,OpenIdAuthorization>
{
}
