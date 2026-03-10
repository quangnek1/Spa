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
        // 1. Chạy tự động các file migration còn thiếu (nếu có)
        if (_context.Database.IsSqlServer()) await _context.Database.MigrateAsync();

        // 2. Tạo Roles
        await SeedRolesAsync();

        // 3. Tạo Admin User
        await SeedAdminUserAsync();

        // 4. Tạo Cấu hình hệ thống mặc định
        await SeedSystemSettingsAsync();

        // 5. Tạo Danh mục và Dịch vụ Spa mẫu
        await SeedSpaServicesAsync();
    }

    private async Task SeedRolesAsync()
    {
        var roles = new List<ApplicationRole>
        {
            new() { Name = "Administrator", Description = "Quản trị viên toàn quyền" },
            new() { Name = "Manager", Description = "Quản lý cơ sở Spa" },
            new() { Name = "Customer", Description = "Khách hàng" }
        };

        foreach (var role in roles)
            if (!await _roleManager.RoleExistsAsync(role.Name))
                await _roleManager.CreateAsync(role);
    }

    private async Task SeedAdminUserAsync()
    {
        if (!await _userManager.Users.AnyAsync(u => u.Email == "admin@hanahspa.com"))
        {
            var adminUser = new ApplicationUser
            {
                UserName = "admin@hanahspa.com",
                Email = "admin@hanahspa.com",
                Name = "Hanah Admin",
                EmailConfirmed = true
            };

            var result = await _userManager.CreateAsync(adminUser, "Admin@123!"); // Mật khẩu mặc định
            if (result.Succeeded) await _userManager.AddToRoleAsync(adminUser, "Administrator");
        }
    }

    private async Task SeedSystemSettingsAsync()
    {
        if (!await _context.SystemSettings.AnyAsync())
        {
            var settings = new List<SystemSetting>
            {
                new() { Name = "MaxCustomerPerSlot", Type = "Booking", Value = "3", Status = true },
                new() { Name = "AdvanceBookingDays", Type = "Booking", Value = "30", Status = true },
                new() { Name = "SpaOpenTime", Type = "Time", Value = "09:00", Status = true },
                new() { Name = "SpaCloseTime", Type = "Time", Value = "21:00", Status = true }
            };

            await _context.SystemSettings.AddRangeAsync(settings);
            await _context.SaveChangesAsync();
        }
    }

    private async Task SeedSpaServicesAsync()
    {
        if (!await _context.Categories.AnyAsync())
        {
            // Tạo Danh mục
            var bodyCategory = new Category
            {
                Name = "Massage Body",
                Slug = "massage-body",
                SeoTitle = "Dịch Vụ Massage Body Toàn Thân | Hanah Spa",
                Sort = 1,
                Status = true,
                CreatedBy = "System",
                CreatedDate = DateTimeOffset.UtcNow
            };

            var headCategory = new Category
            {
                Name = "Gội Đầu Dưỡng Sinh",
                Slug = "goi-dau-duong-sinh",
                SeoTitle = "Gội Đầu Dưỡng Sinh Thư Giãn | Hanah Spa",
                Sort = 2,
                Status = true,
                CreatedBy = "System",
                CreatedDate = DateTimeOffset.UtcNow
            };

            await _context.Categories.AddRangeAsync(bodyCategory, headCategory);
            await _context.SaveChangesAsync();

            // Tạo Dịch vụ & Gói thời gian
            var thaiMassage = new Service
            {
                CategoryId = bodyCategory.Id,
                Name = "Massage Thái Trị Liệu",
                Slug = "massage-thai-tri-lieu",
                Image = "/images/services/thai-massage.jpg",
                ShortDescription = "Phương pháp bấm huyệt kéo giãn cơ chuẩn Thái Lan.",
                Description = "Chi tiết bài massage...",
                Hot = true,
                Status = true,
                CreatedBy = "System",
                CreatedDate = DateTimeOffset.UtcNow,
                Packages = new List<ServicePackage>
                {
                    new() { DurationMinutes = 60, Price = 300000 },
                    new() { DurationMinutes = 90, Price = 450000 }
                }
            };

            await _context.Services.AddAsync(thaiMassage);
            await _context.SaveChangesAsync();
        }
    }
}