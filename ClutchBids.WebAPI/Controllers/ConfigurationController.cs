using ClutchBids.Domain.DTOs.Admin;
using ClutchBids.Domain.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ClutchBids.WebAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin,SuperAdmin")]
public class ConfigurationController : ControllerBase
{
    private readonly IConfigurationService _configurationService;

    public ConfigurationController(IConfigurationService configurationService)
    {
        _configurationService = configurationService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAllConfigurations()
    {
        var configurations = await _configurationService.GetAllConfigurationsAsync();
        return Ok(configurations);
    }

    [HttpGet("{configKey}")]
    public async Task<IActionResult> GetConfiguration(string configKey)
    {
        var configuration = await _configurationService.GetConfigurationAsync(configKey);
        if (configuration == null)
            return NotFound(new { message = "Configuration not found" });

        return Ok(configuration);
    }

    [HttpPut("{configKey}")]
    public async Task<IActionResult> UpdateConfiguration(string configKey, [FromBody] UpdateSystemConfigurationRequestDto request)
    {
        var configuration = await _configurationService.UpdateConfigurationAsync(configKey, request);
        if (configuration == null)
            return BadRequest(new { message = "Failed to update configuration" });

        return Ok(configuration);
    }
}
