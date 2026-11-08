using ClutchBids.Domain.Enums;

namespace ClutchBids.Domain.Entities;

public class File : BaseEntity
{
    public string FileName { get; set; }
    public string FileUrl { get; set; }
    public FileType FileType { get; set; }
    public string MimeType { get; set; }
    public long FileSizeInBytes { get; set; }
    public long? UploadedById { get; set; }

    // Navigation properties (one-to-one, optional)
    public ApplicationUser UploadedBy { get; set; }
    public CarListing CarListingSaleProof { get; set; }
    public CarImage CarImage { get; set; }
    public CarVideo CarVideo { get; set; }
    public CarDocument CarDocument { get; set; }
}
