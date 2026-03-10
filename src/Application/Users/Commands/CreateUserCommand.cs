using EXAM_SYSTEM.Application.Common.Interfaces;
using EXAM_SYSTEM.Application.Common.Models;
using MediatR;

namespace EXAM_SYSTEM.Application.Users.Commands;

public record CreateUserCommand(string Username, string Email, string Password) : IRequest<(Result Succeeded, string UserId)>;

public class CreateUserCommandHandler(IIdentityService identityService) : IRequestHandler<CreateUserCommand, (Result, string)>
{
    public async Task<(Result, string)> Handle(CreateUserCommand request, CancellationToken cancellationToken)
    {
        // This calls the IdentityService method you just wrote!
        var result = await identityService.CreateUserAsync(request.Username, request.Email, request.Password);
        return (result.Result, result.UserId);
    }
}
