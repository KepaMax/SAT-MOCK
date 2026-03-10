using EXAM_SYSTEM.Application.Users.Commands;
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
        var identityGroup = groupBuilder.MapGroup("identity");
        identityGroup.MapIdentityApi<ApplicationUser>();
        // This registers at: /users/identity/register, /users/identity/login, etc.

        // Your register lives at /users/register — no conflict
        groupBuilder.MapPost("register", async (ISender sender, CreateUserCommand command) =>
        {
            var result = await sender.Send(command);
            return result.Succeeded.Succeeded
                ? Results.Ok()
                : Results.BadRequest(result.Succeeded.Errors);
        });

        groupBuilder.MapPost("logout", Logout).RequireAuthorization();
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
