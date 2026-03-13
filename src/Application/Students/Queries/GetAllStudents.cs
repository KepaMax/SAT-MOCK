using EXAM_SYSTEM.Application.Common.Interfaces;
using EXAM_SYSTEM.Application.Common.Mappings;
using EXAM_SYSTEM.Application.Common.Models;
using EXAM_SYSTEM.Application.Students.Queries.GetStudentProfile; // Using your existing DTO
using AutoMapper;
using AutoMapper.QueryableExtensions;
using EXAM_SYSTEM.Application.Common.Models.Students;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace EXAM_SYSTEM.Application.Students.Queries.GetStudentsWithPagination;

public record GetStudentsWithPaginationQuery : IRequest<PaginatedList<StudentProfileDto>>
{
    public int? PageNumber { get; init; } = 1;
    public int? PageSize { get; init; } = 10;
}

public class GetStudentsWithPaginationHandler(IApplicationDbContext context, IMapper mapper) 
    : IRequestHandler<GetStudentsWithPaginationQuery, PaginatedList<StudentProfileDto>>
{
    public async Task<PaginatedList<StudentProfileDto>> Handle(GetStudentsWithPaginationQuery request, CancellationToken ct)
    {
        return await context.Students
            .AsNoTracking()
            .OrderBy(x => x.FullName)
            .ProjectTo<StudentProfileDto>(mapper.ConfigurationProvider)
            .PaginatedListAsync(request.PageNumber ?? 1, request.PageSize ?? 10, ct);
    }
}
