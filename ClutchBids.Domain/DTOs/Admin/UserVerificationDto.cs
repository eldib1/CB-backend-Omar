namespace ClutchBids.Domain.DTOs.Admin;

public class UserVerificationDto
{
    public long UserId { get; set; }
    public string Email { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public string? IdDocumentUrl { get; set; }
    public string IdentityNumber { get; set; } = string.Empty;
    public int Status { get; set; }
    public DateTime CreatedAt { get; set; }
}
