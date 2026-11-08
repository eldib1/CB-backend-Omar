using ClutchBids.Domain.Entities;
using ClutchBids.Domain.Enums;
using ClutchBids.Domain.Interfaces.Managers;
using ClutchBids.Domain.Interfaces.Repositories;
using Microsoft.Extensions.Logging;

namespace ClutchBids.Infrastructure.Managers;

public class NotificationManager : INotificationManager
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<NotificationManager> _logger;

    public NotificationManager(IUnitOfWork unitOfWork, ILogger<NotificationManager> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task SendUserApprovalNotificationAsync(long userId, string approverName)
    {
        var notification = new Notification
        {
            RecipientUserId = userId,
            NotificationType = NotificationType.UserApproved,
            Title = "Account Approved",
            Message = "Your account has been approved. You can now access all features.",
            IsRead = false
        };

        await _unitOfWork.Notifications.AddAsync(notification);
        await _unitOfWork.SaveChangesAsync();

        _logger.LogInformation($"User approval notification sent to user {userId}");
    }

    public async Task SendUserRejectionNotificationAsync(long userId, string rejectionReason)
    {
        var notification = new Notification
        {
            RecipientUserId = userId,
            NotificationType = NotificationType.UserRejected,
            Title = "Account Verification Failed",
            Message = $"Your account verification was not approved. Reason: {rejectionReason}",
            IsRead = false
        };

        await _unitOfWork.Notifications.AddAsync(notification);
        await _unitOfWork.SaveChangesAsync();

        _logger.LogInformation($"User rejection notification sent to user {userId}");
    }

    public async Task SendListingApprovalNotificationAsync(long sellerId, string listingTitle)
    {
        var notification = new Notification
        {
            RecipientUserId = sellerId,
            NotificationType = NotificationType.ListingApproved,
            Title = "Listing Approved",
            Message = $"Your listing '{listingTitle}' has been approved and is now live.",
            IsRead = false
        };

        await _unitOfWork.Notifications.AddAsync(notification);
        await _unitOfWork.SaveChangesAsync();

        _logger.LogInformation($"Listing approval notification sent to user {sellerId}");
    }

    public async Task SendNewOfferNotificationAsync(long sellerId, long buyerId, decimal offerAmount, string carDetails)
    {
        var notification = new Notification
        {
            RecipientUserId = sellerId,
            RelatedUserId = buyerId,
            NotificationType = NotificationType.NewPriceOffer,
            Title = "New Offer Received",
            Message = $"You have received a new offer of ${offerAmount:F2} for {carDetails}.",
            IsRead = false
        };

        await _unitOfWork.Notifications.AddAsync(notification);
        await _unitOfWork.SaveChangesAsync();

        _logger.LogInformation($"New offer notification sent to seller {sellerId}");
    }

    public async Task SendOfferAcceptanceNotificationAsync(long buyerId, long sellerId)
    {
        var notification = new Notification
        {
            RecipientUserId = buyerId,
            NotificationType = NotificationType.OfferAccepted,
            Title = "Offer Accepted",
            Message = "Your offer has been accepted! Proceed with the next steps.",
            IsRead = false
        };

        await _unitOfWork.Notifications.AddAsync(notification);
        await _unitOfWork.SaveChangesAsync();

        _logger.LogInformation($"Offer acceptance notification sent to buyer {buyerId}");
    }

    public async Task SendOfferRejectionNotificationAsync(long buyerId, long sellerId)
    {
        var notification = new Notification
        {
            RecipientUserId = buyerId,
            NotificationType = NotificationType.OfferRejected,
            Title = "Offer Rejected",
            Message = "Unfortunately, your offer has been rejected.",
            IsRead = false
        };

        await _unitOfWork.Notifications.AddAsync(notification);
        await _unitOfWork.SaveChangesAsync();

        _logger.LogInformation($"Offer rejection notification sent to buyer {buyerId}");
    }

    public async Task SendNewChatMessageNotificationAsync(long receiverId, long senderId, long chatSessionId)
    {
        var notification = new Notification
        {
            RecipientUserId = receiverId,
            RelatedUserId = senderId,
            NotificationType = NotificationType.NewChatMessage,
            Title = "New Message",
            Message = "You have received a new message.",
            IsRead = false
        };

        await _unitOfWork.Notifications.AddAsync(notification);
        await _unitOfWork.SaveChangesAsync();

        _logger.LogInformation($"New chat message notification sent from {senderId} to {receiverId}");
    }

    public async Task SendNewChatNotificationAsync(long userId, long otherId, string message)
    {
        var notification = new Notification
        {
            RecipientUserId = userId,
            RelatedUserId = otherId,
            NotificationType = NotificationType.NewChatMessage,
            Title = "New Chat",
            Message = message,
            IsRead = false
        };

        await _unitOfWork.Notifications.AddAsync(notification);
        await _unitOfWork.SaveChangesAsync();

        _logger.LogInformation($"New chat notification sent to user {userId}");
    }
}
