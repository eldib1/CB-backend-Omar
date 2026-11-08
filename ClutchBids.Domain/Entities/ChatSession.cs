namespace ClutchBids.Domain.Entities;

public class ChatSession : BaseEntity
{
    public long FirstUserId { get; set; }
    public long SecondUserId { get; set; }
    public long? CarListingId { get; set; }
    public DateTime LastMessageAt { get; set; }

    // Navigation properties
    public ApplicationUser FirstUser { get; set; }
    public ApplicationUser SecondUser { get; set; }
    public CarListing CarListing { get; set; }
    public ICollection<ChatMessage> Messages { get; set; } = [];
}
