using EXAM_SYSTEM.Application.Common.Interfaces;
using EXAM_SYSTEM.Application.Common.Models;
using EXAM_SYSTEM.Application.Users.Commands.LoginUser;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.BearerToken;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace EXAM_SYSTEM.Infrastructure.Identity;

public class IdentityService(UserManager<ApplicationUser> userManager,
    IUserClaimsPrincipalFactory<ApplicationUser> userClaimsPrincipalFactory,
    IAuthorizationService authorizationService,
    SignInManager<ApplicationUser> signInManager,
    IOptionsMonitor<BearerTokenOptions> bearerTokenOptions,
    IDataProtectionProvider dataProtectionProvider,
    TimeProvider timeProvider) : IIdentityService
{
    private readonly UserManager<ApplicationUser> _userManager = userManager;
    private readonly SignInManager<ApplicationUser> _signInManager = signInManager;
    private readonly IUserClaimsPrincipalFactory<ApplicationUser> _userClaimsPrincipalFactory = userClaimsPrincipalFactory;
    private readonly IAuthorizationService _authorizationService = authorizationService;
    private readonly IOptionsMonitor<BearerTokenOptions> _bearerTokenOptions = bearerTokenOptions;
    private readonly IDataProtector _protector = dataProtectionProvider.CreateProtector("Microsoft.AspNetCore.Authentication.BearerToken");
    private readonly TimeProvider _timeProvider = timeProvider;

    public async Task<string?> GetUserNameAsync(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);

        return user?.UserName;
    }

    public async Task<(Result Result, string UserId)> CreateUserAsync(string userName, string email, string password)
    {
        var existingUser = await _userManager.FindByEmailAsync(email);
        if (existingUser != null)
        {
            return (Result.Failure(["Email is already taken."]), string.Empty);
        }

        var user = new ApplicationUser
        {
            UserName = userName,
            Email = email,
        };

        var result = await _userManager.CreateAsync(user, password);

        return (result.ToApplicationResult(), user.Id);
    }

    public async Task<bool> IsInRoleAsync(string userId, string role)
    {
        var user = await _userManager.FindByIdAsync(userId);

        return user != null && await _userManager.IsInRoleAsync(user, role);
    }

    public async Task<UserProfileInfo?> GetUserProfileAsync(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);

        return user == null
            ? null
            : new UserProfileInfo
            {
                Id = user.Id,
                UserName = user.UserName,
                Email = user.Email
            };
    }

    public async Task<bool> AuthorizeAsync(string userId, string policyName)
    {
        var user = await _userManager.FindByIdAsync(userId);

        if (user == null)
        {
            return false;
        }

        var principal = await _userClaimsPrincipalFactory.CreateAsync(user);

        var result = await _authorizationService.AuthorizeAsync(principal, policyName);

        return result.Succeeded;
    }

    public async Task<LoginResponse?> LoginAsync(string email, string password)
    {
        var user = await _userManager.FindByEmailAsync(email);
        if (user == null) return null;

        // Use CheckPasswordSignInAsync to respect lockout rules
        var result = await _signInManager.CheckPasswordSignInAsync(user, password, lockoutOnFailure: true);
        if (!result.Succeeded) return null;

        // 1. Create the Principal
        var principal = await _signInManager.CreateUserPrincipalAsync(user);

        // 2. Set Properties (Crucial for expiration!)
        var options = _bearerTokenOptions.Get(IdentityConstants.BearerScheme);
        var ticket = new AuthenticationTicket(principal, IdentityConstants.BearerScheme);

        // Add Issued and Expires dates to the ticket properties
        var utcNow = DateTimeOffset.UtcNow;
        ticket.Properties.IssuedUtc = utcNow;
        ticket.Properties.ExpiresUtc = utcNow.Add(options.BearerTokenExpiration);

        // 3. Protect using the built-in DataFormat
        // Ensure _protector is the one provided by BearerTokenOptions
        var accessToken = options.BearerTokenProtector.Protect(ticket);

        // Refresh tokens usually have a longer life
        ticket.Properties.ExpiresUtc = utcNow.Add(options.RefreshTokenExpiration);
        var refreshToken = options.RefreshTokenProtector.Protect(ticket);

        return new LoginResponse(
            "Bearer",
            accessToken,
            (long)options.BearerTokenExpiration.TotalSeconds,
            refreshToken);
    }

    public async Task<LoginResponse?> RefreshTokenAsync(string refreshToken)
    {
        var options = _bearerTokenOptions.Get(IdentityConstants.BearerScheme);
        var dataFormat = new TicketDataFormat(_protector);

        // 1. Decrypt the refresh token back into a ticket
        var ticket = dataFormat.Unprotect(refreshToken);

        if (ticket?.Principal == null || ticket.Properties.ExpiresUtc < _timeProvider.GetUtcNow())
        {
            return null; // Token is invalid or expired
        }

        // 2. Extract the user and generate a fresh ticket
        // This ensures roles/claims are updated if they changed
        var user = await _userManager.GetUserAsync(ticket.Principal);
        if (user == null) return null;

        var newPrincipal = await _userClaimsPrincipalFactory.CreateAsync(user);
        var newTicket = new AuthenticationTicket(newPrincipal, IdentityConstants.BearerScheme);

        // 3. Issue new tokens
        var newAccessToken = dataFormat.Protect(newTicket);
        var newRefreshToken = dataFormat.Protect(newTicket);

        return new LoginResponse(
            "Bearer",
            newAccessToken,
            (long)options.BearerTokenExpiration.TotalSeconds,
            newRefreshToken);
    }

    public async Task<Result> DeleteUserAsync(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);

        return user != null ? await DeleteUserAsync(user) : Result.Success();
    }

    public async Task<Result> DeleteUserAsync(ApplicationUser user)
    {
        var result = await _userManager.DeleteAsync(user);

        return result.ToApplicationResult();
    }
}
