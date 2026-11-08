namespace ClutchBids.Domain.Entities;

public class ChatMessage : BaseEntity
{
    public long ChatSessionId { get; set; }
    public long SenderId { get; set; }
    public long ReceiverId { get; set; }
    public string Message { get; set; }
    public bool IsRead { get; set; }
    public DateTime? ReadAt { get; set; }

    // Navigation properties
    public ChatSession ChatSession { get; set; }
    public ApplicationUser Sender { get; set; }
    public ApplicationUser Receiver { get; set; }
}
