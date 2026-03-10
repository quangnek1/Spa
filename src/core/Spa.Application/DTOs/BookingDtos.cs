using Spa.Domain.Enums;

namespace Spa.Application.DTOs
{
	public class BookingDtos
	{
	}
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
}
