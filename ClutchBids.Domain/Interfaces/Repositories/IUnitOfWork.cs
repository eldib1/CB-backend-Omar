using ClutchBids.Domain.Entities;

namespace ClutchBids.Domain.Interfaces.Repositories;

public interface IUnitOfWork : IDisposable
{
    IGenericRepository<Domain.Entities.File> Files { get; }
    IGenericRepository<Car> Cars { get; }
    IGenericRepository<CarListing> CarListings { get; }
    IGenericRepository<CarImage> CarImages { get; }
    IGenericRepository<CarDocument> CarDocuments { get; }
    IGenericRepository<CarVideo> CarVideos { get; }
    IGenericRepository<PriceOffer> PriceOffers { get; }
    IGenericRepository<ChatSession> ChatSessions { get; }
    IGenericRepository<ChatMessage> ChatMessages { get; }
    IGenericRepository<ListingCode> ListingCodes { get; }
    IGenericRepository<SystemConfiguration> SystemConfigurations { get; }
    IGenericRepository<Notification> Notifications { get; }
    IGenericRepository<UserReport> UserReports { get; }

    Task<int> SaveChangesAsync();
    Task<bool> BeginTransactionAsync();
    Task<bool> CommitTransactionAsync();
    Task<bool> RollbackTransactionAsync();
}
