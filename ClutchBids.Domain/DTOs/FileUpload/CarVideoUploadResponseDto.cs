namespace ClutchBids.Domain.DTOs.FileUpload;

public class CarVideoUploadResponseDto
{
    public long VideoId { get; set; }
    public string VideoUrl { get; set; }
    public string ThumbnailUrl { get; set; }
    public bool Success { get; set; }
}
