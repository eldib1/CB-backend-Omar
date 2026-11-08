namespace ClutchBids.Domain.DTOs.Chat;

public class SendMessageRequestDto
{
    public long ChatSessionId { get; set; }
    public long ReceiverId { get; set; }
    public string Message { get; set; } = string.Empty;
}
