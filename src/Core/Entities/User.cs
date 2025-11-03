using Microsoft.AspNetCore.Identity;
using Core.Entities.Common;

namespace Core.Entities;

public class User : IdentityUser<long>, IEntity<long>
{
    public string FirstName { get; set; } = default!;
    public string? LastName { get; set; }
    public string? Code { get; set; }
    public string FullName => LastName + " " + FirstName;
}