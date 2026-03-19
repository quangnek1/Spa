using Microsoft.Extensions.Configuration;
using Spa.Application.DTOs.Bookings;
using Spa.Application.Interfaces;
using Stripe.Checkout;

namespace Spa.Infrastructure.Payments;

public class StripePaymentGateway : IPaymentGateway
{
    private readonly IConfiguration _configuration;

    public StripePaymentGateway(IConfiguration configuration)
    {
        _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
    }

    public async Task<StripeSessionResponseDto> CreateStripeSessionUrlAsync(BookingResponseDto booking)
    {
        // 🟢 1. LẤY TIỀN CỌC CHỨ KHÔNG LẤY TỔNG TIỀN NỮA
        var amountInCents = (long)(booking.DepositAmount * 100);
        // Đọc URL từ appsettings.json
        var frontendUrl = _configuration["Stripe:FrontendUrl"];

        var options = new SessionCreateOptions
        {
            PaymentMethodTypes = new List<string> { "card", "afterpay_clearpay" },
            LineItems = new List<SessionLineItemOptions>
            {
                new()
                {
                    PriceData = new SessionLineItemPriceDataOptions()
                    {
                        UnitAmount = amountInCents,
                        Currency = "aud",
                        ProductData = new SessionLineItemPriceDataProductDataOptions()
                        {
                            Name = $"Deposit (20%) - {booking.ServiceName}",
                            Description = $"Pay for the booking code: #{booking.Id}"
                        }
                    },
                    Quantity = 1
                }
            },
            Mode = "payment",
            SuccessUrl = $"{frontendUrl}/booking/success?session_id={{CHECKOUT_SESSION_ID}}",
            CancelUrl = $"{frontendUrl}/booking/cancel",
            Metadata = new Dictionary<string, string>
            {
                { "BookingId", booking.Id.ToString() }
            }
        };
        var service = new SessionService();
        var session = await service.CreateAsync(options);

        return new StripeSessionResponseDto() { SessionUrl = session.Url, SessionId = session.Id };
    }
}