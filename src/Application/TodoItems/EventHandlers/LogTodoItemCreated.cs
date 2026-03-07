using EXAM_SYSTEM.Domain.Events;
using Microsoft.Extensions.Logging;

namespace EXAM_SYSTEM.Application.TodoItems.EventHandlers;

public class LogTodoItemCreated : INotificationHandler<TodoItemCreatedEvent>
{
    private readonly ILogger<LogTodoItemCreated> _logger;

    public LogTodoItemCreated(ILogger<LogTodoItemCreated> logger)
    {
        _logger = logger;
    }

    public Task Handle(TodoItemCreatedEvent notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation("EXAM_SYSTEM Domain Event: {DomainEvent}", notification.GetType().Name);

        return Task.CompletedTask;
    }
}
