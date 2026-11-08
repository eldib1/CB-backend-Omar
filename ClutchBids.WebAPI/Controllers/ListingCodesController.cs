using ClutchBids.Domain.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using ClutchBids.Domain.DTOs.ListingCode;

namespace ClutchBids.WebAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin,SuperAdmin")]
public class ListingCodesController : ControllerBase
{
    private readonly IListingCodeService _listingCodeService;

    public ListingCodesController(IListingCodeService listingCodeService)
    {
        _listingCodeService = listingCodeService;
    }

    [HttpPost("generate")]
    public async Task<IActionResult> GenerateCodes([FromBody] GenerateListingCodesRequestDto request)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        var result = await _listingCodeService.GenerateCodesAsync(long.Parse(userId), request.Count);
        if (!result)
            return BadRequest(new { message = "Failed to generate listing codes" });

        return Ok(new { message = "Listing codes generated successfully", count = request.Count });
    }

    [HttpGet("unused")]
    public async Task<IActionResult> GetUnusedCodes()
    {
        var codes = await _listingCodeService.GetUnusedCodesAsync();
        return Ok(codes);
    }

    [HttpGet("all")]
    public async Task<IActionResult> GetAllCodes()
    {
        var codes = await _listingCodeService.GetAllCodesAsync();
        return Ok(codes);
    }

    [HttpPost("validate/{code}")]
    public async Task<IActionResult> ValidateAndUseCode(string code, [FromBody] ValidateListingCodeRequestDto request)
    {
        if (request?.ListingId == null || request.ListingId == 0)
            return BadRequest(new { message = "Invalid listing ID" });

        var result = await _listingCodeService.ValidateAndUseCodeAsync(code, request.ListingId);
        if (!result)
            return BadRequest(new { message = "Invalid or already used code" });

        return Ok(new { message = "Code validated and used successfully" });
    }
}
