using ClutchBids.Domain.DTOs.Admin;
using ClutchBids.Domain.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ClutchBids.WebAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin,SuperAdmin")]
public class DashboardController : ControllerBase
{
    private readonly IDashboardService _dashboardService;

    public DashboardController(IDashboardService dashboardService)
    {
        _dashboardService = dashboardService;
    }

    [HttpGet("statistics")]
    public async Task<IActionResult> GetDashboardStatistics()
    {
        var statistics = await _dashboardService.GetDashboardStatisticsAsync();
        return Ok(statistics);
    }

    [HttpGet("system-statistics")]
    public async Task<IActionResult> GetSystemStatistics()
    {
        var statistics = await _dashboardService.GetSystemStatisticsAsync();
        return Ok(statistics);
    }
}
