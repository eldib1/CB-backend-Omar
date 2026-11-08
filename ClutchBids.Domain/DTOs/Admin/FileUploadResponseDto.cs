namespace ClutchBids.Domain.DTOs.Admin;

public class FileUploadResponseDto
{
    public bool Success { get; set; }
    public string FilePath { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
}
