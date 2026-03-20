using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Spa.Application.Interfaces;

namespace Spa.WebAPI.Controllers;

public class AdminController : BaseController
{
    private readonly IDashboardService _dashboardService;

    public AdminController(IDashboardService dashboardService)
    {
        _dashboardService = dashboardService ?? throw new ArgumentNullException(nameof(dashboardService));
    }
    [HttpGet("dashboard-stats")]
    [Authorize(Roles = "Administrator")]
    public async Task<IActionResult> GetDashboardStats()
    {
        try
        {
            var stats = await _dashboardService.GetAdminDashboardStatsAsync();
            return Ok(stats);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}