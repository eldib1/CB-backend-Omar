using ClutchBids.Domain.DTOs.Configuration;
using ClutchBids.Domain.DTOs.Admin;

namespace ClutchBids.Domain.Interfaces.Services;

/// <summary>
/// Service for handling system configuration
/// </summary>
public interface IConfigurationService
{
    Task<SystemConfigurationDto?> GetConfigurationAsync(string configKey);
    Task<SystemConfigurationDto?> UpdateConfigurationAsync(string configKey, UpdateSystemConfigurationRequestDto request);
    Task<IEnumerable<SystemConfigurationDto>> GetAllConfigurationsAsync();
}
