using EXAM_SYSTEM.Application.Common.Interfaces;

namespace EXAM_SYSTEM.Application.Schools.Commands.DeleteSchool;

public record DeleteSchoolCommand(int Id) : IRequest;

public class DeleteSchoolCommandHandler(IApplicationDbContext context) 
    : IRequestHandler<DeleteSchoolCommand>
{
    public async Task Handle(DeleteSchoolCommand request, CancellationToken ct)
    {
        var entity = await context.Schools.FindAsync([request.Id], ct) 
                     ?? throw new Exception("School not found");

        context.Schools.Remove(entity);
        await context.SaveChangesAsync(ct);
    }
}
