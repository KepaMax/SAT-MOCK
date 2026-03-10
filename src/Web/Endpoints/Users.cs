using EXAM_SYSTEM.Application.Users.Commands;
using EXAM_SYSTEM.Application.Users.Commands.LoginUser;
using EXAM_SYSTEM.Application.Users.Queries.GetProfile;
using EXAM_SYSTEM.Infrastructure.Identity;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace EXAM_SYSTEM.Web.Endpoints;

public class Users : EndpointGroupBase
{
    public override void Map(RouteGroupBuilder groupBuilder)
    {
        // 1. Map the built-in API

        groupBuilder.MapPost("register", Register);
        groupBuilder.MapPost("login", Login);
        groupBuilder.MapPost("refresh", Refresh);
        groupBuilder.MapGet("profile", GetProfile);
        groupBuilder.MapPost("logout", Logout).RequireAuthorization();
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

    [EndpointName(nameof(Login))]
    [EndpointSummary("Login")]
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
        var result = await sender.Send(new GetProfileQuery());
        return result is not null
            ? Results.Ok(result)
            : Results.Unauthorized();
    }

    [EndpointName(nameof(Refresh))]
    [EndpointSummary("Refreh")]
    [EndpointDescription("Refresh user tokens")]
    public static async Task<IResult> Refresh(ISender sender, RefreshTokenCommand command)
    {
        var result = await sender.Send(command);

        return result is not null
            ? Results.Ok(result)
            : Results.Unauthorized();
    }

    [EndpointName(nameof(Logout))]
    [EndpointSummary("Log out")]
    [EndpointDescription("Logs out the current user by clearing the authentication cookie.")]
    public async Task<Results<Ok, UnauthorizedHttpResult>> Logout(SignInManager<ApplicationUser> signInManager, [FromBody] object empty)
    {
        if (empty != null)
        {
            await signInManager.SignOutAsync();
            return TypedResults.Ok();
        }

        return TypedResults.Unauthorized();
    }
}
