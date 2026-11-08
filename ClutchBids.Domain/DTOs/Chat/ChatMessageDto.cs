namespace ClutchBids.Domain.DTOs.Chat;

public class ChatMessageDto
{
    public long Id { get; set; }
    public long ChatSessionId { get; set; }
    public long SenderId { get; set; }
    public string SenderName { get; set; } = string.Empty;
    public long ReceiverId { get; set; }
    public string Message { get; set; } = string.Empty;
    public bool IsRead { get; set; }
    public DateTime CreatedAt { get; set; }
}
