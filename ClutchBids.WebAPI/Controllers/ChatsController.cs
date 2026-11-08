using ClutchBids.Domain.DTOs.Chat;
using ClutchBids.Domain.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ClutchBids.WebAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ChatsController : ControllerBase
{
    private readonly IChatService _chatService;

    public ChatsController(IChatService chatService)
    {
        _chatService = chatService;
    }

    /// <summary>
    /// Admin creates a chat session with a user and sends an initial message
    /// </summary>
    [HttpPost("admin-chat")]
    [Authorize(Roles = "Admin,SuperAdmin")]
    public async Task<IActionResult> CreateAdminChat([FromBody] CreateAdminChatRequestDto request)
    {
        var adminId = long.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
        
        var result = await _chatService.CreateAdminChatAsync(adminId, request.UserId, request.Message);
        if (result == null)
            return BadRequest(new { message = "Failed to create chat session" });

        return Ok(result);
    }

    /// <summary>
    /// Creates a chat session when a seller accepts a buyer's offer
    /// </summary>
    [HttpPost("offer-acceptance-chat")]
    public async Task<IActionResult> CreateOfferAcceptanceChat([FromQuery] long buyerId, [FromQuery] long offerId)
    {
        var sellerId = long.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
        
        var result = await _chatService.CreateOfferAcceptanceChatAsync(sellerId, buyerId, offerId);
        if (result == null)
            return BadRequest(new { message = "Failed to create chat session" });

        return Ok(result);
    }

    /// <summary>
    /// Admin creates a chat session with rejection reason for user verification rejection
    /// </summary>
    [HttpPost("user-rejection-chat")]
    [Authorize(Roles = "Admin,SuperAdmin")]
    public async Task<IActionResult> CreateUserRejectionChat([FromQuery] long userId, [FromQuery] string rejectionReason)
    {
        var adminId = long.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
        
        var result = await _chatService.CreateUserRejectionChatAsync(adminId, userId, rejectionReason);
        if (result == null)
            return BadRequest(new { message = "Failed to create chat session" });

        return Ok(result);
    }

    /// <summary>
    /// Admin creates a chat session with rejection reason for car listing rejection
    /// </summary>
    [HttpPost("listing-rejection-chat")]
    [Authorize(Roles = "Admin,SuperAdmin")]
    public async Task<IActionResult> CreateListingRejectionChat([FromQuery] long userId, [FromQuery] long carListingId, [FromQuery] string rejectionReason)
    {
        var adminId = long.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
        
        var result = await _chatService.CreateListingRejectionChatAsync(adminId, userId, carListingId, rejectionReason);
        if (result == null)
            return BadRequest(new { message = "Failed to create chat session" });

        return Ok(result);
    }

    /// <summary>
    /// Sends a message in an existing chat session
    /// </summary>
    [HttpPost("send-message")]
    public async Task<IActionResult> SendMessage([FromBody] SendMessageRequestDto request)
    {
        var senderId = long.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
        
        var result = await _chatService.SendMessageAsync(request.ChatSessionId, senderId, request.ReceiverId, request.Message);
        if (result == null)
            return BadRequest(new { message = "Failed to send message" });

        return Ok(result);
    }

    /// <summary>
    /// Gets all chat sessions for the current user
    /// </summary>
    [HttpGet("my-chats")]
    public async Task<IActionResult> GetMyChats()
    {
        var userId = long.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
        
        var chats = await _chatService.GetUserChatsAsync(userId);
        return Ok(chats);
    }

    /// <summary>
    /// Gets messages from a specific chat session
    /// </summary>
    [HttpGet("{chatSessionId}/messages")]
    public async Task<IActionResult> GetChatMessages(long chatSessionId)
    {
        var userId = long.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
        
        var chat = await _chatService.GetChatMessagesAsync(chatSessionId, userId);
        if (chat == null)
            return NotFound(new { message = "Chat session not found or you don't have access" });

        return Ok(chat);
    }

    /// <summary>
    /// Gets a specific chat session
    /// </summary>
    [HttpGet("{chatSessionId}")]
    public async Task<IActionResult> GetChatSession(long chatSessionId)
    {
        var userId = long.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
        
        var chat = await _chatService.GetChatSessionAsync(chatSessionId, userId);
        if (chat == null)
            return NotFound(new { message = "Chat session not found or you don't have access" });

        return Ok(chat);
    }

    /// <summary>
    /// Marks a message as read
    /// </summary>
    [HttpPut("message/{messageId}/read")]
    public async Task<IActionResult> MarkMessageAsRead(long messageId)
    {
        var userId = long.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
        
        var result = await _chatService.MarkMessageAsReadAsync(messageId, userId);
        if (!result)
            return BadRequest(new { message = "Failed to mark message as read" });

        return Ok(new { message = "Message marked as read" });
    }
}
