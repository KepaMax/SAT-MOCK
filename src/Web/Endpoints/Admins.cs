using EXAM_SYSTEM.Application.Common.Models;
using EXAM_SYSTEM.Application.Common.Models.Students;
using EXAM_SYSTEM.Application.Students.Commands.AssignSchool;
using EXAM_SYSTEM.Application.Students.Queries.GetStudentsWithPagination;
using EXAM_SYSTEM.Application.Users.Commands.LoginUser;

namespace EXAM_SYSTEM.Web.Endpoints;

public class Admins: EndpointGroupBase
{
    public override void Map(RouteGroupBuilder groupBuilder)
    {
        groupBuilder.WithTags("Admins");
        groupBuilder.MapPost("admin/login", Login);
        groupBuilder.MapPost("assign-school", AssignToSchool).RequireAuthorization(policy => policy.RequireRole("Administrator"));
        groupBuilder.MapGet("students", GetStudentsWithPagination).RequireAuthorization(policy => policy.RequireRole("Administrator"));
    }
    
    [EndpointName(nameof(GetStudentsWithPagination))]
    [EndpointSummary("Get Students")]
    [EndpointDescription("Get Students with Pagination")]
    public async Task<PaginatedList<StudentProfileDto>> GetStudentsWithPagination(
        ISender sender, 
        [AsParameters] GetStudentsWithPaginationQuery query)
    {
        return await sender.Send(query);
    }
    
    [EndpointName(nameof(AssignToSchool))]
    [EndpointSummary("Assign to School")]
    [EndpointDescription("Assign student to specific School")]
    public async Task<IResult> AssignToSchool(ISender sender, AssignSchoolCommand command)
    {
        await sender.Send(command);
        return Results.NoContent();
    }

    [EndpointName("AdminLogin")]
    [EndpointSummary("Admin Login")]
    [EndpointDescription("Login Admin into account")]
    public async Task<IResult> Login(ISender sender, LoginUserCommand command)
    {
        var result = await sender.Send(command);

        // If the handler returned null (invalid email/pass), we return 401
        return result is not null
            ? Results.Ok(result)
            : Results.Unauthorized();
    }
}
