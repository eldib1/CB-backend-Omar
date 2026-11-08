using ClutchBids.Domain.Entities;
using ClutchBids.Domain.Interfaces.Repositories;
using ClutchBids.Infrastructure.DBContext;
using Microsoft.EntityFrameworkCore.Storage;

namespace ClutchBids.Infrastructure.Repositories;

public class UnitOfWork : IUnitOfWork
{
    private readonly ClutchBidsDbContext _context;
    private IDbContextTransaction _transaction;

    public IGenericRepository<Domain.Entities.File> Files { get; }
    public IGenericRepository<Car> Cars { get; }
    public IGenericRepository<CarListing> CarListings { get; }
    public IGenericRepository<CarImage> CarImages { get; }
    public IGenericRepository<CarDocument> CarDocuments { get; }
    public IGenericRepository<CarVideo> CarVideos { get; }
    public IGenericRepository<PriceOffer> PriceOffers { get; }
    public IGenericRepository<ChatSession> ChatSessions { get; }
    public IGenericRepository<ChatMessage> ChatMessages { get; }
    public IGenericRepository<ListingCode> ListingCodes { get; }
    public IGenericRepository<SystemConfiguration> SystemConfigurations { get; }
    public IGenericRepository<Notification> Notifications { get; }
    public IGenericRepository<UserReport> UserReports { get; }

    public UnitOfWork(ClutchBidsDbContext context)
    {
        _context = context;
        Files = new GenericRepository<Domain.Entities.File>(context);
        Cars = new GenericRepository<Car>(context);
        CarListings = new GenericRepository<CarListing>(context);
        CarImages = new GenericRepository<CarImage>(context);
        CarDocuments = new GenericRepository<CarDocument>(context);
        CarVideos = new GenericRepository<CarVideo>(context);
        PriceOffers = new GenericRepository<PriceOffer>(context);
        ChatSessions = new GenericRepository<ChatSession>(context);
        ChatMessages = new GenericRepository<ChatMessage>(context);
        ListingCodes = new GenericRepository<ListingCode>(context);
        SystemConfigurations = new GenericRepository<SystemConfiguration>(context);
        Notifications = new GenericRepository<Notification>(context);
        UserReports = new GenericRepository<UserReport>(context);
    }

    public async Task<int> SaveChangesAsync()
    {
        return await _context.SaveChangesAsync();
    }

    public async Task<bool> BeginTransactionAsync()
    {
        _transaction = await _context.Database.BeginTransactionAsync();
        return true;
    }

    public async Task<bool> CommitTransactionAsync()
    {
        try
        {
            await _context.SaveChangesAsync();
            await _transaction?.CommitAsync()!;
            return true;
        }
        catch
        {
            await RollbackTransactionAsync();
            throw;
        }
        finally
        {
            _transaction?.Dispose();
            _transaction = null;
        }
    }

    public async Task<bool> RollbackTransactionAsync()
    {
        try
        {
            await _transaction?.RollbackAsync()!;
            return true;
        }
        finally
        {
            _transaction?.Dispose();
            _transaction = null;
        }
    }

    public void Dispose()
    {
        _transaction?.Dispose();
        _context.Dispose();
    }
}
