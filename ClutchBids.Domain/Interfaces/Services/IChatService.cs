namespace ClutchBids.Domain.Interfaces.Services;

/// <summary>
/// Service for handling chat operations including session creation, messaging, and history retrieval
/// </summary>
public interface IChatService
{
    /// <summary>
    /// Creates a chat session initiated by an admin to a user with an initial message
    /// </summary>
    Task<dynamic?> CreateAdminChatAsync(long adminId, long userId, string message);

    /// <summary>
    /// Creates a chat session when a seller accepts a buyer's offer
    /// </summary>
    Task<dynamic?> CreateOfferAcceptanceChatAsync(long sellerId, long buyerId, long offerId);

    /// <summary>
    /// Creates a chat session when an admin rejects a user verification with a reason
    /// </summary>
    Task<dynamic?> CreateUserRejectionChatAsync(long adminId, long userId, string rejectionReason);

    /// <summary>
    /// Creates a chat session when an admin rejects a car listing verification with a reason
    /// </summary>
    Task<dynamic?> CreateListingRejectionChatAsync(long adminId, long userId, long carListingId, string rejectionReason);

    /// <summary>
    /// Sends a message in an existing chat session
    /// </summary>
    Task<dynamic?> SendMessageAsync(long chatSessionId, long senderId, long receiverId, string message);

    /// <summary>
    /// Retrieves all chat sessions for a user
    /// </summary>
    Task<IEnumerable<dynamic>> GetUserChatsAsync(long userId);

    /// <summary>
    /// Retrieves messages from a specific chat session
    /// </summary>
    Task<dynamic?> GetChatMessagesAsync(long chatSessionId, long userId);

    /// <summary>
    /// Marks a message as read
    /// </summary>
    Task<bool> MarkMessageAsReadAsync(long messageId, long userId);

    /// <summary>
    /// Gets a specific chat session by ID
    /// </summary>
    Task<dynamic?> GetChatSessionAsync(long chatSessionId, long userId);

    /// <summary>
    /// Checks if a chat session exists between two users for a specific context
    /// </summary>
    Task<dynamic?> GetExistingChatAsync(long firstUserId, long secondUserId, long? carListingId = null);
}
