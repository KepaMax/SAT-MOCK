using EXAM_SYSTEM.Application.Common.Interfaces;
using EXAM_SYSTEM.Domain.Entities;

namespace EXAM_SYSTEM.Application.Schools.Commands.CreateSchool;

public record CreateSchoolCommand : IRequest<int>
{
    public required string Name { get; init; }
    public required string Address { get; init; }
}

public class CreateSchoolCommandHandler(IApplicationDbContext context) 
    : IRequestHandler<CreateSchoolCommand, int>
{
    public async Task<int> Handle(CreateSchoolCommand request, CancellationToken ct)
    {
        var entity = new School() { Name = request.Name, Address = request.Address };
        context.Schools.Add(entity);
        await context.SaveChangesAsync(ct);
        return entity.Id;
    }
}
