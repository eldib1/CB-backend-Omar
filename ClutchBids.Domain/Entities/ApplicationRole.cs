using Microsoft.AspNetCore.Identity;

namespace ClutchBids.Domain.Entities;

/// <summary>
/// ApplicationRole extends IdentityRole to leverage Microsoft Identity framework
/// while maintaining custom application-specific properties
/// </summary>
public class ApplicationRole : IdentityRole<long>
{
    public string Description { get; set; }
    
    // Soft delete
    public bool IsDeleted { get; set; }
    public DateTime? DeletedAt { get; set; }
    public long? DeletedBy { get; set; }
}
