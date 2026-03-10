using EXAM_SYSTEM.Application.Common.Interfaces;
using EXAM_SYSTEM.Application.Common.Models;
using MediatR;

namespace EXAM_SYSTEM.Application.Users.Queries.GetProfile;

public record GetProfileQuery : IRequest<UserProfileInfo>;

public class GetProfileQueryHandler : IRequestHandler<GetProfileQuery, UserProfileInfo>
{
    private readonly IIdentityService _identityService;
    private readonly IUser _user; // This is your ICurrentUserService

    public GetProfileQueryHandler(IIdentityService identityService, IUser user)
    {
        _identityService = identityService;
        _user = user;
    }

    public async Task<UserProfileInfo> Handle(GetProfileQuery request, CancellationToken cancellationToken)
    {
        // _user.Id is automatically extracted from the HttpContext Claims by your Infrastructure
        var userId = _user.Id;

        if (string.IsNullOrEmpty(userId))
            throw new UnauthorizedAccessException();

        var profile = await _identityService.GetUserProfileAsync(userId);

        if (profile == null)
            throw new Exception("User not found"); // Or a custom NotFoundException

        return profile;
    }
}
