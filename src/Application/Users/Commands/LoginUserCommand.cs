// EXAM_SYSTEM.Application/Users/Commands/LoginUser/LoginUserCommand.cs
using EXAM_SYSTEM.Application.Common.Interfaces;
using MediatR;

namespace EXAM_SYSTEM.Application.Users.Commands.LoginUser;

public record LoginUserCommand : IRequest<LoginResponse?>
{
    public string Email { get; init; } = default!;
    public string Password { get; init; } = default!;
}

public record LoginResponse(
    string TokenType,
    string AccessToken,
    long ExpiresIn,
    string RefreshToken);

public class LoginUserCommandHandler : IRequestHandler<LoginUserCommand, LoginResponse?>
{
    private readonly IIdentityService _identityService;

    public LoginUserCommandHandler(IIdentityService identityService) 
        => _identityService = identityService;

    public async Task<LoginResponse?> Handle(LoginUserCommand request, CancellationToken cancellationToken)
    {
        // We pass the email specifically to our service
        return await _identityService.LoginAsync(request.Email, request.Password);
    }
}
