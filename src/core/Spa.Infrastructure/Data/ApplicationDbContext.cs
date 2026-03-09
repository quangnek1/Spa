using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Spa.Domain.Common.InterFaces;
using Spa.Domain.Entities.Bookings;
using Spa.Domain.Entities.Customers;
using Spa.Domain.Entities.Identity;
using Spa.Domain.Entities.Payments;
using Spa.Domain.Entities.Post;
using Spa.Domain.Entities.Reviews;
using Spa.Domain.Entities.Services;
using Spa.Domain.Entities.SystemConfigs;
using Spa.Domain.Entities.VoucherCoupon;

namespace Spa.Infrastructure.Data;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, Guid>
{
	public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
	{
	}
	// 1. Users & Staff
	public DbSet<Staff> Staffs { get; set; }

	// 2. Services
	public DbSet<Category> Categories { get; set; }
	public DbSet<Service> Services { get; set; }
	public DbSet<ServicePackage> ServicePackages { get; set; }
	public DbSet<ServiceImage> ServiceImages { get; set; }
	public DbSet<Review> Reviews { get; set; }

	// 3. Bookings & Payments
	public DbSet<Booking> Bookings { get; set; }
	public DbSet<BookingSetting> BookingSettings { get; set; }
	public DbSet<Payment> Payments { get; set; }
	public DbSet<Coupon> Coupons { get; set; }

	// 4. CMS (Posts, Pages, Contacts)
	public DbSet<PostCategory> PostCategories { get; set; }
	public DbSet<Post> Posts { get; set; }
	public DbSet<Page> Pages { get; set; }
	public DbSet<Contact> Contacts { get; set; }

	// 5. System Configs
	public DbSet<SystemSetting> SystemSettings { get; set; }
	public DbSet<Menu> Menus { get; set; }
	public DbSet<Banner> Banners { get; set; }
	public DbSet<FAQ> FAQs { get; set; }
	protected override void OnModelCreating(ModelBuilder builder)
	{
		base.OnModelCreating(builder);

		// Đổi tên các bảng mặc định của Identity cho sạch sẽ (Tuỳ chọn)
		builder.Entity<ApplicationUser>().ToTable("Users");
		builder.Entity<ApplicationRole>().ToTable("Roles");
		builder.Entity<IdentityUserRole<Guid>>().ToTable("UserRoles");
		builder.Entity<IdentityUserClaim<Guid>>().ToTable("UserClaims");
		builder.Entity<IdentityUserLogin<Guid>>().ToTable("UserLogins");
		builder.Entity<IdentityRoleClaim<Guid>>().ToTable("RoleClaims");
		builder.Entity<IdentityUserToken<Guid>>().ToTable("UserTokens");

		#region SEO Indexing (Tối ưu tốc độ query cho Next.js)
		// Tạo Unique Index cho các đường dẫn Slug để Next.js query chi tiết bài viết/dịch vụ siêu nhanh
		builder.Entity<Category>().HasIndex(x => x.Slug).IsUnique();
		builder.Entity<Service>().HasIndex(x => x.Slug).IsUnique();
		builder.Entity<PostCategory>().HasIndex(x => x.Slug).IsUnique();
		builder.Entity<Post>().HasIndex(x => x.Slug).IsUnique();
		builder.Entity<Page>().HasIndex(x => x.Slug).IsUnique();

		builder.Entity<Booking>()
			.HasIndex(b => new { b.ScheduledStartTime, b.ScheduledEndTime, b.Status })
			.HasDatabaseName("IX_Booking_TimeSlot_Status");

		builder.Entity<Payment>()
			.HasIndex(p => p.BookingId)
			.HasDatabaseName("IX_Payment_BookingId");

		builder.Entity<Service>()
			.HasIndex(s => s.CategoryId)
			.HasDatabaseName("IX_Service_CategoryId");
		#endregion

		#region Booking Relationships (Chặn Cascade Delete)
		builder.Entity<Booking>(b =>
		{
			// Nếu xoá User thì không xoá Booking (Giữ lại lịch sử đối soát)
			b.HasOne(x => x.User)
			 .WithMany()
			 .HasForeignKey(x => x.UserId)
			 .OnDelete(DeleteBehavior.Restrict);


			// Nếu xoá Gói dịch vụ thì không xoá Booking cũ
			b.HasOne(x => x.ServicePackage)
			 .WithMany()
			 .HasForeignKey(x => x.ServicePackageId)
			 .OnDelete(DeleteBehavior.Restrict);

		});

		builder.Entity<Payment>()
			.HasOne(x => x.Booking)
			.WithMany(x => x.Payments)
			.HasForeignKey(x => x.BookingId)
			.OnDelete(DeleteBehavior.Restrict);
		#endregion

		#region Service Relationships
		builder.Entity<Service>()
			.HasOne(x => x.Category)
			.WithMany(x => x.Services)
			.HasForeignKey(x => x.CategoryId)
			.OnDelete(DeleteBehavior.Restrict); // Không cho xoá Danh mục nếu đang có Dịch vụ bên trong

		// Xoá Service thì xoá luôn Image, Package, Review đi kèm
		builder.Entity<ServiceImage>()
			.HasOne(x => x.Service)
			.WithMany(x => x.Images)
			.HasForeignKey(x => x.ServiceId)
			.OnDelete(DeleteBehavior.Cascade);

		builder.Entity<ServicePackage>()
			.HasOne(x => x.Service)
			.WithMany(x => x.Packages)
			.HasForeignKey(x => x.ServiceId)
			.OnDelete(DeleteBehavior.Cascade);

		builder.Entity<Review>()
			.HasOne(x => x.Service)
			.WithMany(x => x.Reviews)
			.HasForeignKey(x => x.ServiceId)
			.OnDelete(DeleteBehavior.Cascade);
		#endregion

		#region CMS Relationships
		builder.Entity<Post>()
			.HasOne(x => x.PostCategory)
			.WithMany(x => x.Posts)
			.HasForeignKey(x => x.PostCategoryId)
			.OnDelete(DeleteBehavior.Restrict);
		#endregion
	}

	public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = new CancellationToken())
	{
		// Giả sử bạn có service lấy ID của người đang đăng nhập (IHttpContextAccessor)
		// string currentUserId = _currentUserService.UserId ?? "System";
		string currentUserId = "System"; // Tạm thời hardcode, sẽ inject ICurrentUserService sau

		foreach (var entry in ChangeTracker.Entries<IAuditable>())
		{
			switch (entry.State)
			{
				case EntityState.Added:
					entry.Entity.CreatedDate = DateTimeOffset.UtcNow; // Chuẩn giờ UTC
					entry.Entity.CreatedBy = currentUserId;
					break;

				case EntityState.Modified:
					entry.Entity.LastModifiedDate = DateTimeOffset.UtcNow;
					entry.Entity.ModifiedBy = currentUserId;
					break;
			}
		}

		return base.SaveChangesAsync(cancellationToken);
	}
}

