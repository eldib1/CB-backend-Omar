namespace ClutchBids.Domain.Interfaces.Managers;

public interface INotificationManager
{
    Task SendUserApprovalNotificationAsync(long userId, string approverName);
    Task SendUserRejectionNotificationAsync(long userId, string rejectionReason);
    Task SendListingApprovalNotificationAsync(long sellerId, string listingTitle);
    Task SendNewOfferNotificationAsync(long sellerId, long buyerId, decimal offerAmount, string carDetails);
    Task SendOfferAcceptanceNotificationAsync(long buyerId, long sellerId);
    Task SendOfferRejectionNotificationAsync(long buyerId, long sellerId);
    Task SendNewChatMessageNotificationAsync(long receiverId, long senderId, long chatSessionId);
    Task SendNewChatNotificationAsync(long userId, long otherId, string message);
}
