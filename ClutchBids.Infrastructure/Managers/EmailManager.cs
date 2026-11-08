using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using ClutchBids.Domain.Interfaces.Managers;

namespace ClutchBids.Infrastructure.Managers;

public class EmailManager : IEmailManager
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<EmailManager> _logger;

    public EmailManager(IConfiguration configuration, ILogger<EmailManager> logger)
    {
        _configuration = configuration;
        _logger = logger;
    }

    public async Task SendEmailAsync(string to, string subject, string body)
    {
        await SendEmailAsync(new List<string> { to }, subject, body);
    }

    public async Task SendEmailAsync(List<string> to, string subject, string body)
    {
        try
        {
            // TODO: Implement SMTP email sending using System.Net.Mail or a third-party service
            // For now, this is a placeholder
            _logger.LogInformation($"Email would be sent to {string.Join(", ", to)} with subject: {subject}");
            await Task.CompletedTask;
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error sending email: {ex.Message}");
            throw;
        }
    }

    public async Task SendUserApprovalEmailAsync(string email, string firstName)
    {
        var subject = "Your ClutchBids Account Has Been Approved";
        var body = $@"<h2>Welcome to ClutchBids, {firstName}!</h2><p>Your account has been approved and you can now access all features of ClutchBids.</p>";
        await SendEmailAsync(email, subject, body);
    }

    public async Task SendUserRejectionEmailAsync(string email, string firstName, string reason)
    {
        var subject = "ClutchBids Account Verification - Action Required";
        var body = $@"<h2>Account Verification Update</h2><p>Hello {firstName},</p><p>Unfortunately, your account verification was not approved.</p><p><strong>Reason:</strong> {reason}</p>";
        await SendEmailAsync(email, subject, body);
    }

    public async Task SendListingApprovalEmailAsync(string email, string listingTitle)
    {
        var subject = "Your Listing Has Been Approved";
        var body = $@"<h2>Listing Approved</h2><p>Your listing '{listingTitle}' has been approved and is now live on ClutchBids.</p>";
        await SendEmailAsync(email, subject, body);
    }

    public async Task SendNewOfferNotificationEmailAsync(string sellerEmail, string buyerName, string carDetails, decimal offerAmount)
    {
        var subject = "New Offer Received";
        var body = $@"<h2>New Offer Notification</h2><p>You have received a new offer on your listing: {carDetails}</p><p><strong>Buyer:</strong> {buyerName}</p><p><strong>Offered Price:</strong> ${offerAmount:F2}</p>";
        await SendEmailAsync(sellerEmail, subject, body);
    }
}
