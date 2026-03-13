using EXAM_SYSTEM.Application.Common.Models.Schools;

namespace EXAM_SYSTEM.Application.Common.Models.Students;

public class StudentProfileDto
{
    public int Id { get; set; }
    public string FullName { get; set; } = null!;
    
    // The nested School DTO
    public SchoolDto? School { get; set; }
}
