namespace ClutchBids.Domain.DTOs.Admin;

public class ApproveUserRequestDto
{
    public long UserId { get; set; }
    public string? ApprovalNotes { get; set; }
}
