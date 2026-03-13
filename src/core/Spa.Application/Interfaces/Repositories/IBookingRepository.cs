using Spa.Application.Seedwork;
using Spa.Domain.Entities.Bookings;

namespace Spa.Application.Interfaces.Repositories;

public interface IBookingRepository : IGenericRepository<Booking>
{
	Task<PagedResult<Booking>> GetPagedBookingsAsync(PagingRequestParameters request);
}
