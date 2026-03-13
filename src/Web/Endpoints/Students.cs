using EXAM_SYSTEM.Application.Common.Models;
using EXAM_SYSTEM.Application.Common.Models.Students;
using EXAM_SYSTEM.Application.Students.Commands.AssignSchool;
using EXAM_SYSTEM.Application.Students.Queries.GetStudentProfile;
using EXAM_SYSTEM.Application.Students.Queries.GetStudentsWithPagination;
using EXAM_SYSTEM.Application.Users.Commands;
using EXAM_SYSTEM.Application.Users.Commands.LoginUser;
using EXAM_SYSTEM.Application.Users.Queries.GetProfile;
using EXAM_SYSTEM.Infrastructure.Identity;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace EXAM_SYSTEM.Web.Endpoints;

public class Students : EndpointGroupBase
{
    public override void Map(RouteGroupBuilder groupBuilder)
    {
        groupBuilder.WithTags("Students");

        // Public Endpoints
        groupBuilder.MapPost("register", Register);
        groupBuilder.MapPost("login", Login);
        groupBuilder.MapGet("profile", GetProfile).RequireAuthorization();
    }

    [EndpointName(nameof(Register))]
    [EndpointSummary("Register User")]
    [EndpointDescription("Registers a new user account with the provided username, email and password.")]
    public async Task<Results<Ok, BadRequest<string[]>>> Register(ISender sender, CreateUserCommand command)
    {
        var result = await sender.Send(command);
        return result.Succeeded.Succeeded
            ? TypedResults.Ok()
            : TypedResults.BadRequest(result.Succeeded.Errors);
    }

    [EndpointName("StudentLogin")]
    [EndpointSummary("Student Login")]
    [EndpointDescription("Login user into account")]
    public async Task<IResult> Login(ISender sender, LoginUserCommand command)
    {
        var result = await sender.Send(command);

        // If the handler returned null (invalid email/pass), we return 401
        return result is not null
            ? Results.Ok(result)
            : Results.Unauthorized();
    }

    [EndpointName(nameof(GetProfile))]
    [EndpointSummary("Get Profile")]
    [EndpointDescription("Get user Profile info")]
    public async Task<IResult> GetProfile(ISender sender)
    {
        var result = await sender.Send(new GetStudentProfileQuery());
        return result is not null
            ? Results.Ok(result)
            : Results.Unauthorized();
    }
}
