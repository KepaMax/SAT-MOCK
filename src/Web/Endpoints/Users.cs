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
        groupBuilder.MapPost("refresh", Refresh);
        groupBuilder.MapPost("logout", Logout).RequireAuthorization();
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
