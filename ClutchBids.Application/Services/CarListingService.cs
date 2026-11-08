using AutoMapper;
using ClutchBids.Domain.DTOs.CarListing;
using ClutchBids.Domain.Entities;
using ClutchBids.Domain.Enums;
using ClutchBids.Domain.Interfaces.Managers;
using ClutchBids.Domain.Interfaces.Repositories;
using ClutchBids.Domain.Interfaces.Services;
using Microsoft.EntityFrameworkCore;

namespace ClutchBids.Application.Services;

/// <summary>
/// Implementation of car listing service for managing vehicle listings
/// </summary>
public class CarListingService : ICarListingService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IIdentityService _identityService;
    private readonly IFileUploadManager _fileUploadService;
    private readonly INotificationManager _notificationService;
    private readonly IMapper _mapper;

    public CarListingService(
        IUnitOfWork unitOfWork,
        IIdentityService identityService,
        IFileUploadManager fileUploadService,
        INotificationManager notificationService,
        IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _identityService = identityService;
        _fileUploadService = fileUploadService;
        _notificationService = notificationService;
        _mapper = mapper;
    }

    public async Task<CarListingDto?> CreateListingAsync(long sellerId, CreateCarListingRequestDto request)
    {
        // Check if seller exists and is active
        var seller = await _identityService.FindUserByIdAsync(sellerId);
        if (seller == null || seller.Status != UserStatus.Active)
            return null;

        // Get or create car
        var car = new Car
        {
            Make = request.Make,
            Model = request.Model,
            Year = request.Year,
            VIN = request.VIN,
            RegistrationNumber = request.RegistrationNumber,
            Color = request.Color,
            BodyType = request.BodyType,
            FuelType = request.FuelType,
            Mileage = request.Mileage,
            Transmission = request.Transmission,
            EngineSize = request.EngineSize,
            Condition = request.Condition,
            Description = request.Description
        };

        await _unitOfWork.Cars.AddAsync(car);

        ListingCode? usedCode = null;
        if (!string.IsNullOrEmpty(request.ListingCodeValue))
        {
            var codes = await _unitOfWork.ListingCodes.FindAsync(lc =>
                lc.Code == request.ListingCodeValue && lc.CreatedByUserId == sellerId && !lc.IsUsed);
            usedCode = codes.FirstOrDefault();
            if (usedCode != null)
            {
                usedCode.IsUsed = true;
                usedCode.UsedAt = DateTime.UtcNow;
            }
        }

        var listing = new CarListing
        {
            SellerId = sellerId,
            ListingTitle = request.ListingTitle,
            ListingDescription = request.ListingDescription,
            StartingPrice = request.StartingPrice,
            ReservePrice = request.ReservePrice,
            Status = CarListingStatus.PendingApproval,
            ListedAt = DateTime.UtcNow
        };

        await _unitOfWork.CarListings.AddAsync(listing);
        if (usedCode != null)
        {
            await _unitOfWork.ListingCodes.UpdateAsync(usedCode);
        }

        await _unitOfWork.SaveChangesAsync();

        return _mapper.Map<CarListingDto>(listing);
    }

    public async Task<CarListingDto?> UpdateListingAsync(long listingId, long sellerId, UpdateCarListingRequestDto request)
    {
        var listing = await _unitOfWork.CarListings.GetByIdAsync(listingId);
        if (listing == null || listing.SellerId != sellerId)
            return null;

        listing.ListingTitle = request.ListingTitle;
        listing.ListingDescription = request.ListingDescription;
        listing.StartingPrice = request.StartingPrice;
        listing.ReservePrice = request.ReservePrice;

        await _unitOfWork.CarListings.UpdateAsync(listing);
        await _unitOfWork.SaveChangesAsync();

        return _mapper.Map<CarListingDto>(listing);
    }

    public async Task<bool> DeleteListingAsync(long listingId, long sellerId)
    {
        var listing = await _unitOfWork.CarListings.GetByIdAsync(listingId);
        if (listing == null || listing.SellerId != sellerId)
            return false;

        listing.IsDeleted = true;
        await _unitOfWork.CarListings.UpdateAsync(listing);
        await _unitOfWork.SaveChangesAsync();

        return true;
    }

    public async Task<CarListingDto?> GetListingAsync(long listingId)
    {
        var listing = await _unitOfWork.CarListings
            .Query()
            .Include(cl => cl.Car)
            .Include(cl => cl.Seller)
            .Include(cl => cl.Images)
            .Include(cl => cl.Documents)
            .Include(cl => cl.Videos)
            .Include(cl => cl.PriceOffers)
                .ThenInclude(po => po.Buyer)
            .FirstOrDefaultAsync(cl => cl.Id == listingId);
        if (listing == null)
            return null;

        return _mapper.Map<CarListingDto>(listing);
    }

    public async Task<IEnumerable<CarListingDto>> GetSellerListingsAsync(long sellerId)
    {
        var listings = await _unitOfWork.CarListings
            .Query()
            .Include(cl => cl.Car)
            .Include(cl => cl.Images)
            .Where(cl => cl.SellerId == sellerId)
            .OrderByDescending(cl => cl.CreatedAt)
            .ToListAsync();

        return listings.Select(listing => _mapper.Map<CarListingDto>(listing)).ToList();
    }

    public async Task<IEnumerable<CarListingDto>> GetActiveListingsAsync()
    {
        var listings = await _unitOfWork.CarListings
            .Query()
            .Include(cl => cl.Car)
            .Include(cl => cl.Seller)
            .Include(cl => cl.Images)
            .Where(cl => cl.Status == CarListingStatus.Active)
            .OrderByDescending(cl => cl.ListedAt)
            .ToListAsync();

        return listings.Select(listing => _mapper.Map<CarListingDto>(listing)).ToList();
    }

    public async Task<dynamic> SearchListingsAsync(CarListingSearchDto searchDto)
    {
        var query = _unitOfWork.CarListings
            .Query()
            .Include(cl => cl.Car)
            .Include(cl => cl.Seller)
            .Include(cl => cl.Images)
                .ThenInclude(img => img.File)
            .Include(cl => cl.PriceOffers)
            .Where(cl => cl.Status == CarListingStatus.Active);

        if (!string.IsNullOrEmpty(searchDto.Make))
            query = query.Where(cl => cl.Car != null && cl.Car.Make.Contains(searchDto.Make));

        if (!string.IsNullOrEmpty(searchDto.Model))
            query = query.Where(cl => cl.Car != null && cl.Car.Model.Contains(searchDto.Model));

        if (searchDto.YearFrom.HasValue)
            query = query.Where(cl => cl.Car != null && cl.Car.Year >= searchDto.YearFrom);

        if (searchDto.YearTo.HasValue)
            query = query.Where(cl => cl.Car != null && cl.Car.Year <= searchDto.YearTo);

        if (searchDto.PriceFrom.HasValue)
            query = query.Where(cl => cl.StartingPrice >= searchDto.PriceFrom);

        if (searchDto.PriceTo.HasValue)
            query = query.Where(cl => cl.StartingPrice <= searchDto.PriceTo);

        if (!string.IsNullOrEmpty(searchDto.FuelType))
            query = query.Where(cl => cl.Car != null && cl.Car.FuelType == searchDto.FuelType);

        if (!string.IsNullOrEmpty(searchDto.Transmission))
            query = query.Where(cl => cl.Car != null && cl.Car.Transmission == searchDto.Transmission);

        if (!string.IsNullOrEmpty(searchDto.BodyType))
            query = query.Where(cl => cl.Car != null && cl.Car.BodyType == searchDto.BodyType);

        if (!string.IsNullOrEmpty(searchDto.Suburb))
            query = query.Where(cl => cl.Seller != null && cl.Seller.Suburb == searchDto.Suburb);

        if (!string.IsNullOrEmpty(searchDto.State))
            query = query.Where(cl => cl.Seller != null && cl.Seller.State == searchDto.State);

        var pagedQuery = query
            .OrderByDescending(cl => cl.IsFeatured)
            .ThenByDescending(cl => cl.ListedAt)
            .Skip((searchDto.PageNumber - 1) * searchDto.PageSize)
            .Take(searchDto.PageSize);

        var listings = await pagedQuery.ToListAsync();
        var totalItems = await _unitOfWork.CarListings
            .Query()
            .Where(cl => cl.Status == CarListingStatus.Active)
            .CountAsync();
        var items = listings.Select(listing => _mapper.Map<CarListingDto>(listing)).ToList();

        return new
        {
            Items = items,
            TotalItems = totalItems,
            PageNumber = searchDto.PageNumber,
            PageSize = searchDto.PageSize
        };
    }

    public async Task<CarListingDto?> GetFeaturedListingAsync()
    {
        var listing = await _unitOfWork.CarListings
            .Query()
            .Include(cl => cl.Car)
            .Include(cl => cl.Seller)
            .Include(cl => cl.Images)
                .ThenInclude(img => img.File)
            .Include(cl => cl.PriceOffers)
            .Where(cl => cl.Status == CarListingStatus.Active && cl.IsFeatured)
            .OrderByDescending(cl => cl.UpdatedAt ?? cl.CreatedAt)
            .FirstOrDefaultAsync();

        return listing == null ? null : _mapper.Map<CarListingDto>(listing);
    }

    public async Task<bool> SetFeaturedAsync(long id, bool featured)
    {
        var listing = await _unitOfWork.CarListings.GetByIdAsync(id);
        if (listing == null)
            return false;

        if (featured)
        {
            // Only one listing is featured at a time
            var current = await _unitOfWork.CarListings
                .Query()
                .Where(cl => cl.IsFeatured && cl.Id != id)
                .ToListAsync();
            foreach (var other in current)
            {
                other.IsFeatured = false;
                await _unitOfWork.CarListings.UpdateAsync(other);
            }
        }

        listing.IsFeatured = featured;
        await _unitOfWork.CarListings.UpdateAsync(listing);
        await _unitOfWork.SaveChangesAsync();
        return true;
    }

    public async Task<dynamic?> GetListingDetailsAsync(long id, bool isAuthenticated)
    {
        return await GetListingAsync(id);
    }

    public async Task<bool> SubmitListingForApprovalAsync(long id, long userId)
    {
        var listing = await _unitOfWork.CarListings.GetByIdAsync(id);
        if (listing == null || listing.SellerId != userId)
            return false;
        
        listing.Status = CarListingStatus.PendingApproval;
        await _unitOfWork.CarListings.UpdateAsync(listing);
        await _unitOfWork.SaveChangesAsync();
        return true;
    }

    public async Task<bool> ApproveListingAsync(long id, long userId)
    {
        var listing = await _unitOfWork.CarListings.GetByIdAsync(id);
        if (listing == null)
            return false;
        
        listing.Status = CarListingStatus.Active;
        await _unitOfWork.CarListings.UpdateAsync(listing);
        await _unitOfWork.SaveChangesAsync();
        return true;
    }

    public async Task<bool> RejectListingAsync(long id, long userId, string reason)
    {
        var listing = await _unitOfWork.CarListings.GetByIdAsync(id);
        if (listing == null)
            return false;
        
        listing.Status = CarListingStatus.Rejected;
        await _unitOfWork.CarListings.UpdateAsync(listing);

        var chatSession = new ChatSession
        {
            FirstUserId = userId,
            SecondUserId = listing.SellerId,
            CarListingId = listing.Id,
            LastMessageAt = DateTime.UtcNow
        };

        await _unitOfWork.ChatSessions.AddAsync(chatSession);

        var message = new ChatMessage
        {
            ChatSessionId = chatSession.Id,
            SenderId = userId,
            ReceiverId = listing.SellerId,
            Message = reason,
            IsRead = false
        };

        await _unitOfWork.ChatMessages.AddAsync(message);

        await _notificationService.SendNewChatMessageNotificationAsync(listing.SellerId, userId, chatSession.Id);

        await _unitOfWork.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeactivateListingAsync(long id, long userId)
    {
        var listing = await _unitOfWork.CarListings.GetByIdAsync(id);
        if (listing == null || listing.SellerId != userId)
            return false;
        
        listing.Status = CarListingStatus.Inactive;
        await _unitOfWork.CarListings.UpdateAsync(listing);
        await _unitOfWork.SaveChangesAsync();
        return true;
    }

    public async Task<bool> MarkListingAsSoldAsync(long id, long userId)
    {
        var listing = await _unitOfWork.CarListings.GetByIdAsync(id);
        if (listing == null || listing.SellerId != userId)
            return false;
        
        listing.Status = CarListingStatus.Sold;
        await _unitOfWork.CarListings.UpdateAsync(listing);
        await _unitOfWork.SaveChangesAsync();
        return true;
    }

    public async Task<bool> UploadSaleProofAsync(long id, long userId, Stream stream, string fileName)
    {
        var listing = await _unitOfWork.CarListings.GetByIdAsync(id);
        if (listing == null || listing.SellerId != userId)
            return false;
        
        var filePath = await _fileUploadService.UploadCarPaperAsync(stream, fileName);
        
        // Create a new File entity
        var file = new Domain.Entities.File
        {
            FileName = fileName,
            FileUrl = filePath,
            FileType = FileType.Document,
            MimeType = "application/pdf",
            FileSizeInBytes = stream.Length,
            UploadedById = userId
        };
        
        await _unitOfWork.Files.AddAsync(file);
        await _unitOfWork.SaveChangesAsync();
        
        listing.SaleProofFileId = file.Id;
        await _unitOfWork.CarListings.UpdateAsync(listing);
        await _unitOfWork.SaveChangesAsync();
        return true;
    }

}

