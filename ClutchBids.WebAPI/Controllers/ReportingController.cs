using ClutchBids.Domain.DTOs.Reporting;
using ClutchBids.Domain.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ClutchBids.WebAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ReportingController : ControllerBase
{
    private readonly IUserReportingService _reportingService;

    public ReportingController(IUserReportingService reportingService)
    {
        _reportingService = reportingService;
    }

    /// <summary>
    /// Report a user for inappropriate behavior
    /// </summary>
    [HttpPost("report-user")]
    public async Task<IActionResult> ReportUser([FromBody] CreateUserReportRequestDto request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var reporterId = long.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
        
        var result = await _reportingService.ReportUserAsync(reporterId, request);
        
        if (result == null)
            return BadRequest(new { message = "Failed to create report. Please check the reported user ID." });

        return Ok(new { message = "Report submitted successfully", data = result });
    }

    /// <summary>
    /// Get all pending reports (Admin only)
    /// </summary>
    [HttpGet("pending")]
    [Authorize(Roles = "Admin,SuperAdmin")]
    public async Task<IActionResult> GetPendingReports()
    {
        var reports = await _reportingService.GetPendingReportsAsync();
        return Ok(new { data = reports });
    }

    /// <summary>
    /// Get reports for a specific user (Admin only)
    /// </summary>
    [HttpGet("user/{userId}")]
    [Authorize(Roles = "Admin,SuperAdmin")]
    public async Task<IActionResult> GetUserReports(long userId)
    {
        var reports = await _reportingService.GetUserReportsAsync(userId);
        return Ok(new { data = reports });
    }

    /// <summary>
    /// Get reports by status (Admin only)
    /// </summary>
    [HttpGet("status/{status}")]
    [Authorize(Roles = "Admin,SuperAdmin")]
    public async Task<IActionResult> GetReportsByStatus(int status)
    {
        var reports = await _reportingService.GetReportsByStatusAsync(status);
        return Ok(new { data = reports });
    }

    /// <summary>
    /// Review and action a report (Admin only)
    /// </summary>
    [HttpPost("review")]
    [Authorize(Roles = "Admin,SuperAdmin")]
    public async Task<IActionResult> ReviewReport([FromBody] ReviewUserReportRequestDto request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var adminId = long.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
        
        var result = await _reportingService.ReviewReportAsync(adminId, request);
        
        if (!result)
            return BadRequest(new { message = "Failed to review report." });

        return Ok(new { message = "Report reviewed and actioned successfully" });
    }
}
