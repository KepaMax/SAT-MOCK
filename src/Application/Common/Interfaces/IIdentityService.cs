using EXAM_SYSTEM.Application.Common.Models;

namespace EXAM_SYSTEM.Application.Common.Interfaces;

public interface IIdentityService
{
    Task<string?> GetUserNameAsync(string userId);

    Task<bool> IsInRoleAsync(string userId, string role);

    Task<bool> AuthorizeAsync(string userId, string policyName);

    Task<(Result Result, string UserId)> CreateUserAsync(string userName, string email, string password);

    Task<Result> DeleteUserAsync(string userId);
}
