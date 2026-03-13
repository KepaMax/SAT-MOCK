using EXAM_SYSTEM.Application.Common.Interfaces;
using EXAM_SYSTEM.Application.Common.Mappings;
using EXAM_SYSTEM.Application.Common.Models;
using EXAM_SYSTEM.Application.Common.Models.Schools;

namespace EXAM_SYSTEM.Application.Schools.Queries.GetSchoolsWithPagination;

public record GetSchoolsWithPaginationQuery : IRequest<PaginatedList<SchoolDto>>
{
    public int? PageNumber { get; init; } = 1;
    public int? PageSize { get; init; } = 10;
}

public class GetSchoolsWithPaginationHandler(IApplicationDbContext context, IMapper mapper) 
    : IRequestHandler<GetSchoolsWithPaginationQuery, PaginatedList<SchoolDto>>
{
    public async Task<PaginatedList<SchoolDto>> Handle(GetSchoolsWithPaginationQuery request, CancellationToken ct)
    {
        return await context.Schools
            .OrderBy(x => x.Name)
            .ProjectTo<SchoolDto>(mapper.ConfigurationProvider)
            .PaginatedListAsync(request.PageNumber ?? 1, request.PageSize ?? 10, ct);
    }
}
