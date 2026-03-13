namespace EXAM_SYSTEM.Domain.Entities;

public class School : BaseEntity
{
    public required string Name { get; set; }
    public required string Address { get; set; }

    public ICollection<Student> Students { get; set; } = new List<Student>();
}
