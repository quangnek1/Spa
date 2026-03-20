using Spa.Application.DTOs.Admin;

namespace Spa.Application.Interfaces;

public interface IDashboardService
{
    Task<DashboardStatsDto> GetAdminDashboardStatsAsync();
}       