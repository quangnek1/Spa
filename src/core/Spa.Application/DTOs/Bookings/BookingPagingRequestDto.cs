using Spa.Application.Seedwork;

namespace Spa.Application.DTOs.Bookings;

public class BookingPagingRequest : PagingRequestParameters
{
	public int? CustomerId { get; set; }
	public int? StaffId { get; set; }
}
