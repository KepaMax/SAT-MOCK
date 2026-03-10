using System.Diagnostics; // For timing the action
using EXAM_SYSTEM.Application.Common.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace EXAM_SYSTEM.Application.Common.Behaviours;

public class LoggingBehaviour<TRequest, TResponse>(ILogger<TRequest> logger, IUser user, IIdentityService identityService) : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    private readonly ILogger<TRequest> _logger = logger;
    private readonly IUser _user = user;
    private readonly IIdentityService _identityService = identityService;

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        var requestName = typeof(TRequest).Name;
        var userId = _user.Id ?? string.Empty;
        string? userName = string.Empty;

        if (!string.IsNullOrEmpty(userId))
        {
            userName = await _identityService.GetUserNameAsync(userId);
        }

        // 1. Log the Start
        _logger.LogInformation("Starting Request: {Name} {@UserId} {@UserName} {@Request}",
            requestName, userId, userName, request);

        var timer = Stopwatch.StartNew();

        try
        {
            // 2. Execute the actual Action (Command/Query)
            var response = await next();

            timer.Stop();

            // 3. Log Success & Duration
            _logger.LogInformation("Completed Request: {Name} in {ElapsedMilliseconds}ms",
                requestName, timer.ElapsedMilliseconds);

            return response;
        }
        catch (Exception ex)
        {
            timer.Stop();
            // 4. Log Failure
            _logger.LogError(ex, "Request Failed: {Name} after {ElapsedMilliseconds}ms",
                requestName, timer.ElapsedMilliseconds);
            throw;
        }
    }
}
