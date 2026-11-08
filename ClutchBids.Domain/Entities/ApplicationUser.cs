using Microsoft.AspNetCore.Identity;
using ClutchBids.Domain.Enums;

namespace ClutchBids.Domain.Entities;

/// <summary>
/// ApplicationUser extends IdentityUser to leverage Microsoft Identity framework
/// while maintaining custom application-specific properties
/// </summary>
public class ApplicationUser : IdentityUser<long>
{
    // Custom properties specific to ClutchBids
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public long? ProfileImageFileId { get; set; }
    public string Address { get; set; }
    public string Suburb { get; set; }
    public string State { get; set; }
    public string PostalCode { get; set; }
    public UserStatus Status { get; set; }
    public DateTime? ApprovedAt { get; set; }
    public long? ApprovedBy { get; set; }
    public string RejectionReason { get; set; }
    public long? IdDocumentFileId { get; set; }
    public string IdentityNumber { get; set; }
    public DateTime? IdentityExpiryDate { get; set; }
    
    // Soft delete
    public bool IsDeleted { get; set; }
    public DateTime? DeletedAt { get; set; }
    public long? DeletedBy { get; set; }

    // Navigation properties
    public File ProfileImageFile { get; set; }
    public File IdDocumentFile { get; set; }
    public ICollection<CarListing> CarListings { get; set; }
    public ICollection<PriceOffer> ReceivedOffers { get; set; }
    public ICollection<PriceOffer> SentOffers { get; set; }
    public ICollection<ChatMessage> SentMessages { get; set; }
    public ICollection<ChatMessage> ReceivedMessages { get; set; }
    public ICollection<UserReport> ReportsSubmitted { get; set; }
    public ICollection<UserReport> ReportsReceived { get; set; }
    public ICollection<UserReport> ReviewedReports { get; set; }
    public ICollection<CarListing> ApprovedListings { get; set; }
}
