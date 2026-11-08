using ClutchBids.Application.Services;
using ClutchBids.Domain.Entities;
using ClutchBids.Domain.Interfaces.Managers;
using ClutchBids.Domain.Interfaces.Repositories;
using ClutchBids.Domain.Interfaces.Services;
using ClutchBids.Infrastructure.Hubs;
using ClutchBids.Infrastructure.Logging;
using ClutchBids.Infrastructure.Managers;
using ClutchBids.Infrastructure.Mappings;
using ClutchBids.Infrastructure.DBContext;
using ClutchBids.Infrastructure.Repositories;
using ClutchBids.Infrastructure.Seeding;
using ClutchBids.WebAPI.Middleware;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add DbContext
builder.Services.AddDbContext<ClutchBidsDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Add Microsoft Identity
builder.Services.AddIdentity<ApplicationUser, ApplicationRole>(options =>
{
    // Password requirements
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireUppercase = true;
    options.Password.RequireNonAlphanumeric = true;
    options.Password.RequiredLength = 8;
    
    // Email requirements
    options.SignIn.RequireConfirmedEmail = false; // Set to true to require email verification
    options.User.RequireUniqueEmail = true;
})
.AddEntityFrameworkStores<ClutchBidsDbContext>()
.AddDefaultTokenProviders();

// Add Repository Pattern
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

// Add Application Services
builder.Services.AddScoped<IIdentityService, IdentityService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<ICarListingService, CarListingService>();
builder.Services.AddScoped<IPriceOfferService, PriceOfferService>();
builder.Services.AddScoped<IAdminService, AdminService>();
builder.Services.AddScoped<ISuperAdminService, SuperAdminService>();
builder.Services.AddScoped<IDashboardService, DashboardService>();
builder.Services.AddScoped<IConfigurationService, ConfigurationService>();
builder.Services.AddScoped<IListingCodeService, ListingCodeService>();
builder.Services.AddScoped<IChatService, ChatService>();
builder.Services.AddScoped<IUserReportingService, UserReportingService>();

// Add AutoMapper
builder.Services.AddAutoMapper(typeof(IdentityMappingProfile).Assembly);

// Add Infrastructure Managers
builder.Services.AddScoped<ITokenManager, TokenManager>();
builder.Services.AddScoped<IEmailManager, EmailManager>();
builder.Services.AddScoped<IFileUploadManager, FileUploadManager>();
builder.Services.AddScoped<INotificationManager, NotificationManager>();

// Add Data Seeder
builder.Services.AddScoped<DataSeeder>();

// Add Logging
builder.Services.AddLogging(config =>
{
    config.ClearProviders();
    config.AddConsole();

    var fileLoggingEnabled = builder.Configuration.GetValue<bool>("Logging:File:Enabled");
    if (fileLoggingEnabled)
    {
        var filePath = builder.Configuration.GetValue<string>("Logging:File:Path") ?? "Logs/app.log";
        config.AddProvider(new FileLoggerProvider(filePath));
    }
});

// Add Authentication
var jwtSettings = builder.Configuration.GetSection("JwtSettings");
var secretKey = jwtSettings["SecretKey"];
builder.Services
    .AddAuthentication("Bearer")
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey ?? "")),
            ValidateIssuer = true,
            ValidIssuer = jwtSettings["Issuer"],
            ValidateAudience = true,
            ValidAudience = jwtSettings["Audience"],
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero
        };
    });

// Add Authorization
builder.Services.AddAuthorization();

// Add CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngular", builder =>
    {
        builder
            .AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader();
    });
});

// Add SignalR
builder.Services.AddSignalR();


// Add Controllers
builder.Services.AddControllers();

// Add OpenAPI
builder.Services.AddOpenApi();

// Add NSwag
builder.Services.AddOpenApiDocument();

var app = builder.Build();

// Migrate database and seed data on startup
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<ClutchBidsDbContext>();
    await dbContext.Database.MigrateAsync();
    
    // Seed initial data
    var dataSeeder = scope.ServiceProvider.GetRequiredService<DataSeeder>();
    await dataSeeder.SeedAllAsync();
}

// Configure HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseOpenApi();
    app.UseSwaggerUi();
    
    // Open browser automatically
    var addresses = app.Urls;
    if (addresses.Any())
    {
        var httpUrl = addresses.FirstOrDefault(a => a.StartsWith("http:")) 
                      ?? addresses.FirstOrDefault(a => a.StartsWith("https:"));
        if (!string.IsNullOrEmpty(httpUrl))
        {
            var swaggerUrl = httpUrl.TrimEnd('/') + "/swagger";
            try
            {
                var psi = new System.Diagnostics.ProcessStartInfo
                {
                    FileName = swaggerUrl,
                    UseShellExecute = true
                };
                System.Diagnostics.Process.Start(psi);
            }
            catch { }
        }
    }
}

// Add Global Exception Handler Middleware
app.UseMiddleware<GlobalExceptionHandlerMiddleware>();

app.UseHttpsRedirection();
app.UseCors("AllowAngular");
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapHub<NotificationHub>("/hubs/notifications");
app.MapHub<ChatHub>("/hubs/chat");

app.Run();
