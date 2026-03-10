using System.ComponentModel.DataAnnotations.Schema;
using Spa.Domain.Common;
using Spa.Domain.Entities.Bookings;
using Spa.Domain.Enums;

namespace Spa.Domain.Entities.Payments;

public class Payment : EntityAuditBase<int>
{
    public int BookingId { get; set; }

    [Column(TypeName = "decimal(12,2)")] public decimal Amount { get; set; }

    public PaymentMethod PaymentMethod { get; set; }

    // Rất quan trọng cho Stripe/VnPay: Lưu lại mã giao dịch của đối tác để đối soát/refund
    [Column(TypeName = "varchar(250)")] public string? TransactionId { get; set; }

    public DateTime? PaymentDate { get; set; }
    public PaymentStatus Status { get; set; }
    public Booking? Booking { get; set; }
}