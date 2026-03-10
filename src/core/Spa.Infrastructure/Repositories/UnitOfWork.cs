using Microsoft.EntityFrameworkCore.Storage;
using Spa.Domain.Entities.Bookings;
using Spa.Domain.Entities.Customers;
using Spa.Domain.Entities.Identity;
using Spa.Domain.Entities.Payments;
using Spa.Domain.Entities.Post;
using Spa.Domain.Entities.Reviews;
using Spa.Domain.Entities.Services;
using Spa.Domain.Entities.SystemConfigs;
using Spa.Domain.Entities.VoucherCoupon;
using Spa.Domain.Repositories;
using Spa.Infrastructure.Data;

namespace Spa.Infrastructure.Repositories;

public class UnitOfWork : IUnitOfWork
{
    private readonly ApplicationDbContext _context;
    private IDbContextTransaction? _transaction;

    public UnitOfWork(ApplicationDbContext context)
    {
        _context = context;
    }

    // Khởi tạo lười (Lazy Loading) bằng properties để tối ưu bộ nhớ
    public IGenericRepository<Category> Categories => new GenericRepository<Category>(_context);
    public IGenericRepository<Service> Services => new GenericRepository<Service>(_context);
    public IGenericRepository<ServicePackage> ServicePackages => new GenericRepository<ServicePackage>(_context);
    public IGenericRepository<ServiceImage> ServiceImages => new GenericRepository<ServiceImage>(_context);
    public IGenericRepository<Review> Reviews => new GenericRepository<Review>(_context);

    public IGenericRepository<Booking> Bookings => new GenericRepository<Booking>(_context);
    public IGenericRepository<BookingSetting> BookingSettings => new GenericRepository<BookingSetting>(_context);
    public IGenericRepository<Payment> Payments => new GenericRepository<Payment>(_context);
    public IGenericRepository<Coupon> Coupons => new GenericRepository<Coupon>(_context);

    public IGenericRepository<PostCategory> PostCategories => new GenericRepository<PostCategory>(_context);
    public IGenericRepository<Post> Posts => new GenericRepository<Post>(_context);
    public IGenericRepository<Page> Pages => new GenericRepository<Page>(_context);
    public IGenericRepository<Contact> Contacts => new GenericRepository<Contact>(_context);
    public IGenericRepository<SystemSetting> SystemSettings => new GenericRepository<SystemSetting>(_context);
    public IGenericRepository<Menu> Menus => new GenericRepository<Menu>(_context);
    public IGenericRepository<Banner> Banners => new GenericRepository<Banner>(_context);
    public IGenericRepository<FAQ> FAQs => new GenericRepository<FAQ>(_context);

    public IGenericRepository<Staff> Staffs => new GenericRepository<Staff>(_context);

    public async Task<int> SaveChangesAsync()
    {
        return await _context.SaveChangesAsync();
    }

    public async Task BeginTransactionAsync()
    {
        _transaction = await _context.Database.BeginTransactionAsync();
    }

    public async Task CommitTransactionAsync()
    {
        if (_transaction != null)
        {
            await _transaction.CommitAsync();
            await _transaction.DisposeAsync();
        }
    }

    public async Task RollbackTransactionAsync()
    {
        if (_transaction != null)
        {
            await _transaction.RollbackAsync();
            await _transaction.DisposeAsync();
        }
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}