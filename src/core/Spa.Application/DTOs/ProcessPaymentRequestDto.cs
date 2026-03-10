namespace Spa.Application.DTOs;

public class ProcessPaymentRequestDto
{
    public int BookingId { get; set; }
    public decimal AmountPaid { get; set; }
    public string PaymentMethod { get; set; } = "Cash"; // Cash, CreditCard, VNPay, Stripe...
    public string? TransactionId { get; set; } // Mã giao dịch của VNPay/Stripe trả về
}