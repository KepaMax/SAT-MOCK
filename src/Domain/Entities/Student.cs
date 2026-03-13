namespace EXAM_SYSTEM.Domain.Entities;

public class Student : BaseEntity
{
    // The "Bridge" to Microsoft Identity
    public required string IdentityId { get; set; }

    public required string FullName { get; set; }

    // Relationship to School is now clean and within the same layer
    public int? SchoolId { get; set; } 
    public School? School { get; set; }

// Future expansion
// public ICollection<ExamResult> Results { get; set; }
}
