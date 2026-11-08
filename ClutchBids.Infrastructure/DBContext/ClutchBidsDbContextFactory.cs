using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace ClutchBids.Infrastructure.DBContext;

public class ClutchBidsDbContextFactory : IDesignTimeDbContextFactory<ClutchBidsDbContext>
{
    public ClutchBidsDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<ClutchBidsDbContext>();
        
        // Read connection string from appsettings.json
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Path.Combine(Directory.GetCurrentDirectory(), "../ClutchBids.WebAPI"))
            .AddJsonFile("appsettings.json")
            .Build();

        var connectionString = configuration.GetConnectionString("DefaultConnection");
        optionsBuilder.UseSqlServer(connectionString);

        return new ClutchBidsDbContext(optionsBuilder.Options);
    }
}
