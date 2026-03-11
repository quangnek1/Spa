using Spa.Domain.Enums;

namespace Spa.Application.DTOs.Bookings;

public class BookingResponseDto
{
	public int Id { get; set; }
	public string CustomerName { get; set; } = default!;
	public string ServiceName { get; set; } = default!;
	public int DurationMinutes { get; set; }
	public DateTime ScheduledStartTime { get; set; }
	public DateTime ScheduledEndTime { get; set; }
	public decimal TotalPrice { get; set; }
	public decimal DiscountAmount { get; set; }
	public decimal FinalAmount { get; set; }
	public BookingStatus Status { get; set; }
}
