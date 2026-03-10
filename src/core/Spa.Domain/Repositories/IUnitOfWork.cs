using Spa.Domain.Entities.Bookings;
using Spa.Domain.Entities.Customers;
using Spa.Domain.Entities.Identity;
using Spa.Domain.Entities.Payments;
using Spa.Domain.Entities.Post;
using Spa.Domain.Entities.Reviews;
using Spa.Domain.Entities.Services;
using Spa.Domain.Entities.SystemConfigs;
using Spa.Domain.Entities.VoucherCoupon;

namespace Spa.Domain.Repositories;

public interface IUnitOfWork: IDisposable
{
	// Services Module
	IGenericRepository<Category> Categories { get; }
	IGenericRepository<Service> Services { get; }
	IGenericRepository<ServicePackage> ServicePackages { get; }
	IGenericRepository<ServiceImage> ServiceImages { get; }
	IGenericRepository<Review> Reviews { get; }

	// Booking Module
	IGenericRepository<Booking> Bookings { get; }
	IGenericRepository<BookingSetting> BookingSettings { get; }
	IGenericRepository<Payment> Payments { get; }
	IGenericRepository<Coupon> Coupons { get; }

	// CMS & System Module
	IGenericRepository<PostCategory> PostCategories { get; }
	IGenericRepository<Post> Posts { get; }
	IGenericRepository<Page> Pages { get; }
	IGenericRepository<Contact> Contacts { get; }
	IGenericRepository<SystemSetting> SystemSettings { get; }
	IGenericRepository<Menu> Menus { get; }
	IGenericRepository<Banner> Banners { get; }
	IGenericRepository<FAQ> FAQs { get; }

	// Users
	IGenericRepository<Staff> Staffs { get; }

	Task<int> SaveChangesAsync();
	Task BeginTransactionAsync();
	Task CommitTransactionAsync();
	Task RollbackTransactionAsync();

}