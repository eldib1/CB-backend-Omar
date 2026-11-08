using AutoMapper;
using ClutchBids.Domain.DTOs.Admin;
using ClutchBids.Domain.DTOs.Configuration;
using ClutchBids.Domain.Interfaces.Repositories;
using ClutchBids.Domain.Interfaces.Services;
using Microsoft.Extensions.Logging;

namespace ClutchBids.Application.Services;

/// <summary>
/// Implementation of configuration service for managing system configurations
/// </summary>
public class ConfigurationService : IConfigurationService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<ConfigurationService> _logger;
    private readonly IMapper _mapper;

    public ConfigurationService(IUnitOfWork unitOfWork, ILogger<ConfigurationService> logger, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
        _mapper = mapper;
    }

    public async Task<IEnumerable<SystemConfigurationDto>> GetAllConfigurationsAsync()
    {
        var configs = await _unitOfWork.SystemConfigurations.GetAllAsync();
        return _mapper.Map<IEnumerable<SystemConfigurationDto>>(configs);
    }

    public async Task<SystemConfigurationDto?> GetConfigurationAsync(string configKey)
    {
        var configs = await _unitOfWork.SystemConfigurations.FindAsync(c => c.ConfigKey == configKey);
        var config = configs.FirstOrDefault();
        return config != null ? _mapper.Map<SystemConfigurationDto>(config) : null;
    }

    public async Task<string?> GetConfigurationValueAsync(string configKey)
    {
        var configs = await _unitOfWork.SystemConfigurations.FindAsync(c => c.ConfigKey == configKey);
        return configs.FirstOrDefault()?.ConfigValue;
    }

    public async Task<SystemConfigurationDto?> UpdateConfigurationAsync(
        string configKey,
        UpdateSystemConfigurationRequestDto request)
    {
        var configs = await _unitOfWork.SystemConfigurations.FindAsync(c => c.ConfigKey == configKey);
        var config = configs.FirstOrDefault();

        if (config == null)
        {
            _logger.LogWarning($"Configuration {configKey} not found");
            return null;
        }

        config.ConfigValue = request.ConfigValue;

        await _unitOfWork.SystemConfigurations.UpdateAsync(config);
        await _unitOfWork.SaveChangesAsync();

        _logger.LogInformation($"Configuration {configKey} updated");
        return _mapper.Map<SystemConfigurationDto>(config);
    }

    public async Task<bool> DeleteConfigurationAsync(string configKey)
    {
        var configs = await _unitOfWork.SystemConfigurations.FindAsync(c => c.ConfigKey == configKey);
        var config = configs.FirstOrDefault();

        if (config == null)
            return false;

        await _unitOfWork.SystemConfigurations.DeleteAsync(config);
        await _unitOfWork.SaveChangesAsync();

        _logger.LogInformation($"Configuration {configKey} deleted");
        return true;
    }

}
