namespace ClutchBids.Domain.DTOs.Chat;

public class ChatSessionDto
{
    public long Id { get; set; }
    public long FirstUserId { get; set; }
    public string FirstUserName { get; set; } = string.Empty;
    public long SecondUserId { get; set; }
    public string SecondUserName { get; set; } = string.Empty;
    public long? CarListingId { get; set; }
    public string? CarTitle { get; set; }
    public DateTime LastMessageAt { get; set; }
    public List<ChatMessageDto> Messages { get; set; } = [];
}
