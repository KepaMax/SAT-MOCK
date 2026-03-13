
using EXAM_SYSTEM.Domain.Entities;

namespace EXAM_SYSTEM.Application.Common.Interfaces;

public interface IApplicationDbContext
{
    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
    DbSet<Student> Students { get; set; }
    DbSet<School> Schools { get; set; }
}
