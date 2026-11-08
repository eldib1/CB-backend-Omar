namespace ClutchBids.Domain.Interfaces.Managers;

public interface IFileUploadManager
{
    Task<string> UploadIdDocumentAsync(Stream fileStream, string fileName);
    Task<string> UploadCarPaperAsync(Stream fileStream, string fileName);
    Task<string> UploadCarImageAsync(Stream fileStream, string fileName);
    Task<string> UploadCarVideoAsync(Stream fileStream, string fileName);
    Task<bool> DeleteFileAsync(string filePath);
    Task<Stream?> DownloadFileAsync(string filePath);
}
