using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using ClutchBids.Domain.Interfaces.Managers;

namespace ClutchBids.Infrastructure.Managers;

public class FileUploadManager : IFileUploadManager
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<FileUploadManager> _logger;

    public FileUploadManager(IConfiguration configuration, ILogger<FileUploadManager> logger)
    {
        _configuration = configuration;
        _logger = logger;
    }

    public async Task<string> UploadIdDocumentAsync(Stream fileStream, string fileName)
    {
        return await UploadFileAsync(fileStream, fileName, "FileUpload:IdDocumentsPath");
    }

    public async Task<string> UploadCarPaperAsync(Stream fileStream, string fileName)
    {
        return await UploadFileAsync(fileStream, fileName, "FileUpload:CarPapersPath");
    }

    public async Task<string> UploadCarImageAsync(Stream fileStream, string fileName)
    {
        return await UploadFileAsync(fileStream, fileName, "FileUpload:CarImagesPath");
    }

    public async Task<string> UploadCarVideoAsync(Stream fileStream, string fileName)
    {
        return await UploadFileAsync(fileStream, fileName, "FileUpload:CarVideosPath");
    }

    public async Task<bool> DeleteFileAsync(string filePath)
    {
        try
        {
            var fullPath = Path.Combine(Directory.GetCurrentDirectory(), filePath);
            if (File.Exists(fullPath))
            {
                File.Delete(fullPath);
                _logger.LogInformation($"File deleted: {fullPath}");
                return true;
            }

            return false;
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error deleting file: {ex.Message}");
            return false;
        }
    }

    public async Task<Stream?> DownloadFileAsync(string filePath)
    {
        try
        {
            var fullPath = Path.Combine(Directory.GetCurrentDirectory(), filePath);
            if (File.Exists(fullPath))
            {
                return File.OpenRead(fullPath);
            }

            return null;
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error downloading file: {ex.Message}");
            return null;
        }
    }

    private async Task<string> UploadFileAsync(Stream fileStream, string fileName, string configKey)
    {
        try
        {
            var uploadPath = _configuration[configKey] ?? "uploads";
            var fullPath = Path.Combine(Directory.GetCurrentDirectory(), uploadPath);

            if (!Directory.Exists(fullPath))
            {
                Directory.CreateDirectory(fullPath);
            }

            var filePath = Path.Combine(fullPath, fileName);
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await fileStream.CopyToAsync(stream);
            }

            _logger.LogInformation($"File uploaded: {filePath}");
            return Path.Combine(uploadPath, fileName);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error uploading file: {ex.Message}");
            throw;
        }
    }
}
