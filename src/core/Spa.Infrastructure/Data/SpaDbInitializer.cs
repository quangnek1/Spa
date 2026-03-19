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
                FirstName = "System Admin",
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
                FirstName = "Spa Manager",
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
                FirstName = "Nguyễn Văn A",
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

        var massageCategory = new Category
        {
            Name = "Massage Rituals",
            Slug = "massage-rituals",
            SeoTitle = "Massage Rituals | Relaxing Massage Treatments",
            MetaKeywords = "massage spa, aromatherapy massage, shiatsu massage, hot stone massage",
            MetaDescription =
                "Explore relaxing massage rituals including aromatherapy bliss, shiatsu therapy, and luxury spa combos.",
            Sort = 1,
            Status = true,
            CreatedBy = "System",
            CreatedDate = now
        };

        var headSpaCategory = new Category
        {
            Name = "Head Spa Rituals",
            Slug = "japanese-head-spa-rituals",
            SeoTitle = "Japanese Head Spa Treatments",
            MetaKeywords = "japanese head spa, scalp spa treatment, head massage spa",
            MetaDescription =
                "Traditional Japanese head spa rituals designed for deep relaxation, scalp cleansing, and rejuvenation.",
            Sort = 2,
            Status = true,
            CreatedBy = "System",
            CreatedDate = now
        };

        await _context.Categories.AddRangeAsync(massageCategory, headSpaCategory);
        await _context.SaveChangesAsync();

        var services = new List<Service>
        {
            new()
            {
                CategoryId = massageCategory.Id,
                Name = "Aromatherapy Bliss",
                Slug = "aromatherapy-bliss-massage",
                Image = "service-aromatherapy.jpg",
                ShortDescription = "Relaxing Swedish-inspired aromatherapy massage.",
                Description =
                    "Inspired by traditional Swedish massage, this aromatherapy treatment melts away tension and restores deep harmony.",
                SeoTitle = "Aromatherapy Bliss Massage | Relaxing Swedish Spa Treatment",
                MetaKeywords = "aromatherapy massage, swedish massage spa, relaxation massage",
                MetaDescription =
                    "Enjoy a relaxing aromatherapy bliss massage inspired by Swedish techniques to melt away tension and restore harmony.",
                Hot = false,
                Status = true,
                CreatedBy = "System",
                CreatedDate = now,
                Packages = new List<ServicePackage>
                {
                    new() { DurationMinutes = 60, Price = 100 }
                }
            },

            new()
            {
                CategoryId = massageCategory.Id,
                Name = "Remedial Shiatsu / Cupping / Hot Stone",
                Slug = "shiatsu-cupping-hot-stone-massage",
                Image = "service-hot-stone.jpg",
                ShortDescription = "Traditional Japanese Shiatsu therapy for body balance.",
                Description =
                    "A therapeutic massage using finger pressure, stretching, and gentle techniques to release tension, improve circulation, and promote deep relaxation.",
                SeoTitle = "Shiatsu, Cupping & Hot Stone Massage",
                MetaKeywords = "shiatsu massage, hot stone massage, cupping therapy massage",
                MetaDescription =
                    "Experience therapeutic Shiatsu massage combined with cupping or hot stone treatment to relieve tension and restore balance.",
                Hot = false,
                Status = true,
                CreatedBy = "System",
                CreatedDate = now,
                Packages = new List<ServicePackage>
                {
                    new() { DurationMinutes = 90, Price = 150 }
                }
            },

            new()
            {
                CategoryId = massageCategory.Id,
                Name = "Ultra Luxury Radiance Combo",
                Slug = "ultra-luxury-radiance-combo",
                Image = "service-deluxe.jpg",
                ShortDescription = "VIP treatment combining massage, headwash, and advanced facial care.",
                Description =
                    "The most luxurious spa experience including massage, headwash, and advanced machine facial treatment that detoxifies, firms, brightens, and makes skin glow.",
                SeoTitle = "Ultra Luxury Radiance Spa Combo",
                MetaKeywords = "luxury spa combo, facial spa treatment, massage and facial spa",
                MetaDescription =
                    "The ultimate luxury spa treatment combining massage, headwash, and advanced facial care for radiant glowing skin.",
                Hot = true,
                Status = true,
                CreatedBy = "System",
                CreatedDate = now,
                Packages = new List<ServicePackage>
                {
                    new() { DurationMinutes = 150, Price = 235 }
                }
            },

            new()
            {
                CategoryId = massageCategory.Id,
                Name = "Signature Deluxe Combo",
                Slug = "signature-deluxe-combo",
                Image = "service-deep-tissue.jpg",
                ShortDescription = "Aromatherapy massage combined with Japanese head spa.",
                Description =
                    "A divine blend of Aromatherapy Bliss followed by a Traditional Japanese Head Spa for the perfect harmony of body and soul.",
                SeoTitle = "Signature Deluxe Spa Combo",
                MetaKeywords = "spa combo package, aromatherapy and head spa",
                MetaDescription =
                    "Signature deluxe spa combo including aromatherapy massage and traditional Japanese head spa.",
                Hot = true,
                Status = true,
                CreatedBy = "System",
                CreatedDate = now,
                Packages = new List<ServicePackage>
                {
                    new() { DurationMinutes = 120, Price = 180 }
                }
            },

            new()
            {
                CategoryId = headSpaCategory.Id,
                Name = "Traditional Japanese Head Spa",
                Slug = "traditional-japanese-head-spa",
                Image = "service-head-spa.jpg",
                ShortDescription = "Deep relaxing Japanese scalp treatment.",
                Description =
                    "A relaxing head spa journey with scalp cleansing, mini facial, and aromatherapy mist inspired by ancient Japanese techniques.",
                SeoTitle = "Traditional Japanese Head Spa Treatment",
                MetaKeywords = "japanese head spa, scalp spa treatment",
                MetaDescription =
                    "Traditional Japanese head spa ritual featuring scalp cleansing and relaxing head massage.",
                Hot = false,
                Status = true,
                CreatedBy = "System",
                CreatedDate = now,
                Packages = new List<ServicePackage>
                {
                    new() { DurationMinutes = 70, Price = 100 }
                }
            },

            new()
            {
                CategoryId = headSpaCategory.Id,
                Name = "Premium Japanese Head Spa",
                Slug = "premium-japanese-head-spa",
                Image = "service-premium.jpg",
                ShortDescription = "Luxury Japanese head spa with herbal foot bath and massage.",
                Description =
                    "Includes traditional Japanese head spa plus herbal foot bath, foot massage, hand and shoulder massage, and deep conditioning steam ritual.",
                SeoTitle = "Premium Japanese Head Spa Experience",
                MetaKeywords = "premium head spa, japanese scalp treatment",
                MetaDescription =
                    "Luxury Japanese head spa including herbal foot bath, steam treatment, and scalp therapy.",
                Hot = true,
                Status = true,
                CreatedBy = "System",
                CreatedDate = now,
                Packages = new List<ServicePackage>
                {
                    new() { DurationMinutes = 90, Price = 135 }
                }
            },

            new()
            {
                CategoryId = headSpaCategory.Id,
                Name = "Premium Back & Japanese Headwash Combo",
                Slug = "back-massage-japanese-headwash-combo",
                Image = "service-scalp-detox.jpg",
                ShortDescription = "Back massage combined with traditional Japanese headwash.",
                Description =
                    "Back massage to relieve neck and shoulder tension followed by the full traditional Japanese headwash ritual.",
                SeoTitle = "Back Massage & Japanese Headwash Combo",
                MetaKeywords = "back massage spa, japanese headwash",
                MetaDescription =
                    "Relaxing combo including back massage and traditional Japanese scalp cleansing ritual.",
                Hot = false,
                Status = true,
                CreatedBy = "System",
                CreatedDate = now,
                Packages = new List<ServicePackage>
                {
                    new() { DurationMinutes = 90, Price = 135 }
                }
            },

            new()
            {
                CategoryId = headSpaCategory.Id,
                Name = "Head & Back Combo",
                Slug = "head-back-massage-combo",
                Image = "service-herbal-steam.jpg",
                ShortDescription = "Back massage and scalp cleansing treatment.",
                Description =
                    "30 minutes of back massage for neck and shoulders followed by 40 minutes of relaxing scalp cleansing treatment.",
                SeoTitle = "Head and Back Massage Combo",
                MetaKeywords = "head massage spa, back massage combo",
                MetaDescription = "Relaxing spa combo including back massage and scalp cleansing therapy.",
                Hot = false,
                Status = true,
                CreatedBy = "System",
                CreatedDate = now,
                Packages = new List<ServicePackage>
                {
                    new() { DurationMinutes = 70, Price = 110 }
                }
            },

            new()
            {
                CategoryId = headSpaCategory.Id,
                Name = "Advanced Facial Care & Traditional Headwash",
                Slug = "advanced-facial-traditional-headwash",
                Image = "service-neck-shoulder.jpg",
                ShortDescription = "Advanced facial treatment followed by nourishing headwash.",
                Description =
                    "Professional machine-based facial treatment with detoxifying, brightening, firming, smoothing, and deep cleansing followed by a nourishing Japanese headwash.",
                SeoTitle = "Advanced Facial Care & Japanese Headwash",
                MetaKeywords = "facial spa treatment, japanese headwash spa",
                MetaDescription =
                    "Advanced facial care combined with traditional Japanese nourishing headwash for glowing skin and healthy scalp.",
                Hot = true,
                Status = true,
                CreatedBy = "System",
                CreatedDate = now,
                Packages = new List<ServicePackage>
                {
                    new() { DurationMinutes = 90, Price = 140 }
                }
            }
        };

        await _context.Services.AddRangeAsync(services);
        await _context.SaveChangesAsync();
    }
}