using ClutchBids.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace ClutchBids.Infrastructure.DBContext;

public class ClutchBidsDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, long>
{
    public ClutchBidsDbContext(DbContextOptions<ClutchBidsDbContext> options) : base(options)
    {
    }

    public DbSet<Domain.Entities.File> Files { get; set; } = null!;
    public DbSet<Car> Cars { get; set; } = null!;
    public DbSet<CarListing> CarListings { get; set; } = null!;
    public DbSet<CarImage> CarImages { get; set; } = null!;
    public DbSet<CarDocument> CarDocuments { get; set; } = null!;
    public DbSet<CarVideo> CarVideos { get; set; } = null!;
    public DbSet<PriceOffer> PriceOffers { get; set; } = null!;
    public DbSet<ChatSession> ChatSessions { get; set; } = null!;
    public DbSet<ChatMessage> ChatMessages { get; set; } = null!;
    public DbSet<ListingCode> ListingCodes { get; set; } = null!;
    public DbSet<SystemConfiguration> SystemConfigurations { get; set; } = null!;
    public DbSet<Notification> Notifications { get; set; } = null!;
    public DbSet<UserReport> UserReports { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // ApplicationUser Configuration
        modelBuilder.Entity<ApplicationUser>(entity =>
        {
            entity.ToTable("AspNetUsers");
            entity.Property(e => e.FirstName).HasMaxLength(100);
            entity.Property(e => e.LastName).HasMaxLength(100);
            entity.Property(e => e.Address).HasMaxLength(500);
            entity.Property(e => e.Suburb).HasMaxLength(100);
            entity.Property(e => e.State).HasMaxLength(50);
            entity.Property(e => e.PostalCode).HasMaxLength(20);
            entity.Property(e => e.IdentityNumber).HasMaxLength(50);
            entity.Property(e => e.RejectionReason).HasMaxLength(500);
            entity.HasIndex(e => e.IsDeleted);
            entity.HasIndex(e => e.ProfileImageFileId);
            entity.HasIndex(e => e.IdDocumentFileId);

            entity.HasOne(e => e.ProfileImageFile)
                .WithOne()
                .HasForeignKey<ApplicationUser>(e => e.ProfileImageFileId)
                .OnDelete(DeleteBehavior.NoAction);

            entity.HasOne(e => e.IdDocumentFile)
                .WithOne()
                .HasForeignKey<ApplicationUser>(e => e.IdDocumentFileId)
                .OnDelete(DeleteBehavior.NoAction);

            // Navigation properties
            entity.HasMany(e => e.CarListings)
                .WithOne(cl => cl.Seller)
                .HasForeignKey(cl => cl.SellerId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasMany(e => e.SentOffers)
                .WithOne(po => po.Buyer)
                .HasForeignKey(po => po.BuyerId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasMany(e => e.SentMessages)
                .WithOne(cm => cm.Sender)
                .HasForeignKey(cm => cm.SenderId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasMany(e => e.ReceivedMessages)
                .WithOne(cm => cm.Receiver)
                .HasForeignKey(cm => cm.ReceiverId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // ApplicationRole Configuration
        modelBuilder.Entity<ApplicationRole>(entity =>
        {
            entity.ToTable("AspNetRoles");
            entity.Property(e => e.Description).HasMaxLength(500);
        });

        // Car Configuration
        modelBuilder.Entity<Car>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Make).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Model).IsRequired().HasMaxLength(100);
            entity.Property(e => e.VIN).HasMaxLength(50);
            entity.Property(e => e.RegistrationNumber).HasMaxLength(50);
            entity.Property(e => e.Color).HasMaxLength(50);
            entity.HasIndex(e => e.CarListingId);

            entity.HasOne(e => e.CarListing)
                .WithOne(cl => cl.Car)
                .HasForeignKey<Car>(e => e.CarListingId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // CarListing Configuration
        modelBuilder.Entity<CarListing>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.ListingTitle).IsRequired().HasMaxLength(255);
            entity.Property(e => e.StartingPrice).HasPrecision(18, 2);
            entity.Property(e => e.ReservePrice).HasPrecision(18, 2);
            entity.HasIndex(e => e.SellerId);
            entity.HasIndex(e => e.CarId);
            entity.HasIndex(e => e.Status);
            entity.HasIndex(e => e.IsDeleted);

            entity.HasOne(e => e.Car)
                .WithOne(c => c.CarListing)
                .HasForeignKey<Car>(c => c.CarListingId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.Approver)
                .WithMany(a => a.ApprovedListings)
                .HasForeignKey(ci => ci.ApproverId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.SaleProofFile)
                .WithOne(f => f.CarListingSaleProof)
                .HasForeignKey<CarListing>(e => e.SaleProofFileId)
                .OnDelete(DeleteBehavior.SetNull);

            entity.HasMany(e => e.Images)
                .WithOne(ci => ci.CarListing)
                .HasForeignKey(ci => ci.CarListingId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasMany(e => e.Documents)
                .WithOne(cd => cd.CarListing)
                .HasForeignKey(cd => cd.CarListingId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasMany(e => e.Videos)
                .WithOne(cv => cv.CarListing)
                .HasForeignKey(cv => cv.CarListingId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasMany(e => e.PriceOffers)
                .WithOne(po => po.CarListing)
                .HasForeignKey(po => po.CarListingId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasMany(e => e.ChatSessions)
                .WithOne(cs => cs.CarListing)
                .HasForeignKey(cs => cs.CarListingId)
                .OnDelete(DeleteBehavior.SetNull);
        });

        // File Configuration
        modelBuilder.Entity<Domain.Entities.File>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.FileName).IsRequired().HasMaxLength(255);
            entity.Property(e => e.FileUrl).IsRequired();
            entity.Property(e => e.FileType).IsRequired();
            entity.Property(e => e.MimeType).HasMaxLength(100);
            entity.HasIndex(e => e.UploadedById);
            entity.HasIndex(e => e.FileType);
            entity.HasIndex(e => e.IsDeleted);

            entity.HasOne(e => e.CarImage)
                .WithOne(ci => ci.File)
                .HasForeignKey<CarImage>(ci => ci.FileId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.CarVideo)
                .WithOne(cv => cv.File)
                .HasForeignKey<CarVideo>(cv => cv.FileId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.CarDocument)
                .WithOne(cd => cd.File)
                .HasForeignKey<CarDocument>(cd => cd.FileId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.CarListingSaleProof)
                .WithOne(cl => cl.SaleProofFile)
                .HasForeignKey<CarListing>(cl => cl.SaleProofFileId)
                .OnDelete(DeleteBehavior.SetNull);
        });


        // CarImage Configuration
        modelBuilder.Entity<CarImage>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.CarListingId);
            entity.HasIndex(e => e.FileId);

            entity.HasOne(e => e.CarListing)
                .WithMany(cl => cl.Images)
                .HasForeignKey(e => e.CarListingId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.File)
                .WithOne(f => f.CarImage)
                .HasForeignKey<CarImage>(e => e.FileId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // CarDocument Configuration
        modelBuilder.Entity<CarDocument>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.CarListingId);
            entity.HasIndex(e => e.FileId);

            entity.HasOne(e => e.CarListing)
                .WithMany(cl => cl.Documents)
                .HasForeignKey(e => e.CarListingId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.File)
                .WithOne(f => f.CarDocument)
                .HasForeignKey<CarDocument>(e => e.FileId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // CarVideo Configuration
        modelBuilder.Entity<CarVideo>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.CarListingId);
            entity.HasIndex(e => e.FileId);

            entity.HasOne(e => e.CarListing)
                .WithMany(cl => cl.Videos)
                .HasForeignKey(e => e.CarListingId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.File)
                .WithOne(f => f.CarVideo)
                .HasForeignKey<CarVideo>(e => e.FileId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // PriceOffer Configuration
        modelBuilder.Entity<PriceOffer>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.OfferedPrice).HasPrecision(18, 2);
            entity.HasIndex(e => e.CarListingId);
            entity.HasIndex(e => e.BuyerId);
            entity.HasIndex(e => e.Status);
        });

        // ChatSession Configuration
        modelBuilder.Entity<ChatSession>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => new { e.FirstUserId, e.SecondUserId }).IsUnique();

            entity.HasOne(e => e.FirstUser)
                .WithMany()
                .HasForeignKey(e => e.FirstUserId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.SecondUser)
                .WithMany()
                .HasForeignKey(e => e.SecondUserId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasMany(e => e.Messages)
                .WithOne(cm => cm.ChatSession)
                .HasForeignKey(cm => cm.ChatSessionId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // ChatMessage Configuration
        modelBuilder.Entity<ChatMessage>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Message).IsRequired();
            entity.HasIndex(e => e.ChatSessionId);
            entity.HasIndex(e => e.SenderId);
            entity.HasIndex(e => e.IsRead);
        });

        // ListingCode Configuration
        modelBuilder.Entity<ListingCode>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Code).IsRequired().HasMaxLength(50);
            entity.HasIndex(e => e.Code).IsUnique();
            entity.HasIndex(e => e.IsUsed);
        });

        // SystemConfiguration Configuration
        modelBuilder.Entity<SystemConfiguration>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.ConfigKey).IsRequired().HasMaxLength(100);
            entity.HasIndex(e => e.ConfigKey).IsUnique();
            entity.Property(e => e.ConfigValueType).IsRequired().HasConversion<int>();
        });

        // Notification Configuration
        modelBuilder.Entity<Notification>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Title).IsRequired().HasMaxLength(255);
            entity.Property(e => e.Message).IsRequired();
            entity.HasIndex(e => e.RecipientUserId);
            entity.HasIndex(e => e.IsRead);

            entity.HasOne(e => e.RecipientUser)
                .WithMany()
                .HasForeignKey(e => e.RecipientUserId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // UserReport Configuration
        modelBuilder.Entity<UserReport>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Reason).IsRequired().HasMaxLength(255);
            entity.Property(e => e.Description).IsRequired();
            entity.HasIndex(e => e.ReportedUserId);
            entity.HasIndex(e => e.ReportedById);
            entity.HasIndex(e => e.Status);

            entity.HasOne(e => e.ReportedBy)
                .WithMany(u => u.ReportsSubmitted)
                .HasForeignKey(e => e.ReportedById)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.ReportedUser)
                .WithMany(u => u.ReportsReceived)
                .HasForeignKey(e => e.ReportedUserId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.ReviewedBy)
                .WithMany(u => u.ReviewedReports)
                .HasForeignKey(e => e.ReviewedById)
                .OnDelete(DeleteBehavior.SetNull);
        });
    }
}
