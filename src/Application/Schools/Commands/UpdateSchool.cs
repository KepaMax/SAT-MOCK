using EXAM_SYSTEM.Application.Common.Interfaces;

namespace EXAM_SYSTEM.Application.Schools.Commands.UpdateSchool;

public record UpdateSchoolCommand : IRequest
{
    public int Id { get; init; }
    public string? Name { get; init; }
    public string? Address { get; init; }
}

public class UpdateSchoolCommandHandler(IApplicationDbContext context)
    : IRequestHandler<UpdateSchoolCommand>
{
    public async Task Handle(UpdateSchoolCommand request, CancellationToken ct)
    {
        var entity = await context.Schools.FindAsync([request.Id], ct)
                     ?? throw new Exception("School not found");

        if (request.Name != null) entity.Name = request.Name;
        if (request.Address != null) entity.Address = request.Address;

        await context.SaveChangesAsync(ct);
    }
}
