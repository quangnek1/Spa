using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Spa.Application.DTOs.Bookings;
using Spa.Application.Interfaces;
using Spa.Domain.Enums;

namespace Spa.WebAPI.Controllers;

public class BookingsController : BaseController
{
    private readonly IBookingService _bookingService;
	private readonly IPaymentService _paymentService;

	public BookingsController(IBookingService bookingService, IPaymentService paymentService)
    {
        _bookingService = bookingService ?? throw new ArgumentNullException(nameof(bookingService));
        _paymentService = paymentService ?? throw new ArgumentNullException(nameof(paymentService));
	}

    [HttpPost]
	[AllowAnonymous]
	public async Task<IActionResult> CreateBooking([FromBody] CreateBookingRequestDto request)
    {
        try
        {
	        // 🟢 BƯỚC BẢO MẬT: XỬ LÝ USER ID TỪ TOKEN
	        if (User.Identity != null && User.Identity.IsAuthenticated)
	        {
		        // Nếu khách có mang Token hợp lệ, móc NameIdentifier (chính là UserId) ra
		        var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
		        if (!string.IsNullOrEmpty(userIdClaim))
		        {
			        // Ghi đè thông tin an toàn vào DTO, mặc kệ Frontend có gửi gì lên hay không
			        request.UserId = Guid.Parse(userIdClaim); 
		        }
	        }
	        else
	        {
		        // Nếu khách vãng lai (không có Token), ép thẳng UserId về null 
		        // để tránh trường hợp hacker cố tình nhét UserId lạ vào JSON
		        request.UserId = null;
	        }
			// 1. GỌI SERVICE 1: Xử lý Đặt lịch
			var bookingResponse = await _bookingService.CreateBookingAsync(request);

			// 2. GỌI SERVICE 2: Xử lý Thanh toán
			if (request.PaymentMethod == PaymentMethod.Stripe)
			{
				var stripeSection = await _paymentService.GenerateStripeSessionAsync(bookingResponse);

				return Ok(new
				{
					message = "Đang chuyển hướng thanh toán...",
					redirectUrl = stripeSection.SessionUrl,
					stripeSessionId = stripeSection.SessionId
				});
			}

			return Ok(new
			{
				message = "Đặt lịch thành công! Vui lòng thanh toán tại quầy khi đến Spa.",
				redirectUrl = "/booking/success" // Trả về đường dẫn trang Cảm ơn của Next.js
			});
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetBooking(int id)
    {
        var booking = await _bookingService.GetBookingByIdAsync(id);
        if (booking == null) return NotFound();
        return Ok(booking);
    }
	[HttpGet]
	public async Task<IActionResult> GetBookings([FromQuery] BookingPagingRequest request)
	{
		var result = await _bookingService.GetAllBookingPagingAsync(request);
		if (result == null) return NotFound();
		return Ok(result);
	}
	[HttpGet("available-slots")]
	[AllowAnonymous]
	public async Task<IActionResult> GetAvailableSlots([FromQuery] DateTime date, [FromQuery] int durationMinutes)
	{
		var slots = await _bookingService.GetAvailableTimeSlotsAsync(date, durationMinutes);
		return Ok(slots);
	}
	[HttpGet("my-bookings")]
	[Authorize] // Bắt buộc phải có Token (Đã đăng nhập) mới được gọi
	public async Task<IActionResult> GetMyBookings()
	{
		try
		{
			// 1. Giải mã Token JWT để lấy Id của User đang đăng nhập
			var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            
			if (string.IsNullOrEmpty(userIdString)) 
				return Unauthorized(new { message = "Vui lòng đăng nhập lại." });

			var userId = Guid.Parse(userIdString);

			// 2. Gọi Service lấy danh sách Booking của đúng ông User này
			var bookings = await _bookingService.GetCustomerBookingsAsync(userId);
            
			return Ok(bookings);
		}
		catch (Exception ex)
		{
			return BadRequest(new { message = ex.Message });
		}
	}
}