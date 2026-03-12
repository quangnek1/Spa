using System.ComponentModel.DataAnnotations.Schema;
using Spa.Domain.Common;
using Spa.Domain.Entities.Identity;
using Spa.Domain.Entities.Payments;
using Spa.Domain.Entities.Services;
using Spa.Domain.Entities.VoucherCoupon;
using Spa.Domain.Enums;

namespace Spa.Domain.Entities.Bookings;

public class Booking : EntityAuditBase<int>
{
    public Guid? UserId { get; set; }

    // Thêm thông tin khách vãng lai
    [Column(TypeName = "nvarchar(250)")] public string? CustomerName { get; set; } = default!;
    [Column(TypeName = "varchar(20)")] public string? CustomerPhone { get; set; } = default!;
    [Column(TypeName = "varchar(250)")]
    public string? CustomerEmail { get; set; } // Cần để gửi email xác nhận và receipt Stripe

    public int ServicePackageId { get; set; }
    public int? StaffId { get; set; } // Khách chọn kỹ thuật viên (nếu có)
    public int? CouponId { get; set; } // Lưu lại mã giảm giá đã dùng

    [Column(TypeName = "decimal(12,2)")] public decimal DiscountAmount { get; set; }
    [Column(TypeName = "decimal(12,2)")] public decimal TotalPrice { get; set; }
    [Column(TypeName = "decimal(12,2)")] public decimal DepositAmount { get; set; }
    [Column(TypeName = "decimal(12,2)")] public decimal RemainingAmount { get; set; }

    public DateTime ScheduledStartTime { get; set; }
    public DateTime ScheduledEndTime { get; set; }
    public DateTime? ActualStartTime { get; set; }
    public DateTime? ActualEndTime { get; set; }
    public int DurationMinutes { get; set; }

    public PaymentMethod PaymentMethod { get; set; }
    public BookingStatus Status { get; set; }

    [Column(TypeName = "nvarchar(500)")] public string? Notes { get; set; } // Ghi chú của khách hàng khi đặt

// navigation
    public ApplicationUser? User { get; set; }
    public ServicePackage? ServicePackage { get; set; }

    public Staff? Staff { get; set; }
    public Coupon? Coupon { get; set; }

    public ICollection<Payment>? Payments { get; set; }
}

#region

//protected override void OnModelCreating(ModelBuilder modelBuilder)
//	{
//		modelBuilder.Entity<Booking>()
//			.HasIndex(b => new { b.ScheduledStartTime, b.ScheduledEndTime, b.Status })
//			.HasDatabaseName("IX_Booking_TimeSlot_Status");

//		base.OnModelCreating(modelBuilder);
//	}
//Index thứ hai nên có
//	modelBuilder.Entity<Booking>()
//    .HasIndex(b => b.CustomerId)
//    .HasDatabaseName("IX_Booking_CustomerId");

//modelBuilder.Entity<Payment>()
//    .HasIndex(p => p.BookingId)
//    .HasDatabaseName("IX_Payment_BookingId");

//modelBuilder.Entity<Service>()
//    .HasIndex(s => s.CategoryId)
//    .HasDatabaseName("IX_Service_CategoryId");

#endregion