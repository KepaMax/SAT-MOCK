using AutoMapper;
using AutoMapper.QueryableExtensions;
using EXAM_SYSTEM.Application.Common.Interfaces;
using EXAM_SYSTEM.Application.Common.Models.Students;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace EXAM_SYSTEM.Application.Students.Queries.GetStudentProfile;

public record GetStudentProfileQuery : IRequest<StudentProfileDto>;

public class GetStudentProfileHandler(
    IApplicationDbContext context, 
    IUser user, 
    IMapper mapper) : IRequestHandler<GetStudentProfileQuery, StudentProfileDto>
{
    public async Task<StudentProfileDto> Handle(GetStudentProfileQuery request, CancellationToken cancellationToken)
    {
        var profile = await context.Students
            .Where(s => s.IdentityId == user.Id)
            .ProjectTo<StudentProfileDto>(mapper.ConfigurationProvider)
            .FirstOrDefaultAsync(cancellationToken);

        if (profile == null)
        {
            // The template usually has a NotFoundException that maps to 404
            throw new Exception("Profile not found for the current user.");
        }

        return profile;
    }
}
