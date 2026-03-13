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
			// 1. GỌI SERVICE 1: Xử lý Đặt lịch
			var bookingResponse = await _bookingService.CreateBookingAsync(request);

			// 2. GỌI SERVICE 2: Xử lý Thanh toán
			if (request.PaymentMethod == PaymentMethod.Stripe)
			{
				var stripeUrl = await _paymentService.GenerateStripeSessionAsync(bookingResponse);

				return Ok(new
				{
					message = "Đang chuyển hướng thanh toán...",
					redirectUrl = stripeUrl
				});
			}

			return Ok(result);
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
}