using ClutchBids.Domain.Enums;

namespace ClutchBids.Domain.DTOs.Configuration;

public class SystemConfigurationDto
{
    public long Id { get; set; }
    public string ConfigKey { get; set; } = string.Empty;
    public string ConfigValue { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public ConfigValueType ConfigValueType { get; set; }
    public bool RequiresListingCode { get; set; }
    public int MaxListingsPerUser { get; set; }
}
