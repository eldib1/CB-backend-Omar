using ClutchBids.Domain.DTOs.PriceOffer;
using ClutchBids.Domain.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ClutchBids.WebAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "User,Admin,SuperAdmin")]
public class PriceOffersController : ControllerBase
{
    private readonly IPriceOfferService _priceOfferService;

    public PriceOffersController(IPriceOfferService priceOfferService)
    {
        _priceOfferService = priceOfferService;
    }

    [HttpPost("create")]
    public async Task<IActionResult> CreateOffer([FromBody] CreatePriceOfferRequestDto request)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        var result = await _priceOfferService.CreateOfferAsync(long.Parse(userId), request);
        if (result == null)
            return BadRequest(new { message = "Failed to create offer" });

        return CreatedAtAction(nameof(GetOffer), new { id = result.Id }, result);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetOffer(long id)
    {
        var result = await _priceOfferService.GetOfferAsync(id);
        if (result == null)
            return NotFound(new { message = "Offer not found" });

        return Ok(result);
    }

    [HttpPost("{id}/accept")]
    public async Task<IActionResult> AcceptOffer(long id)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        var result = await _priceOfferService.AcceptOfferAsync(id, long.Parse(userId));
        if (result == null)
            return BadRequest(new { message = "Failed to accept offer" });

        return Ok(new { message = "Offer accepted successfully", data = result });
    }

    [HttpPost("{id}/reject")]
    public async Task<IActionResult> RejectOffer(long id, [FromBody] RejectOfferRequestDto request)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        var result = await _priceOfferService.RejectOfferAsync(id, long.Parse(userId), request.RejectionReason);
        if (result == null)
            return BadRequest(new { message = "Failed to reject offer" });

        return Ok(new { message = "Offer rejected successfully", data = result });
    }

    [HttpPost("{id}/withdraw")]
    public async Task<IActionResult> WithdrawOffer(long id)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        var result = await _priceOfferService.DeleteOfferAsync(id, long.Parse(userId));
        if (!result)
            return BadRequest(new { message = "Failed to withdraw offer" });

        return Ok(new { message = "Offer withdrawn successfully" });
    }
}
