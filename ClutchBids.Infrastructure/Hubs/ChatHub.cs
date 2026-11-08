using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;

namespace ClutchBids.Infrastructure.Hubs;

public class ChatHub : Hub
{
    private readonly ILogger<ChatHub> _logger;

    public ChatHub(ILogger<ChatHub> logger)
    {
        _logger = logger;
    }

    public override async Task OnConnectedAsync()
    {
        var userId = Context.User?.FindFirst("sub")?.Value ?? Context.ConnectionId;
        await Groups.AddToGroupAsync(Context.ConnectionId, $"user-{userId}");
        _logger.LogInformation($"User {userId} connected to chat");
        await base.OnConnectedAsync();
    }

    public async Task JoinChatSession(string chatSessionId)
    {
        var userId = Context.User?.FindFirst("sub")?.Value ?? Context.ConnectionId;
        await Groups.AddToGroupAsync(Context.ConnectionId, $"chat-{chatSessionId}");
        await Clients.Group($"chat-{chatSessionId}").SendAsync("UserJoined", userId);
        _logger.LogInformation($"User {userId} joined chat session {chatSessionId}");
    }

    public async Task SendMessage(string chatSessionId, string senderId, string receiverId, string message)
    {
        await Clients.Group($"chat-{chatSessionId}").SendAsync("ReceiveMessage", new
        {
            SenderId = senderId,
            ReceiverId = receiverId,
            Message = message,
            SentAt = DateTime.UtcNow
        });

        // Also notify the receiver in their personal group
        await Clients.Group($"user-{receiverId}").SendAsync("NewChatMessage", new
        {
            SenderId = senderId,
            ChatSessionId = chatSessionId,
            Message = message
        });

        _logger.LogInformation($"Message sent in chat session {chatSessionId}");
    }

    public async Task MarkMessageAsRead(string chatSessionId, string messageId)
    {
        await Clients.Group($"chat-{chatSessionId}").SendAsync("MessageRead", messageId);
    }

    public async Task LeaveChatSession(string chatSessionId)
    {
        var userId = Context.User?.FindFirst("sub")?.Value ?? Context.ConnectionId;
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"chat-{chatSessionId}");
        await Clients.Group($"chat-{chatSessionId}").SendAsync("UserLeft", userId);
        _logger.LogInformation($"User {userId} left chat session {chatSessionId}");
    }
}
