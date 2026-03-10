using Microsoft.AspNetCore.Identity;

public class CustomUserValidator<TUser> : IUserValidator<TUser> where TUser : class
{
    public Task<IdentityResult> ValidateAsync(UserManager<TUser> manager, TUser user)
    {
        // We only care about the default validation or we can return Success
        // and let the 'RequireUniqueEmail' option handle the email check.
        return Task.FromResult(IdentityResult.Success);
    }
}
