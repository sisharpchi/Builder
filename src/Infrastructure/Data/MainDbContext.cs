using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Reflection;
using Core.Entities;
using Core.Entities.OpenId;

namespace Infrastructure.Data;

public class MainDbContext : IdentityDbContext<User, AppRole, long>
{
    public MainDbContext(DbContextOptions<MainDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<IdentityRoleClaim<long>>().ToTable("RoleClaims");
        builder.Entity<IdentityUserRole<long>>().ToTable("UserRoles");
        builder.Entity<IdentityUserLogin<long>>().ToTable("UserLogins");
        builder.Entity<IdentityUserClaim<long>>().ToTable("UserClaims");

        builder.Entity<OpenIdApplication>().ToTable("OpenIddictEntityFrameworkCoreApplications");
        builder.Entity<OpenIdAuthorization>().ToTable("OpenIddictEntityFrameworkCoreAuthorizations");
        builder.Entity<OpenIdScope>().ToTable("OpenIddictEntityFrameworkCoreScopes");
        builder.Entity<OpenIdToken>().ToTable("OpenIddictEntityFrameworkCoreTokens");

        builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

    }
}