using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Spa.Domain.Entities.Identity;
using Spa.Domain.Entities.Services;
using Spa.Domain.Entities.SystemConfigs;

namespace Spa.Infrastructure.Data;

public class SpaDbInitializer
{
	private readonly ApplicationDbContext _context;
	private readonly RoleManager<ApplicationRole> _roleManager;
	private readonly UserManager<ApplicationUser> _userManager;

	public SpaDbInitializer(
		ApplicationDbContext context,
		UserManager<ApplicationUser> userManager,
		RoleManager<ApplicationRole> roleManager)
	{
		_context = context;
		_userManager = userManager;
		_roleManager = roleManager;
	}

	public async Task SeedAsync()
	{
		if (_context.Database.IsSqlServer())
			await _context.Database.MigrateAsync();

		await SeedRolesAsync();
		await SeedUsersAsync();
		await SeedSystemSettingsAsync();
		await SeedSpaServicesAsync();
	}

	// ================= ROLES =================
	private async Task SeedRolesAsync()
	{
		var roles = new List<ApplicationRole>
		{
			new() { Name = "Administrator", Description = "Quản trị viên toàn quyền" },
			new() { Name = "Manager", Description = "Quản lý Spa" },
			new() { Name = "Therapist", Description = "Nhân viên trị liệu" },
			new() { Name = "Customer", Description = "Khách hàng" }
		};

		foreach (var role in roles)
		{
			if (!await _roleManager.RoleExistsAsync(role.Name))
				await _roleManager.CreateAsync(role);
		}
	}

	// ================= USERS =================
	private async Task SeedUsersAsync()
	{
		// ADMIN
		if (!await _userManager.Users.AnyAsync(x => x.Email == "admin@hanahspa.com"))
		{
			var admin = new ApplicationUser
			{
				UserName = "admin@hanahspa.com",
				Email = "admin@hanahspa.com",
				Name = "System Admin",
				EmailConfirmed = true
			};

			await _userManager.CreateAsync(admin, "Admin@123!");
			await _userManager.AddToRoleAsync(admin, "Administrator");
		}

		// MANAGER
		if (!await _userManager.Users.AnyAsync(x => x.Email == "manager@hanahspa.com"))
		{
			var manager = new ApplicationUser
			{
				UserName = "manager@hanahspa.com",
				Email = "manager@hanahspa.com",
				Name = "Spa Manager",
				EmailConfirmed = true
			};

			await _userManager.CreateAsync(manager, "Manager@123!");
			await _userManager.AddToRoleAsync(manager, "Manager");
		}

		// CUSTOMER SAMPLE
		if (!await _userManager.Users.AnyAsync(x => x.Email == "customer1@gmail.com"))
		{
			var customer = new ApplicationUser
			{
				UserName = "customer1@gmail.com",
				Email = "customer1@gmail.com",
				Name = "Nguyễn Văn A",
				EmailConfirmed = true
			};

			await _userManager.CreateAsync(customer, "Customer@123!");
			await _userManager.AddToRoleAsync(customer, "Customer");
		}
	}

	// ================= SYSTEM SETTINGS =================
	private async Task SeedSystemSettingsAsync()
	{
		if (await _context.SystemSettings.AnyAsync())
			return;

		var settings = new List<SystemSetting>
		{
			new() { Name = "MaxCustomerPerSlot", Type = "Booking", Value = "3", Status = true },
			new() { Name = "AdvanceBookingDays", Type = "Booking", Value = "30", Status = true },
			new() { Name = "SpaOpenTime", Type = "Time", Value = "09:00", Status = true },
			new() { Name = "SpaCloseTime", Type = "Time", Value = "21:00", Status = true },
			new() { Name = "SlotDuration", Type = "Booking", Value = "30", Status = true },
			new() { Name = "CancelBeforeHours", Type = "Booking", Value = "6", Status = true }
		};

		await _context.SystemSettings.AddRangeAsync(settings);
		await _context.SaveChangesAsync();
	}

	// ================= SPA SERVICES =================
	private async Task SeedSpaServicesAsync()
	{
		if (await _context.Categories.AnyAsync())
			return;

		var now = DateTimeOffset.UtcNow;

		// Categories
		var categories = new List<Category>
		{
			new()
			{
				Name = "Massage Body",
				Slug = "massage-body",
				SeoTitle = "Massage Body Thư Giãn",
				Sort = 1,
				Status = true,
				CreatedBy = "System",
				CreatedDate = now
			},
			new()
			{
				Name = "Gội Đầu Dưỡng Sinh",
				Slug = "goi-dau-duong-sinh",
				SeoTitle = "Gội Đầu Dưỡng Sinh Thư Giãn",
				Sort = 2,
				Status = true,
				CreatedBy = "System",
				CreatedDate = now
			},
			new()
			{
				Name = "Chăm Sóc Da Mặt",
				Slug = "cham-soc-da-mat",
				SeoTitle = "Chăm Sóc Da Mặt Chuyên Sâu",
				Sort = 3,
				Status = true,
				CreatedBy = "System",
				CreatedDate = now
			}
		};

		await _context.Categories.AddRangeAsync(categories);
		await _context.SaveChangesAsync();

		var body = categories[0];
		var head = categories[1];
		var face = categories[2];

		var services = new List<Service>
		{
			new()
			{
				CategoryId = body.Id,
				Name = "Massage Thái Trị Liệu",
				Slug = "massage-thai",
				Image = "/images/services/thai.jpg",
				ShortDescription = "Massage kéo giãn cơ chuẩn Thái.",
				Description = "Chi tiết massage thái...",
				Hot = true,
				Status = true,
				CreatedBy = "System",
				CreatedDate = now,
				Packages = new List<ServicePackage>
				{
					new() { DurationMinutes = 60, Price = 300000 },
					new() { DurationMinutes = 90, Price = 450000 },
					new() { DurationMinutes = 120, Price = 600000 }
				}
			},

			new()
			{
				CategoryId = body.Id,
				Name = "Massage Đá Nóng",
				Slug = "massage-da-nong",
				Image = "/images/services/hotstone.jpg",
				ShortDescription = "Massage đá nóng thư giãn sâu.",
				Description = "Chi tiết massage đá nóng...",
				Hot = false,
				Status = true,
				CreatedBy = "System",
				CreatedDate = now,
				Packages = new List<ServicePackage>
				{
					new() { DurationMinutes = 60, Price = 350000 },
					new() { DurationMinutes = 90, Price = 500000 }
				}
			},

			new()
			{
				CategoryId = head.Id,
				Name = "Gội Đầu Dưỡng Sinh Cơ Bản",
				Slug = "goi-dau-co-ban",
				Image = "/images/services/head1.jpg",
				ShortDescription = "Thư giãn da đầu và cổ vai gáy.",
				Description = "Chi tiết gội đầu dưỡng sinh...",
				Hot = true,
				Status = true,
				CreatedBy = "System",
				CreatedDate = now,
				Packages = new List<ServicePackage>
				{
					new() { DurationMinutes = 45, Price = 120000 },
					new() { DurationMinutes = 60, Price = 150000 }
				}
			},

			new()
			{
				CategoryId = face.Id,
				Name = "Chăm Sóc Da Mặt Cơ Bản",
				Slug = "facial-basic",
				Image = "/images/services/facial1.jpg",
				ShortDescription = "Làm sạch sâu và dưỡng da.",
				Description = "Chi tiết facial...",
				Hot = false,
				Status = true,
				CreatedBy = "System",
				CreatedDate = now,
				Packages = new List<ServicePackage>
				{
					new() { DurationMinutes = 60, Price = 250000 },
					new() { DurationMinutes = 90, Price = 400000 }
				}
			}
		};

		await _context.Services.AddRangeAsync(services);
		await _context.SaveChangesAsync();
	}
}