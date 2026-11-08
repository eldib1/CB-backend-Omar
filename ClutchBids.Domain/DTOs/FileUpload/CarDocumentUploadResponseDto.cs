namespace ClutchBids.Domain.DTOs.FileUpload;

public class CarDocumentUploadResponseDto
{
    public long DocumentId { get; set; }
    public string DocumentUrl { get; set; }
    public bool Success { get; set; }
}
