using EXAM_SYSTEM.Application.Common.Models;
using EXAM_SYSTEM.Application.Users.Commands.LoginUser;

namespace EXAM_SYSTEM.Application.Common.Interfaces;

public interface IIdentityService
{
    Task<string?> GetUserNameAsync(string userId);

    Task<bool> IsInRoleAsync(string userId, string role);

    Task<bool> AuthorizeAsync(string userId, string policyName);

    Task<(Result Result, string UserId)> CreateUserAsync(string email, string password);

    Task<Result> DeleteUserAsync(string userId);

    Task<LoginResponse?> LoginAsync(string email, string password);

    Task<LoginResponse?> RefreshTokenAsync(string refreshToken);

    Task<UserProfileInfo?> GetUserProfileAsync(string userId);
}
