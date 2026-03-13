using EXAM_SYSTEM.Application.Common.Interfaces;
using EXAM_SYSTEM.Domain.Entities;

namespace EXAM_SYSTEM.Application.Students.Commands.AssignSchool;

public record AssignSchoolCommand(int SchoolId, int StudentId) : IRequest;

public class AssignSchoolCommandHandler(IApplicationDbContext context) 
    : IRequestHandler<AssignSchoolCommand>
{
    public async Task Handle(AssignSchoolCommand request, CancellationToken ct)
    {
        // 1. Find the student by the ID provided by the Admin
        var student = await context.Students
            .FirstOrDefaultAsync(s => s.Id == request.StudentId, ct);

        if (student == null)
        {
            throw new NotFoundException(nameof(Student), request.StudentId.ToString());
        }

        // 2. Assign the school
        student.SchoolId = request.SchoolId;

        // 3. Save changes
        await context.SaveChangesAsync(ct);
    }
}
