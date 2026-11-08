namespace ClutchBids.Domain.DTOs.Admin;

public class RejectUserRequestDto
{
    public long UserId { get; set; }
    public string RejectionReason { get; set; } = string.Empty;
}
