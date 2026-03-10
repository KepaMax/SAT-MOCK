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
        groupBuilder.MapIdentityApi<ApplicationUser>();

        // Manual override so Scalar shows the Username field
        groupBuilder.MapPost("register", async (ISender sender, CreateUserCommand command) =>
        {
            var result = await sender.Send(command);
            return result.Succeeded ? Results.Ok() : Results.BadRequest();
        })
        .WithSummary("Registers a new student")
        .WithDescription("Accepts Username, Email, and Password."); // Scalar will now show all 3 fields!

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
