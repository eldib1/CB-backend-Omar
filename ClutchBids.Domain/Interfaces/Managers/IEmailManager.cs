namespace ClutchBids.Domain.Interfaces.Managers;

public interface IEmailManager
{
    Task SendEmailAsync(string to, string subject, string body);
    Task SendEmailAsync(List<string> to, string subject, string body);
    Task SendUserApprovalEmailAsync(string email, string firstName);
    Task SendUserRejectionEmailAsync(string email, string firstName, string reason);
    Task SendListingApprovalEmailAsync(string email, string listingTitle);
    Task SendNewOfferNotificationEmailAsync(string sellerEmail, string buyerName, string carDetails, decimal offerAmount);
}
