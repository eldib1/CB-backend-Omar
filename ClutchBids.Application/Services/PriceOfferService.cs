using AutoMapper;
using ClutchBids.Domain.Entities;
using ClutchBids.Domain.Enums;
using ClutchBids.Domain.DTOs.PriceOffer;
using ClutchBids.Domain.Interfaces.Managers;
using ClutchBids.Domain.Interfaces.Repositories;
using ClutchBids.Domain.Interfaces.Services;

namespace ClutchBids.Application.Services;

/// <summary>
/// Implementation of price offer service for managing offers on car listings
/// </summary>
public class PriceOfferService : IPriceOfferService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IIdentityService _identityService;
    private readonly INotificationManager _notificationService;
    private readonly IEmailManager _emailService;
    private readonly IMapper _mapper;

    public PriceOfferService(
        IUnitOfWork unitOfWork,
        IIdentityService identityService,
        INotificationManager notificationService,
        IEmailManager emailService,
        IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _identityService = identityService;
        _notificationService = notificationService;
        _emailService = emailService;
        _mapper = mapper;
    }

    public async Task<PriceOfferDto?> CreateOfferAsync(long buyerId, CreatePriceOfferRequestDto request)
    {
        // Verify buyer exists and is active
        var buyer = await _identityService.FindUserByIdAsync(buyerId);
        if (buyer == null || buyer.Status != UserStatus.Active)
            return null;

        // Verify listing exists and is active
        var listing = await _unitOfWork.CarListings.GetByIdAsync(request.CarListingId);
        if (listing == null || listing.Status != CarListingStatus.Active)
            return null;

        // Prevent seller from making offers on their own listings
        if (listing.SellerId == buyerId)
            return null;

        var offer = new PriceOffer
        {
            CarListingId = request.CarListingId,
            BuyerId = buyerId,
            OfferedPrice = request.OfferedPrice,
            Status = PriceOfferStatus.Pending
        };

        await _unitOfWork.PriceOffers.AddAsync(offer);
        await _unitOfWork.SaveChangesAsync();

        // Send notification to seller
        if (listing.Seller != null && buyer != null)
        {
            await _notificationService.SendNewOfferNotificationAsync(
                listing.SellerId,
                buyerId,
                request.OfferedPrice,
                listing.ListingTitle);

            await _emailService.SendNewOfferNotificationEmailAsync(
                listing.Seller.Email,
                buyer.FirstName,
                listing.ListingTitle,
                request.OfferedPrice);
        }

        return _mapper.Map<PriceOfferDto>(offer);
    }

    public async Task<PriceOfferDto?> GetOfferAsync(long offerId)
    {
        var offer = await _unitOfWork.PriceOffers.GetByIdAsync(offerId);
        if (offer == null)
            return null;

        return _mapper.Map<PriceOfferDto>(offer);
    }

    public async Task<PriceOfferDto?> AcceptOfferAsync(long offerId, long sellerId)
    {
        var offer = await _unitOfWork.PriceOffers.GetByIdAsync(offerId);
        if (offer == null || offer.Status != PriceOfferStatus.Pending)
            return null;

        // Verify the seller owns the listing
        var listing = await _unitOfWork.CarListings.GetByIdAsync(offer.CarListingId);
        if (listing == null || listing.SellerId != sellerId)
            return null;

        offer.Status = PriceOfferStatus.Accepted;
        await _unitOfWork.PriceOffers.UpdateAsync(offer);
        await _unitOfWork.SaveChangesAsync();

        // Send notification
        if (offer.Buyer != null)
        {
            await _notificationService.SendOfferAcceptanceNotificationAsync(offer.BuyerId, sellerId);
        }

        return _mapper.Map<PriceOfferDto>(offer);
    }

    public async Task<PriceOfferDto?> RejectOfferAsync(long offerId, long sellerId, string rejectionReason)
    {
        var offer = await _unitOfWork.PriceOffers.GetByIdAsync(offerId);
        if (offer == null || offer.Status != PriceOfferStatus.Pending)
            return null;

        // Verify the seller owns the listing
        var listing = await _unitOfWork.CarListings.GetByIdAsync(offer.CarListingId);
        if (listing == null || listing.SellerId != sellerId)
            return null;

        offer.Status = PriceOfferStatus.Rejected;
        await _unitOfWork.PriceOffers.UpdateAsync(offer);
        await _unitOfWork.SaveChangesAsync();

        // Send notification
        if (offer.Buyer != null)
        {
            await _notificationService.SendOfferRejectionNotificationAsync(offer.BuyerId, sellerId);
        }

        return _mapper.Map<PriceOfferDto>(offer);
    }

    public async Task<IEnumerable<PriceOfferDto>> GetOffersByListingAsync(long listingId)
    {
        var offers = await _unitOfWork.PriceOffers.GetAllAsync();
        return _mapper.Map<IEnumerable<PriceOfferDto>>(offers.Where(o => o.CarListingId == listingId).ToList());
    }

    public async Task<IEnumerable<PriceOfferDto>> GetOffersByBuyerAsync(long buyerId)
    {
        var offers = await _unitOfWork.PriceOffers.GetAllAsync();
        return _mapper.Map<IEnumerable<PriceOfferDto>>(offers.Where(o => o.BuyerId == buyerId).ToList());
    }

    public async Task<IEnumerable<PriceOfferDto>> GetOffersBySellerAsync(long sellerId)
    {
        var offers = await _unitOfWork.PriceOffers.GetAllAsync();
        var listings = await _unitOfWork.CarListings.GetAllAsync();
        var sellerListingIds = listings
            .Where(l => l.SellerId == sellerId)
            .Select(l => l.Id)
            .ToList();

        return _mapper.Map<IEnumerable<PriceOfferDto>>(offers.Where(o => sellerListingIds.Contains(o.CarListingId)).ToList());
    }

    public async Task<bool> DeleteOfferAsync(long offerId, long buyerId)
    {
        var offer = await _unitOfWork.PriceOffers.GetByIdAsync(offerId);
        if (offer == null || offer.BuyerId != buyerId || offer.Status != PriceOfferStatus.Pending)
            return false;

        offer.Status = PriceOfferStatus.Withdrawn;
        await _unitOfWork.PriceOffers.UpdateAsync(offer);
        await _unitOfWork.SaveChangesAsync();

        return true;
    }

}
