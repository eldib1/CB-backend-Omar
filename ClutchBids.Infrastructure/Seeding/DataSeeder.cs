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

    public async Task SeedFeaturedAndPlaceholderListingsAsync()
    {
        try
        {
            var sellers = new[] { "marcus.chen@clutchbids.com", "liam.vance@clutchbids.com", "dave.miller@clutchbids.com" };
            var sellerIds = new List<long>();
            foreach (var email in sellers)
            {
                var user = await _identityService.FindUserByEmailAsync(email);
                if (user != null) sellerIds.Add(user.Id);
            }

            if (sellerIds.Count == 0)
            {
                _logger.LogWarning("No seller users found; skipping featured/placeholder listing seed.");
                return;
            }

            var seeds = new (string Code, string Title, string Description, string Make, string Model, int Year, string Color, long Mileage, string Transmission, string Fuel, string Body, string Engine, decimal Start, decimal Reserve, bool Featured)[]
            {
                ("CB-PETROL-011", "2022 Audi RS7 Sportback Performance - 4.0L Twin-Turbo V8",
                    "Stunning 2022 Audi RS7 Sportback Performance with the 4.0L TFSI twin-turbo V8 producing 463kW. Quattro AWD, RS sport exhaust, ceramic brakes, Bang & Olufsen 3D sound and full service history.",
                    "Audi", "RS7 Sportback Performance", 2022, "Nardo Grey", 18500, "8-Speed Tiptronic", "Petrol", "Sportback", "4.0L Twin-Turbo V8", 215000m, 205000m, true),
                ("CB-PETROL-012", "2019 Ferrari 488 Pista - Rosso Corsa",
                    "Track-bred 488 Pista in Rosso Corsa with the 530kW 3.9L twin-turbo V8, 7-speed F1 dual-clutch, carbon wheels and Racing seats. Placeholder listing.",
                    "Ferrari", "488 Pista", 2019, "Rosso Corsa", 6200, "7-Speed Dual-Clutch", "Petrol", "Coupe", "3.9L Twin-Turbo V8", 625000m, 600000m, false),
                ("CB-PETROL-013", "2018 Porsche 911 GT2 RS Weissach Package",
                    "Rear-drive monster with the 515kW 3.8L twin-turbo flat-six, 7-speed PDK and the Weissach package. Placeholder listing.",
                    "Porsche", "911 GT2 RS", 2018, "GT Silver Metallic", 4100, "7-Speed PDK", "Petrol", "Coupe", "3.8L Twin-Turbo Flat-Six", 595000m, 570000m, false),
                ("CB-PETROL-014", "2021 BMW M2 CS - Misano Blue",
                    "Limited-run M2 CS in Misano Blue with the 331kW S55 3.0L twin-turbo inline-six, carbon roof and M adaptive suspension. Placeholder listing.",
                    "BMW", "M2 CS", 2021, "Misano Blue", 12800, "6-Speed Manual", "Petrol", "Coupe", "3.0L Twin-Turbo Inline-6", 118000m, 110000m, false),
                ("CB-PETROL-015", "2020 BMW M5 Competition",
                    "M5 Competition with the 460kW 4.4L twin-turbo V8, M xDrive AWD and Carbon Exterior package. Placeholder listing.",
                    "BMW", "M5 Competition", 2020, "Black Sapphire", 33400, "8-Speed Automatic", "Petrol", "Sedan", "4.4L Twin-Turbo V8", 142000m, 135000m, false),
                ("CB-PETROL-016", "2023 Chevrolet Corvette Z06 Convertible 2LT",
                    "Z06 Convertible 2LT with the flat-plane crank 5.5L LT6 V8, 8-speed dual-clutch and Z07 track pack. Placeholder listing.",
                    "Chevrolet", "Corvette Z06 Convertible 2LT", 2023, "Torch Red", 5400, "8-Speed Dual-Clutch", "Petrol", "Convertible", "5.5L LT6 V8", 235000m, 225000m, false),
                ("CB-PETROL-017", "2022 Maserati MC20 - Bianco Audace",
                    "Mid-engine MC20 with the 463kW 3.0L Nettuno twin-turbo V6, carbon tub and butterfly doors. Placeholder listing.",
                    "Maserati", "MC20", 2022, "Bianco Audace", 7900, "8-Speed Dual-Clutch", "Petrol", "Coupe", "3.0L Twin-Turbo V6 (Nettuno)", 395000m, 380000m, false)
            };

            var existing = (await _unitOfWork.CarListings.GetAllAsync()).Select(l => l.ListingCode).ToHashSet();
            var created = 0;

            for (var i = 0; i < seeds.Length; i++)
            {
                var s = seeds[i];
                if (existing.Contains(s.Code)) continue;

                var listedAt = DateTime.UtcNow.AddDays(-(i + 1));
                var listing = new CarListing
                {
                    SellerId = sellerIds[i % sellerIds.Count],
                    StartingPrice = s.Start,
                    ReservePrice = s.Reserve,
                    ListingTitle = s.Title,
                    ListingDescription = s.Description,
                    Status = CarListingStatus.Active,
                    ListedAt = listedAt,
                    ApprovedAt = listedAt,
                    ListingCode = s.Code,
                    IsFeatured = s.Featured
                };
                await _unitOfWork.CarListings.AddAsync(listing);
                await _unitOfWork.SaveChangesAsync();

                var car = new Car
                {
                    CarListingId = listing.Id,
                    Make = s.Make,
                    Model = s.Model,
                    Year = s.Year,
                    Color = s.Color,
                    VIN = $"CBVIN{s.Year}{i + 11:D4}",
                    RegistrationNumber = $"CB{i + 11:D3}",
                    Mileage = s.Mileage,
                    Transmission = s.Transmission,
                    FuelType = s.Fuel,
                    BodyType = s.Body,
                    EngineSize = s.Engine,
                    Condition = "Excellent",
                    Description = s.Description
                };
                await _unitOfWork.Cars.AddAsync(car);
                await _unitOfWork.SaveChangesAsync();

                listing.CarId = car.Id;
                await _unitOfWork.CarListings.UpdateAsync(listing);
                await _unitOfWork.SaveChangesAsync();
                created++;
            }

            _logger.LogInformation($"Featured/placeholder listings seeded: {created} created.");
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error seeding featured/placeholder listings: {ex.Message}");
        }
    }

    public async Task SeedAllAsync()
    {
        _logger.LogInformation("Starting data seeding...");
        await SeedRolesAsync();
        await SeedSystemConfigurationsAsync();
        await SeedSuperAdminAsync();
        await SeedFeaturedAndPlaceholderListingsAsync();
        _logger.LogInformation("Data seeding completed.");
    }
}
