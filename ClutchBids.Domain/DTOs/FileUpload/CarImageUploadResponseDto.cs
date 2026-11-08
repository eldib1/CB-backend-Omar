namespace ClutchBids.Domain.DTOs.FileUpload;

public class CarImageUploadResponseDto
{
    public long ImageId { get; set; }
    public string ImageUrl { get; set; }
    public bool Success { get; set; }
}
