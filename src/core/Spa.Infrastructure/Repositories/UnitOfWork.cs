using Spa.Domain.Entities.Bookings;
using Spa.Domain.Entities.Services;
using Spa.Domain.Repositories;
using Spa.Infrastructure.Data;

namespace Spa.Infrastructure.Repositories;

public class UnitOfWork : IUnitOfWork
{
    private readonly ApplicationDbContext _context;

    public IGenericRepository<Booking> Bookings { get; private set; }
    public IGenericRepository<Service> Services { get; private set; }

    public UnitOfWork(ApplicationDbContext context)
    {
        _context = context;
        Bookings = new GenericRepository<Booking>(_context);
        Services = new GenericRepository<Service>(_context);
    }

    public async Task<int> SaveChangesAsync()
    {
        return await _context.SaveChangesAsync();
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}