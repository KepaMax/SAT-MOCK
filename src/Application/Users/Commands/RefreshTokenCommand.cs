// EXAM_SYSTEM.Application/Users/Commands/RefreshToken/RefreshTokenCommand.cs
using EXAM_SYSTEM.Application.Common.Interfaces;
using EXAM_SYSTEM.Application.Users.Commands.LoginUser;

public record RefreshTokenCommand : IRequest<LoginResponse?>
{
    public string RefreshToken { get; init; } = default!;
}

public class RefreshTokenCommandHandler : IRequestHandler<RefreshTokenCommand, LoginResponse?>
{
    private readonly IIdentityService _identityService;
    public RefreshTokenCommandHandler(IIdentityService identityService) => _identityService = identityService;

    public async Task<LoginResponse?> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
    {
        return await _identityService.RefreshTokenAsync(request.RefreshToken);
    }
}
