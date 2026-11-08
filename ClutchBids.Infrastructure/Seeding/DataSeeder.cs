using Microsoft.Extensions.Logging;
using ClutchBids.Domain.Entities;
using ClutchBids.Domain.Enums;
using ClutchBids.Domain.Interfaces.Services;
using ClutchBids.Domain.Interfaces.Repositories;

namespace ClutchBids.Infrastructure.Seeding;

public class DataSeeder
{
    private readonly IIdentityService _identityService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<DataSeeder> _logger;

    public DataSeeder(IIdentityService identityService, IUnitOfWork unitOfWork, ILogger<DataSeeder> logger)
    {
        _identityService = identityService;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task SeedRolesAsync()
    {
        try
        {
            var roles = new[] { "SuperAdmin", "Admin", "User" };
            var existingRoles = await _identityService.GetAllRolesAsync();

            foreach (var roleName in roles)
            {
                var existingRole = existingRoles.FirstOrDefault(r => r.Name == roleName);
                if (existingRole == null)
                {
                    var role = new ApplicationRole
                    {
                        Name = roleName,
                        Description = $"{roleName} role for ClutchBids system"
                    };

                    var result = await _identityService.CreateRoleAsync(role);
                    if (result.Succeeded)
                    {
                        _logger.LogInformation($"Role '{roleName}' seeded successfully.");
                    }
                    else
                    {
                        _logger.LogError($"Failed to create role '{roleName}': {string.Join(", ", result.Errors.Select(e => e.Description))}");
                    }
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error seeding roles: {ex.Message}");
        }
    }

    public async Task SeedSystemConfigurationsAsync()
    {
        try
        {
            var configs = new[]
            {
                new SystemConfiguration
                {
                    ConfigKey = "Require Listing Code",
                    ConfigValue = "true",
                    Description = "Determines whether a listing code is required when creating a car listing. Set to 'true' to enforce listing code requirement.",
                    ConfigValueType = ConfigValueType.Boolean
                },
                new SystemConfiguration
                {
                    ConfigKey = "Max Images Per Listing",
                    ConfigValue = "10",
                    Description = "Maximum number of images allowed per car listing.",
                    ConfigValueType = ConfigValueType.Number
                },
                new SystemConfiguration
                {
                    ConfigKey = "Max Documents Per Listing",
                    ConfigValue = "5",
                    Description = "Maximum number of documents allowed per car listing.",
                    ConfigValueType = ConfigValueType.Number
                },
                new SystemConfiguration
                {
                    ConfigKey = "Auto Approve Listing",
                    ConfigValue = "false",
                    Description = "Automatically approves new listings without admin review. Set to 'true' to enable auto-approval.",
                    ConfigValueType = ConfigValueType.Boolean
                },
                new SystemConfiguration
                {
                    ConfigKey = "Minimum Listing Price",
                    ConfigValue = "100",
                    Description = "Minimum price allowed for a car listing.",
                    ConfigValueType = ConfigValueType.Number
                }
            };

            var existingConfigs = await _unitOfWork.SystemConfigurations.GetAllAsync();

            foreach (var config in configs)
            {
                var existingConfig = existingConfigs.FirstOrDefault(c => c.ConfigKey == config.ConfigKey);
                if (existingConfig == null)
                {
                    await _unitOfWork.SystemConfigurations.AddAsync(config);
                    _logger.LogInformation($"System configuration '{config.ConfigKey}' seeded successfully.");
                }
            }

            await _unitOfWork.SaveChangesAsync();
            _logger.LogInformation("All system configurations seeded successfully.");
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error seeding system configurations: {ex.Message}");
        }
    }

    public async Task SeedSuperAdminAsync()
    {
        try
        {
            var superAdminEmail = "superadmin@clutchbids.com";
            var existingSuperAdmin = await _identityService.FindUserByEmailAsync(superAdminEmail);

            if (existingSuperAdmin == null)
            {
                var superAdmin = new ApplicationUser
                {
                    UserName = superAdminEmail,
                    Email = superAdminEmail,
                    FirstName = "Super",
                    LastName = "Admin",
                    PhoneNumber = "0000000000",
                    Status = UserStatus.Active,
                    EmailConfirmed = true
                };

                var result = await _identityService.CreateUserAsync(superAdmin, "SuperAdmin@123");
                if (result.Succeeded)
                {
                    // Assign SuperAdmin role
                    var roleResult = await _identityService.AddUserToRoleAsync(superAdmin, "SuperAdmin");
                    if (roleResult.Succeeded)
                    {
                        _logger.LogInformation("SuperAdmin user seeded successfully.");
                    }
                    else
                    {
                        _logger.LogError($"Failed to assign SuperAdmin role: {string.Join(", ", roleResult.Errors.Select(e => e.Description))}");
                    }
                }
                else
                {
                    _logger.LogError($"Failed to create SuperAdmin user: {string.Join(", ", result.Errors.Select(e => e.Description))}");
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error seeding super admin: {ex.Message}");
        }
    }

    public async Task SeedAllAsync()
    {
        _logger.LogInformation("Starting data seeding...");
        await SeedRolesAsync();
        await SeedSystemConfigurationsAsync();
        await SeedSuperAdminAsync();
        _logger.LogInformation("Data seeding completed.");
    }
}
