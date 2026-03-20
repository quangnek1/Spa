namespace Spa.Application.DTOs.Admin;

public class DashboardStatsDto
{
    public int TotalBookings { get; set; }
    public int BookingsToday { get; set; }
    public int UpcomingBookings { get; set; }
    public decimal TotalRevenue { get; set; }
    // ... các trường khác
    public List<RecentBookingDto> RecentBookings { get; set; } = new();
}