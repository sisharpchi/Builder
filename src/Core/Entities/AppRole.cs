using Microsoft.AspNetCore.Identity;
using Core.Entities.Common;

namespace Core.Entities;

public class AppRole : IdentityRole<long>, IEntity<long>
{
    public AppRole(string name) : base(name)
    {
        if (!name.StartsWith("app."))
            base.Name = $"app.{name}";

        DisplayName = base.Name;
    }
    public string? DisplayName { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public string? CreatedBy { get; set; }
    public DateTime? LastModifiedAt { get; set; }
    public string? LastModifiedBy { get; set; }
}