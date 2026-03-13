using Microsoft.AspNetCore.Identity;
using EXAM_SYSTEM.Domain.Entities;

namespace EXAM_SYSTEM.Infrastructure.Identity;

public class ApplicationUser : IdentityUser
{
    public int? SchoolId { get; set; }
    public School? School { get; set; }
}
