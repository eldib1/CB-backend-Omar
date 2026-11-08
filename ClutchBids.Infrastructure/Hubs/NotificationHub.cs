using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;

namespace ClutchBids.Infrastructure.Hubs;

public class NotificationHub : Hub
{
    private readonly ILogger<NotificationHub> _logger;

    public NotificationHub(ILogger<NotificationHub> logger)
    {
        _logger = logger;
    }

    public override async Task OnConnectedAsync()
    {
        var userId = Context.User?.FindFirst("sub")?.Value ?? Context.ConnectionId;
        await Groups.AddToGroupAsync(Context.ConnectionId, $"user-{userId}");
        _logger.LogInformation($"User {userId} connected to notifications");
        await base.OnConnectedAsync();
    }

    public async Task SendNotificationToUser(string userId, string title, string message)
    {
        await Clients.Group($"user-{userId}").SendAsync("ReceiveNotification", new
        {
            Title = title,
            Message = message,
            CreatedAt = DateTime.UtcNow
        });
    }

    public async Task MarkNotificationAsRead(string notificationId)
    {
        var userId = Context.User?.FindFirst("sub")?.Value ?? Context.ConnectionId;
        _logger.LogInformation($"Notification {notificationId} marked as read by user {userId}");
        await Task.CompletedTask;
    }
}
