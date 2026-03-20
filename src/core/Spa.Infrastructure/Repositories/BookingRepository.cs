using Microsoft.EntityFrameworkCore;
using Spa.Application.Interfaces.Repositories;
using Spa.Application.Seedwork;
using Spa.Domain.Entities.Bookings;
using Spa.Infrastructure.Data;

namespace Spa.Infrastructure.Repositories
{
	public class BookingRepository : GenericRepository<Booking>, IBookingRepository
	{

		public BookingRepository(ApplicationDbContext context) : base(context)
		{
		}

		public async Task<PagedResult<Booking>> GetPagedBookingsAsync(PagingRequestParameters request)
		{
			var query = dbSet.AsQueryable()
				.Include(b => b.ServicePackage)
				.ThenInclude(p => p.Service);;

			var totalItems = await query.CountAsync();

			var items = await query
				.OrderByDescending(x => x.Id)
				.Skip((request.PageIndex - 1) * request.PageSize)
				.Take(request.PageSize)
				.ToListAsync();

			return new PagedResult<Booking>
			{
				Items = items,
				TotalItems = totalItems,
				PageIndex = request.PageIndex,
				PageSize = request.PageSize
			};
		}
	}
}
