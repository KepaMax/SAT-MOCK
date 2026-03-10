using System.Reflection;
using EXAM_SYSTEM.Application.Common.Interfaces;
using EXAM_SYSTEM.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace EXAM_SYSTEM.Infrastructure.Data;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser>, IApplicationDbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }
    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        builder.Entity<ApplicationUser>()
        .HasIndex(u => u.NormalizedUserName)
        .HasDatabaseName("UserNameIndex")
        .IsUnique(false);

        builder.Entity<ApplicationUser>()
        .HasIndex(u => u.NormalizedEmail)
        .HasDatabaseName("EmailIndex")
        .IsUnique(true);
    }
}
