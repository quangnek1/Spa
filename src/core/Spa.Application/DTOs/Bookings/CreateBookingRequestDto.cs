namespace Spa.Application.DTOs.Bookings;

public class CreateBookingRequestDto
{
	public Guid? UserId { get; set; } // Null nếu là khách vãng lai
	public string? CustomerName { get; set; }
	public string? CustomerPhone { get; set; }
	public string? CustomerEmail { get; set; }

	public int ServicePackageId { get; set; }
	public int? StaffId { get; set; }
	public string? CouponCode { get; set; }

	public DateTime ScheduledStartTime { get; set; }
	public string? Notes { get; set; }
}

