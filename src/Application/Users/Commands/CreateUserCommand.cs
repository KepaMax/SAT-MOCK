using EXAM_SYSTEM.Application.Common.Interfaces;
using EXAM_SYSTEM.Application.Common.Models;
using EXAM_SYSTEM.Domain.Entities;
using MediatR;

namespace EXAM_SYSTEM.Application.Users.Commands;

public record CreateUserCommand : IRequest<(Result Succeeded, string UserId)>
{
    public required string Email { get; init; }
    public required string Password { get; init; }
    public required string FullName { get; init; } // Required for the Student entity
}

public class CreateUserCommandHandler(
    IIdentityService identityService, 
    IApplicationDbContext context) : IRequestHandler<CreateUserCommand, (Result, string)>
{
    public async Task<(Result, string)> Handle(CreateUserCommand request, CancellationToken cancellationToken)
    {
        // 1. Create the Security/Identity record (Infrastructure Layer)
        var identityResult = await identityService.CreateUserAsync(
            request.Email, 
            request.Password);
        
        if (!identityResult.Result.Succeeded)
        {
            return (identityResult.Result, string.Empty);
        }

        // 2. Create the Business/Domain record (Domain Layer)
        var student = new Student()
        {
            IdentityId = identityResult.UserId,
            FullName = request.FullName,
            SchoolId = null // We leave this null as per your "assign later" requirement
        };

        context.Students.Add(student);

        // 3. Persist the Domain record
        // In a real production app, you might wrap this in a Transaction
        await context.SaveChangesAsync(cancellationToken);

        return (identityResult.Result, identityResult.UserId);
    }
}
