using ClutchBids.Domain.DTOs.Admin;
using ClutchBids.Domain.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ClutchBids.WebAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin,SuperAdmin")]
public class AdminController : ControllerBase
{
    private readonly IAdminService _adminService;

    public AdminController(IAdminService adminService)
    {
        _adminService = adminService;
    }

    [HttpGet("pending-users")]
    public async Task<IActionResult> GetPendingUsers()
    {
        var users = await _adminService.GetPendingVerificationUsersAsync();
        return Ok(users);
    }

    [HttpGet("all-users")]
    public async Task<IActionResult> GetAllUsers()
    {
        var users = await _adminService.GetAllUsersAsync();
        return Ok(users);
    }

    [HttpGet("user/{userId}")]
    public async Task<IActionResult> GetUser(long userId)
    {
        var user = await _adminService.GetUserAsync(userId);
        if (user == null)
            return NotFound();

        return Ok(user);
    }

    [HttpPost("approve-user")]
    public async Task<IActionResult> ApproveUser([FromBody] ApproveUserRequestDto request)
    {
        var approverEmail = User.FindFirst(ClaimTypes.Email)?.Value ?? "Unknown";
        var result = await _adminService.ApproveUserAsync(request.UserId, approverEmail);
        if (result == null)
            return BadRequest(new { message = "Failed to approve user" });

        return Ok(result);
    }

    [HttpPost("reject-user")]
    public async Task<IActionResult> RejectUser([FromBody] RejectUserRequestDto request)
    {
        var rejectorEmail = User.FindFirst(ClaimTypes.Email)?.Value ?? "Unknown";
        var result = await _adminService.RejectUserAsync(request.UserId, request.RejectionReason, rejectorEmail);
        if (result == null)
            return BadRequest(new { message = "Failed to reject user" });

        return Ok(result);
    }

    [HttpPost("suspend-user/{userId}")]
    public async Task<IActionResult> SuspendUser(long userId)
    {
        var result = await _adminService.SuspendUserAsync(userId);
        if (!result)
            return BadRequest(new { message = "Failed to suspend user" });

        return Ok(new { message = "User suspended successfully" });
    }

    [HttpPost("assign-listing-codes")]
    public async Task<IActionResult> AssignListingCodes([FromBody] AssignListingCodeRequestDto request)
    {
        var result = await _adminService.AssignListingCodesAsync(request.UserId, request.Count);
        if (!result)
            return BadRequest(new { message = "Failed to assign listing codes" });

        return Ok(new { message = "Listing codes assigned successfully" });
    }

    [HttpPost("upload-id-document")]
    public async Task<IActionResult> UploadIdDocument(long userId, IFormFile document, [FromForm] string identityNumber, [FromForm] DateTime identityExpiryDate)
    {
        if (document == null || document.Length == 0)
            return BadRequest(new { message = "No document provided" });

        using var stream = document.OpenReadStream();
        var result = await _adminService.UploadIdDocumentAsync(userId, stream, document.FileName, identityNumber, identityExpiryDate);

        if (!result.Success)
            return BadRequest(new { message = result.Message });

        return Ok(result);
    }

    [HttpGet("user-verification/{userId}")]
    public async Task<IActionResult> GetUserForVerification(long userId)
    {
        var user = await _adminService.GetUserForVerificationAsync(userId);
        if (user == null)
            return NotFound();

        return Ok(user);
    }

    [HttpGet("pending-verifications")]
    public async Task<IActionResult> GetPendingVerifications()
    {
        var users = await _adminService.GetPendingVerificationsAsync();
        return Ok(users);
    }
}
