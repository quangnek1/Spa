namespace Spa.Application.DTOs.Admin;

public class RecentBookingDto
{
    public int Id { get; set; }
    public string CustomerName { get; set; } = default!;
    public string ServiceName { get; set; } = default!;
    public DateTime ScheduledStartTime { get; set; }
    public string Status { get; set; } = default!;
}