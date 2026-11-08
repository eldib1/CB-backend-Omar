namespace ClutchBids.Domain.DTOs.Chat;

public class CreateAdminChatRequestDto
{
    public long UserId { get; set; }
    public string Message { get; set; } = string.Empty;
}
