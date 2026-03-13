using System.Reflection;
using EXAM_SYSTEM.Application.Common.Interfaces;
using EXAM_SYSTEM.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using EXAM_SYSTEM.Domain.Entities;

namespace EXAM_SYSTEM.Infrastructure.Data;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser>, IApplicationDbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

    public DbSet<Student> Students { get; set; } = null!;
    public DbSet<School> Schools { get; set; } = null!;
    
    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        builder.Entity<ApplicationUser>()
            .HasIndex(u => u.NormalizedUserName)
            .HasDatabaseName("UserNameIndex")
            .IsUnique(true);

        builder.Entity<ApplicationUser>()
            .HasIndex(u => u.NormalizedEmail)
            .HasDatabaseName("EmailIndex")
            .IsUnique(true);

        builder.Entity<ApplicationUser>()
            .HasOne<Student>()
            .WithOne()
            .HasForeignKey<Student>(s => s.IdentityId)
            .HasPrincipalKey<ApplicationUser>(u => u.Id);

        builder.Entity<Student>(entity =>
        {
            entity.HasOne(s => s.School)
                .WithMany(sch => sch.Students)
                .HasForeignKey(s => s.SchoolId)
                .OnDelete(DeleteBehavior.Restrict); // Prevents deleting a school if it has students
        });
    }
}
