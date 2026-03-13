using EXAM_SYSTEM.Application.Common.Interfaces;
using EXAM_SYSTEM.Application.Common.Models.Schools;

namespace EXAM_SYSTEM.Application.Schools.Queries.GetSchoolById;

public record GetSchoolByIdQuery(int Id) : IRequest<SchoolDto>;

public class GetSchoolByIdHandler(IApplicationDbContext context, IMapper mapper) 
    : IRequestHandler<GetSchoolByIdQuery, SchoolDto>
{
    public async Task<SchoolDto> Handle(GetSchoolByIdQuery request, CancellationToken ct)
    {
        return await context.Schools
                   .Where(x => x.Id == request.Id)
                   .ProjectTo<SchoolDto>(mapper.ConfigurationProvider)
                   .FirstOrDefaultAsync(ct) 
               ?? throw new Exception("School not found");
    }
}
