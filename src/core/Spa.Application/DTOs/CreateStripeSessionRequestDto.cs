namespace Spa.Application.DTOs
{
	// Đặt trong thư mục DTOs
	public class CreateStripeSessionRequestDto
	{
		public int BookingId { get; set; }
		// Nơi Stripe sẽ đá khách về sau khi thanh toán xong (URL của Next.js)
		public string SuccessUrl { get; set; } = "http://localhost:3000/booking/success?session_id={CHECKOUT_SESSION_ID}";
		public string CancelUrl { get; set; } = "http://localhost:3000/booking/cancel";
	}
}
