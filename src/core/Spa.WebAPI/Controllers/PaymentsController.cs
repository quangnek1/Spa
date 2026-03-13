using System.Text.Json;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Spa.Application.DTOs.Bookings;
using Spa.Application.Interfaces;
using Stripe;
using Stripe.Checkout;

namespace Spa.WebAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
//[Authorize] // 🔒 BẮT BUỘC CÓ TOKEN MỚI ĐƯỢC VÀO ĐÂY
public class PaymentsController : ControllerBase
{
    private readonly IPaymentService _paymentService;

    public PaymentsController(IPaymentService paymentService)
    {
        _paymentService = paymentService ?? throw new ArgumentNullException(nameof(paymentService));
    }

    [HttpPost("process")]
    public async Task<IActionResult> ProcessPayment([FromBody] ProcessPaymentRequestDto request)
    {
        try
        {
            var result = await _paymentService.ProcessBookingPaymentAsync(request);
            if (!result) return BadRequest(new { message = "Thanh toán thất bại do lỗi hệ thống." });

            return Ok(new { message = "Thanh toán thành công. Đơn đặt lịch đã được xác nhận!" });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPost("create-checkout-session")]
    // [Authorize] // Tạm thời comment cái này lại để test cho lẹ, test xong mở ra
    public async Task<IActionResult> CreateCheckoutSession([FromBody] CreateStripeSessionRequestDto request)
    {
        // 1. Lấy thông tin Booking từ Database (thực tế bạn sẽ gọi IUnitOfWork ở đây)
        // Giả lập lấy Booking số 1 cần thanh toán 300,000 VND
        var bookingId = request.BookingId;
        var amountToPay = 300000; // Số tiền cần thanh toán
        var serviceName = "Gội đầu dưỡng sinh & Massage VIP"; // Tên dịch vụ hiển thị trên hóa đơn Stripe

        // 2. Cấu hình giỏ hàng Stripe
        var options = new SessionCreateOptions
        {
            PaymentMethodTypes = new List<string> { "card" }, // Chỉ nhận thẻ
            LineItems = new List<SessionLineItemOptions>
            {
                new()
                {
                    PriceData = new SessionLineItemPriceDataOptions
                    {
                        UnitAmount = amountToPay, // Stripe VND không dùng số thập phân, 300k = 300000
                        Currency = "aud", // Chơi tiền Việt luôn
                        ProductData = new SessionLineItemPriceDataProductDataOptions
                        {
                            Name = serviceName,
                            Description = $"Thanh toán cho mã đặt lịch #{bookingId}"
                        }
                    },
                    Quantity = 1
                }
            },
            Mode = "payment", // Chế độ thanh toán 1 lần (không phải subscription)
            SuccessUrl = request.SuccessUrl,
            CancelUrl = request.CancelUrl,

            // Gắn mã BookingId vào metadata để lát nữa Webhook của Stripe gọi về, 
            // ta còn biết tiền này là của đơn hàng nào để update Database
            Metadata = new Dictionary<string, string>
            {
                { "BookingId", bookingId.ToString() }
            }
        };

        // 3. Gửi sang Stripe tạo Session
        var service = new SessionService();
        var session = await service.CreateAsync(options);

        // 4. Trả URL về cho Next.js
        return Ok(new { url = session.Url, sessionId = session.Id });
    }

    [HttpPost("webhook")]
    [AllowAnonymous] // 🔓 Tuyệt đối không để [Authorize] ở đây, vì Stripe gọi vào làm gì có Token của hệ thống bạn!
    public async Task<IActionResult> StripeWebhook()
    {
        var json = await new StreamReader(HttpContext.Request.Body).ReadToEndAsync();

        // Lấy cái mã bí mật Webhook mà bạn vừa cấu hình trong appsettings.json
        //var endpointSecret = _configuration["Stripe:WebhookSecret"];
        //var signatureHeader = Request.Headers["Stripe-Signature"];
        //var stripeEvent = EventUtility.ConstructEvent(json, signatureHeader, endpointSecret);


        try
        {
            // Kiểm tra xem có đúng là thư do chính Stripe gửi không (chống fake)
            //var stripeEvent = EventUtility.ParseEvent(json);

            //// Bắt đúng sự kiện "Khách đã thanh toán thành công"
            //if (stripeEvent.Type == "checkout.session.completed")
            //{
            //	var session = stripeEvent.Data.Object as Stripe.Checkout.Session;

            //	// Moi cái BookingId mà lúc nãy ta đã nhét vào Metadata ra
            //	var bookingIdString = session.Metadata["BookingId"];

            //	if (int.TryParse(bookingIdString, out int bookingId))
            //	{
            //		// TIỀN ĐÃ VỀ! GỌI SERVICE CẬP NHẬT DATABASE THÔI!
            //		var paymentRequest = new ProcessPaymentRequestDto
            //		{
            //			BookingId = bookingId,
            //			AmountPaid = (decimal)(session.AmountTotal / 100), // Stripe tính bằng cent/hào, nên chia 100 nếu dùng tiền USD, nếu VND thì giữ nguyên. Do nãy ta set VND nên không cần chia, sửa lại thành: (decimal)session.AmountTotal
            //			PaymentMethod = "Stripe",
            //			TransactionId = session.Id
            //		};

            //		await _paymentService.ProcessBookingPaymentAsync(paymentRequest);

            //		// Bạn có thể log ra console để ăn mừng
            //		Console.WriteLine($"[WEBHOOK] Đã nhận tiền thành công cho Booking #{bookingId}");
            //	}
            //}
            using var jsonDoc = JsonDocument.Parse(json);
            var root = jsonDoc.RootElement;
            var eventType = root.GetProperty("type").GetString();

            if (eventType == "checkout.session.completed")
            {
                var dataObj = root.GetProperty("data").GetProperty("object");
                var bookingIdString = dataObj.GetProperty("metadata").GetProperty("BookingId").GetString();
                var amountTotal = dataObj.GetProperty("amount_total").GetDecimal();
                var transactionId = dataObj.GetProperty("id").GetString();

                if (int.TryParse(bookingIdString, out var bookingId))
                {
                    var paymentRequest = new ProcessPaymentRequestDto
                    {
                        BookingId = bookingId,
                        AmountPaid = amountTotal,
                        PaymentMethod = "Stripe",
                        TransactionId = transactionId
                    };

                    await _paymentService.ProcessBookingPaymentAsync(paymentRequest);
                    Console.WriteLine(
                        $"[WEBHOOK] Ting ting! Đã update Database thành công cho Booking #{bookingId}. Thu về: {amountTotal} VND");
                }
            }

            return Ok(); // Trả về 200 OK để báo cho Stripe biết "Tao nhận được rồi, đừng gửi nữa"
        }
        catch (StripeException e)
        {
            Console.WriteLine($"[WEBHOOK LỖI] {e.Message}");
            return BadRequest();
        }
        catch (Exception e)
        {
            Console.WriteLine($"[LỖI HỆ THỐNG] {e.Message}");
            return StatusCode(500);
        }
    }
}