using ClutchBids.Domain.Enums;

namespace ClutchBids.Domain.Entities;

public class SystemConfiguration : BaseEntity
{
    public string ConfigKey { get; set; }
    public string ConfigValue { get; set; }
    public string Description { get; set; }
    public ConfigValueType ConfigValueType { get; set; }
}
