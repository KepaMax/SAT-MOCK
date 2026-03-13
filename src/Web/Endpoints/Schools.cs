using EXAM_SYSTEM.Application.Schools.Commands.CreateSchool;
using EXAM_SYSTEM.Application.Schools.Commands.UpdateSchool;
using EXAM_SYSTEM.Application.Schools.Commands.DeleteSchool;
using EXAM_SYSTEM.Application.Schools.Queries.GetSchoolById;
using EXAM_SYSTEM.Application.Schools.Queries.GetSchoolsWithPagination;
using EXAM_SYSTEM.Application.Common.Models;
using EXAM_SYSTEM.Application.Common.Models.Schools;
using MediatR;

namespace EXAM_SYSTEM.Web.Endpoints;

public class Schools : EndpointGroupBase
{
    // Fix: Change IEndpointRouteBuilder to RouteGroupBuilder
    public override void Map(RouteGroupBuilder app)
    {
        var group = app.WithTags("Schools")
            .RequireAuthorization(policy => policy.RequireRole("Administrator"));

        // Ensure the ID parameter is correctly defined in the template
        group.MapGet(GetSchoolsWithPagination, "/"); // Matches: GET /api/schools
    
        group.MapGet(GetSchool, "/{id}");            // Matches: GET /api/schools/5
    
        group.MapPost(Create, "/");                  // Matches: POST /api/schools
    
        group.MapPatch(Update, "/{id}");             // Matches: PATCH /api/schools/5
    
        group.MapDelete(Delete, "/{id}");           // Matches: DELETE /api/schools/5
    }

    public async Task<PaginatedList<SchoolDto>> GetSchoolsWithPagination(ISender sender, [AsParameters] GetSchoolsWithPaginationQuery query)
        => await sender.Send(query);

    public async Task<SchoolDto> GetSchool(ISender sender, int id)
        => await sender.Send(new GetSchoolByIdQuery(id));

    public async Task<int> Create(ISender sender, CreateSchoolCommand command)
        => await sender.Send(command);

    public async Task<IResult> Update(ISender sender, int id, UpdateSchoolCommand command)
    {
        if (id != command.Id) return Results.BadRequest();
        await sender.Send(command);
        return Results.NoContent();
    }

    public async Task<IResult> Delete(ISender sender, int id)
    {
        await sender.Send(new DeleteSchoolCommand(id));
        return Results.NoContent();
    }
}
