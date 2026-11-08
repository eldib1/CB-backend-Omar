using ClutchBids.Domain.DTOs.Admin;
using ClutchBids.Domain.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ClutchBids.WebAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "SuperAdmin")]
public class SuperAdminController : ControllerBase
{
    private readonly ISuperAdminService _superAdminService;

    public SuperAdminController(ISuperAdminService superAdminService)
    {
        _superAdminService = superAdminService;
    }

    [HttpPost("create-admin")]
    public async Task<IActionResult> CreateAdmin([FromBody] CreateAdminRequestDto request)
    {
        var result = await _superAdminService.CreateAdminAsync(request);
        if (result == null)
            return BadRequest(new { message = "Failed to create admin. Email may already exist." });

        return CreatedAtAction(nameof(GetAdmin), new { adminId = result.Id }, result);
    }

    [HttpGet("admins")]
    public async Task<IActionResult> GetAllAdmins()
    {
        var admins = await _superAdminService.GetAllAdminsAsync();
        return Ok(admins);
    }

    [HttpGet("admin/{adminId}")]
    public async Task<IActionResult> GetAdmin(long adminId)
    {
        var admins = await _superAdminService.GetAllAdminsAsync();
        var admin = admins.FirstOrDefault(a => a.Id == adminId);
        if (admin == null)
            return NotFound();

        return Ok(admin);
    }

    [HttpDelete("remove-admin/{adminId}")]
    public async Task<IActionResult> RemoveAdmin(long adminId)
    {
        var result = await _superAdminService.RemoveAdminAsync(adminId);
        if (!result)
            return BadRequest(new { message = "Failed to remove admin" });

        return Ok(new { message = "Admin removed successfully" });
    }
}
