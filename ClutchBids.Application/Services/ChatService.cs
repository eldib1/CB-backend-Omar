using AutoMapper;
using ClutchBids.Domain.DTOs.Chat;
using ClutchBids.Domain.Entities;
using ClutchBids.Domain.Enums;
using ClutchBids.Domain.Interfaces.Managers;
using ClutchBids.Domain.Interfaces.Repositories;
using ClutchBids.Domain.Interfaces.Services;

namespace ClutchBids.Application.Services;

/// <summary>
/// Implementation of chat service for managing chat sessions and messaging
/// </summary>
public class ChatService : IChatService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IIdentityService _identityService;
    private readonly INotificationManager _notificationService;
    private readonly IMapper _mapper;

    public ChatService(
        IUnitOfWork unitOfWork,
        IIdentityService identityService,
        INotificationManager notificationService,
        IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _identityService = identityService;
        _notificationService = notificationService;
        _mapper = mapper;
    }

    public async Task<dynamic?> CreateAdminChatAsync(long adminId, long userId, string message)
    {
        // Verify admin exists and has admin role
        var admin = await _identityService.FindUserByIdAsync(adminId);
        if (admin == null)
            return null;

        // Verify user exists
        var user = await _identityService.FindUserByIdAsync(userId);
        if (user == null)
            return null;

        // Check if chat session already exists
        var existingChat = await GetExistingChatAsync(adminId, userId);
        if (existingChat != null)
            return existingChat;

        // Create new chat session
        var chatSession = new ChatSession
        {
            FirstUserId = adminId,
            SecondUserId = userId,
            LastMessageAt = DateTime.UtcNow
        };

        await _unitOfWork.ChatSessions.AddAsync(chatSession);
        await _unitOfWork.SaveChangesAsync();

        // Send initial message
        var chatMessage = new ChatMessage
        {
            ChatSessionId = chatSession.Id,
            SenderId = adminId,
            ReceiverId = userId,
            Message = message,
            IsRead = false
        };

        await _unitOfWork.ChatMessages.AddAsync(chatMessage);
        chatSession.LastMessageAt = DateTime.UtcNow;
        await _unitOfWork.ChatSessions.UpdateAsync(chatSession);
        await _unitOfWork.SaveChangesAsync();

        // Send notification
        await _notificationService.SendNewChatNotificationAsync(userId, adminId, "Admin initiated a chat");

        return _mapper.Map<ChatSessionDto>(chatSession);
    }

    public async Task<dynamic?> CreateOfferAcceptanceChatAsync(long sellerId, long buyerId, long offerId)
    {
        // Verify users exist
        var seller = await _identityService.FindUserByIdAsync(sellerId);
        var buyer = await _identityService.FindUserByIdAsync(buyerId);
        if (seller == null || buyer == null)
            return null;

        // Verify offer exists and is accepted
        var offer = await _unitOfWork.PriceOffers.GetByIdAsync(offerId);
        if (offer == null || offer.Status != PriceOfferStatus.Accepted)
            return null;

        // Check if chat session already exists for this offer
        var existingChat = await GetExistingChatAsync(sellerId, buyerId, offer.CarListingId);
        if (existingChat != null)
            return existingChat;

        // Create new chat session
        var chatSession = new ChatSession
        {
            FirstUserId = sellerId,
            SecondUserId = buyerId,
            CarListingId = offer.CarListingId,
            LastMessageAt = DateTime.UtcNow
        };

        await _unitOfWork.ChatSessions.AddAsync(chatSession);
        await _unitOfWork.SaveChangesAsync();

        // Send notification to both users
        await _notificationService.SendNewChatNotificationAsync(buyerId, sellerId, "Your offer has been accepted. You can now discuss details.");
        await _notificationService.SendNewChatNotificationAsync(sellerId, buyerId, "You accepted an offer. You can now discuss with the buyer.");

        return _mapper.Map<ChatSessionDto>(chatSession);
    }

    public async Task<dynamic?> CreateUserRejectionChatAsync(long adminId, long userId, string rejectionReason)
    {
        // Verify admin exists
        var admin = await _identityService.FindUserByIdAsync(adminId);
        if (admin == null)
            return null;

        // Verify user exists
        var user = await _identityService.FindUserByIdAsync(userId);
        if (user == null)
            return null;

        // Check if chat session already exists
        var existingChat = await GetExistingChatAsync(adminId, userId);
        if (existingChat != null)
            return existingChat;

        // Create new chat session
        var chatSession = new ChatSession
        {
            FirstUserId = adminId,
            SecondUserId = userId,
            LastMessageAt = DateTime.UtcNow
        };

        await _unitOfWork.ChatSessions.AddAsync(chatSession);
        await _unitOfWork.SaveChangesAsync();

        // Send initial message with rejection reason
        var messageContent = $"Your user verification has been rejected. Reason: {rejectionReason}";
        var chatMessage = new ChatMessage
        {
            ChatSessionId = chatSession.Id,
            SenderId = adminId,
            ReceiverId = userId,
            Message = messageContent,
            IsRead = false
        };

        await _unitOfWork.ChatMessages.AddAsync(chatMessage);
        chatSession.LastMessageAt = DateTime.UtcNow;
        await _unitOfWork.ChatSessions.UpdateAsync(chatSession);
        await _unitOfWork.SaveChangesAsync();

        // Send notification
        await _notificationService.SendNewChatNotificationAsync(userId, adminId, "Your account verification was rejected");

        return _mapper.Map<ChatSessionDto>(chatSession);
    }

    public async Task<dynamic?> CreateListingRejectionChatAsync(long adminId, long userId, long carListingId, string rejectionReason)
    {
        // Verify admin exists
        var admin = await _identityService.FindUserByIdAsync(adminId);
        if (admin == null)
            return null;

        // Verify user exists
        var user = await _identityService.FindUserByIdAsync(userId);
        if (user == null)
            return null;

        // Verify listing exists
        var listing = await _unitOfWork.CarListings.GetByIdAsync(carListingId);
        if (listing == null)
            return null;

        // Check if chat session already exists for this listing
        var existingChat = await GetExistingChatAsync(adminId, userId, carListingId);
        if (existingChat != null)
            return existingChat;

        // Create new chat session
        var chatSession = new ChatSession
        {
            FirstUserId = adminId,
            SecondUserId = userId,
            CarListingId = carListingId,
            LastMessageAt = DateTime.UtcNow
        };

        await _unitOfWork.ChatSessions.AddAsync(chatSession);
        await _unitOfWork.SaveChangesAsync();

        // Send initial message with rejection reason
        var messageContent = $"Your car listing '{listing.ListingTitle}' has been rejected. Reason: {rejectionReason}";
        var chatMessage = new ChatMessage
        {
            ChatSessionId = chatSession.Id,
            SenderId = adminId,
            ReceiverId = userId,
            Message = messageContent,
            IsRead = false
        };

        await _unitOfWork.ChatMessages.AddAsync(chatMessage);
        chatSession.LastMessageAt = DateTime.UtcNow;
        await _unitOfWork.ChatSessions.UpdateAsync(chatSession);
        await _unitOfWork.SaveChangesAsync();

        // Send notification
        await _notificationService.SendNewChatNotificationAsync(userId, adminId, $"Your car listing was rejected");

        return _mapper.Map<ChatSessionDto>(chatSession);
    }

    public async Task<dynamic?> SendMessageAsync(long chatSessionId, long senderId, long receiverId, string message)
    {
        // Verify chat session exists
        var chatSession = await _unitOfWork.ChatSessions.GetByIdAsync(chatSessionId);
        if (chatSession == null)
            return null;

        // Verify sender is part of the chat session
        if (chatSession.FirstUserId != senderId && chatSession.SecondUserId != senderId)
            return null;

        // Verify receiver is part of the chat session
        if (chatSession.FirstUserId != receiverId && chatSession.SecondUserId != receiverId)
            return null;

        // Verify sender and receiver exist
        var sender = await _identityService.FindUserByIdAsync(senderId);
        var receiver = await _identityService.FindUserByIdAsync(receiverId);
        if (sender == null || receiver == null)
            return null;

        // Create message
        var chatMessage = new ChatMessage
        {
            ChatSessionId = chatSessionId,
            SenderId = senderId,
            ReceiverId = receiverId,
            Message = message,
            IsRead = false
        };

        await _unitOfWork.ChatMessages.AddAsync(chatMessage);

        // Update last message time
        chatSession.LastMessageAt = DateTime.UtcNow;
        await _unitOfWork.ChatSessions.UpdateAsync(chatSession);
        await _unitOfWork.SaveChangesAsync();

        // Send notification
        await _notificationService.SendNewChatMessageNotificationAsync(receiverId, senderId, chatSessionId);

        return _mapper.Map<ChatMessageDto>(chatMessage);
    }

    public async Task<IEnumerable<dynamic>> GetUserChatsAsync(long userId)
    {
        // Verify user exists
        var user = await _identityService.FindUserByIdAsync(userId);
        if (user == null)
            return [];

        // Get all chat sessions for the user
        var allSessions = await _unitOfWork.ChatSessions.GetAllAsync();
        var userSessions = allSessions
            .Where(s => s.FirstUserId == userId || s.SecondUserId == userId)
            .OrderByDescending(s => s.LastMessageAt)
            .ToList();

        var result = new List<ChatSessionDto>();
        foreach (var session in userSessions)
        {
            var otherUserId = session.FirstUserId == userId ? session.SecondUserId : session.FirstUserId;
            var otherUser = await _identityService.FindUserByIdAsync(otherUserId);
            
            result.Add(new ChatSessionDto
            {
                Id = session.Id,
                FirstUserId = session.FirstUserId,
                FirstUserName = session.FirstUser?.FirstName ?? "",
                SecondUserId = session.SecondUserId,
                SecondUserName = session.SecondUser?.FirstName ?? "",
                CarListingId = session.CarListingId,
                CarTitle = session.CarListing?.ListingTitle ?? "",
                LastMessageAt = session.LastMessageAt,
                Messages = []
            });
        }

        return result;
    }

    public async Task<dynamic?> GetChatMessagesAsync(long chatSessionId, long userId)
    {
        // Verify chat session exists
        var chatSession = await _unitOfWork.ChatSessions.GetByIdAsync(chatSessionId);
        if (chatSession == null)
            return null;

        // Verify user is part of the chat session
        if (chatSession.FirstUserId != userId && chatSession.SecondUserId != userId)
            return null;

        // Get messages
        var allMessages = await _unitOfWork.ChatMessages.GetAllAsync();
        var messages = allMessages
            .Where(m => m.ChatSessionId == chatSessionId)
            .OrderBy(m => m.CreatedAt)
            .ToList();

        // Map to DTOs
        var messageDtos = new List<ChatMessageDto>();
        foreach (var msg in messages)
        {
            var sender = await _identityService.FindUserByIdAsync(msg.SenderId);
            messageDtos.Add(new ChatMessageDto
            {
                Id = msg.Id,
                ChatSessionId = msg.ChatSessionId,
                SenderId = msg.SenderId,
                SenderName = sender?.FirstName ?? "",
                ReceiverId = msg.ReceiverId,
                Message = msg.Message,
                IsRead = msg.IsRead,
                CreatedAt = msg.CreatedAt
            });
        }

        var firstUser = await _identityService.FindUserByIdAsync(chatSession.FirstUserId);
        var secondUser = await _identityService.FindUserByIdAsync(chatSession.SecondUserId);

        return new ChatSessionDto
        {
            Id = chatSession.Id,
            FirstUserId = chatSession.FirstUserId,
            FirstUserName = firstUser?.FirstName ?? "",
            SecondUserId = chatSession.SecondUserId,
            SecondUserName = secondUser?.FirstName ?? "",
            CarListingId = chatSession.CarListingId,
            CarTitle = chatSession.CarListing?.ListingTitle ?? "",
            LastMessageAt = chatSession.LastMessageAt,
            Messages = messageDtos
        };
    }

    public async Task<bool> MarkMessageAsReadAsync(long messageId, long userId)
    {
        // Verify message exists
        var message = await _unitOfWork.ChatMessages.GetByIdAsync(messageId);
        if (message == null)
            return false;

        // Verify user is the receiver
        if (message.ReceiverId != userId)
            return false;

        message.IsRead = true;
        message.ReadAt = DateTime.UtcNow;

        await _unitOfWork.ChatMessages.UpdateAsync(message);
        await _unitOfWork.SaveChangesAsync();

        return true;
    }

    public async Task<dynamic?> GetChatSessionAsync(long chatSessionId, long userId)
    {
        // Verify chat session exists
        var chatSession = await _unitOfWork.ChatSessions.GetByIdAsync(chatSessionId);
        if (chatSession == null)
            return null;

        // Verify user is part of the chat session
        if (chatSession.FirstUserId != userId && chatSession.SecondUserId != userId)
            return null;

        var firstUser = await _identityService.FindUserByIdAsync(chatSession.FirstUserId);
        var secondUser = await _identityService.FindUserByIdAsync(chatSession.SecondUserId);

        return new ChatSessionDto
        {
            Id = chatSession.Id,
            FirstUserId = chatSession.FirstUserId,
            FirstUserName = firstUser?.FirstName ?? "",
            SecondUserId = chatSession.SecondUserId,
            SecondUserName = secondUser?.FirstName ?? "",
            CarListingId = chatSession.CarListingId,
            CarTitle = chatSession.CarListing?.ListingTitle ?? "",
            LastMessageAt = chatSession.LastMessageAt,
            Messages = []
        };
    }

    public async Task<dynamic?> GetExistingChatAsync(long firstUserId, long secondUserId, long? carListingId = null)
    {
        var allSessions = await _unitOfWork.ChatSessions.GetAllAsync();
        
        ChatSession? existingSession = null;
        
        if (carListingId.HasValue)
        {
            // Look for session with specific context
            existingSession = allSessions.FirstOrDefault(s =>
                ((s.FirstUserId == firstUserId && s.SecondUserId == secondUserId) ||
                 (s.FirstUserId == secondUserId && s.SecondUserId == firstUserId)) &&
                s.CarListingId == carListingId);
        }
        else
        {
            // Look for session between these two users
            existingSession = allSessions.FirstOrDefault(s =>
                (s.FirstUserId == firstUserId && s.SecondUserId == secondUserId) ||
                (s.FirstUserId == secondUserId && s.SecondUserId == firstUserId));
        }

        if (existingSession == null)
            return null;

        var firstUser = await _identityService.FindUserByIdAsync(existingSession.FirstUserId);
        var secondUser = await _identityService.FindUserByIdAsync(existingSession.SecondUserId);

        return _mapper.Map<ChatSessionDto>(existingSession);
    }

}
