using ClutchBids.Domain.DTOs.CarListing;
using ClutchBids.Domain.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ClutchBids.WebAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CarListingsController : ControllerBase
{
    private readonly ICarListingService _carListingService;

    public CarListingsController(ICarListingService carListingService)
    {
        _carListingService = carListingService;
    }

    [HttpGet("search")]
    [AllowAnonymous]
    public async Task<IActionResult> SearchListings([FromQuery] CarListingSearchDto searchDto)
    {
        var result = await _carListingService.SearchListingsAsync(searchDto);
        return Ok(result);
    }

    [HttpGet("featured")]
    [AllowAnonymous]
    public async Task<IActionResult> GetFeaturedListing()
    {
        var result = await _carListingService.GetFeaturedListingAsync();
        if (result == null)
            return NotFound(new { message = "No featured listing" });

        return Ok(result);
    }

    [HttpPost("{id}/feature")]
    [Authorize(Roles = "Admin,SuperAdmin")]
    public async Task<IActionResult> FeatureListing(long id)
    {
        var result = await _carListingService.SetFeaturedAsync(id, true);
        if (!result)
            return NotFound(new { message = "Listing not found" });

        return Ok(new { message = "Listing featured successfully" });
    }

    [HttpPost("{id}/unfeature")]
    [Authorize(Roles = "Admin,SuperAdmin")]
    public async Task<IActionResult> UnfeatureListing(long id)
    {
        var result = await _carListingService.SetFeaturedAsync(id, false);
        if (!result)
            return NotFound(new { message = "Listing not found" });

        return Ok(new { message = "Listing unfeatured successfully" });
    }

    [HttpGet("{id}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetListingDetails(long id)
    {
        var isAuthenticated = User.Identity?.IsAuthenticated ?? false;
        var result = await _carListingService.GetListingDetailsAsync(id, isAuthenticated);
        if (result == null)
            return NotFound(new { message = "Listing not found" });

        return Ok(result);
    }

    [HttpPost("create")]
    [Authorize(Roles = "User,Admin,SuperAdmin")]
    public async Task<IActionResult> CreateListing([FromBody] CreateCarListingRequestDto request)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        var result = await _carListingService.CreateListingAsync(long.Parse(userId), request);
        if (result == null)
            return BadRequest(new { message = "Failed to create listing" });

        return CreatedAtAction(nameof(GetListingDetails), new { id = result.Id }, result);
    }

    [HttpPost("{id}/submit-for-approval")]
    [Authorize(Roles = "User,Admin,SuperAdmin")]
    public async Task<IActionResult> SubmitForApproval(long id)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        var result = await _carListingService.SubmitListingForApprovalAsync(id, long.Parse(userId));
        if (!result)
            return BadRequest(new { message = "Failed to submit listing for approval" });

        return Ok(new { message = "Listing submitted for approval" });
    }

    [HttpPost("{id}/approve")]
    [Authorize(Roles = "Admin,SuperAdmin")]
    public async Task<IActionResult> ApproveListing(long id)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        var result = await _carListingService.ApproveListingAsync(id, long.Parse(userId));
        if (!result)
            return BadRequest(new { message = "Failed to approve listing" });

        return Ok(new { message = "Listing approved successfully" });
    }

    [HttpPost("{id}/reject")]
    [Authorize(Roles = "Admin,SuperAdmin")]
    public async Task<IActionResult> RejectListing(long id, [FromBody] RejectListingRequestDto request)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        var result = await _carListingService.RejectListingAsync(id, long.Parse(userId), request.Reason);
        if (!result)
            return BadRequest(new { message = "Failed to reject listing" });

        return Ok(new { message = "Listing rejected successfully. A chat session has been started with the user." });
    }

    [HttpPost("{id}/deactivate")]
    [Authorize(Roles = "User,Admin,SuperAdmin")]
    public async Task<IActionResult> DeactivateListing(long id)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        var result = await _carListingService.DeactivateListingAsync(id, long.Parse(userId));
        if (!result)
            return BadRequest(new { message = "Failed to deactivate listing" });

        return Ok(new { message = "Listing deactivated successfully" });
    }

    [HttpPost("{id}/mark-as-sold")]
    [Authorize(Roles = "User,Admin,SuperAdmin")]
    public async Task<IActionResult> MarkAsSold(long id)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        var result = await _carListingService.MarkListingAsSoldAsync(id, long.Parse(userId));
        if (!result)
            return BadRequest(new { message = "Failed to mark listing as sold" });

        return Ok(new { message = "Listing marked as sold successfully" });
    }

    [HttpPost("{id}/upload-sale-proof")]
    [Authorize(Roles = "User,Admin,SuperAdmin")]
    public async Task<IActionResult> UploadSaleProof(long id, IFormFile file)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (file == null || file.Length == 0)
            return BadRequest(new { message = "No file provided" });

        using var stream = file.OpenReadStream();
        var result = await _carListingService.UploadSaleProofAsync(id, long.Parse(userId), stream, file.FileName);

        if (!result)
            return BadRequest(new { message = "Failed to upload sale proof" });

        return Ok(new { message = "Sale proof uploaded successfully" });
    }
}
