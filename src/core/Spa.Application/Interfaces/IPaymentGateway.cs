using Spa.Application.DTOs.Bookings;

namespace Spa.Application.Interfaces;

public interface IPaymentGateway
{
    Task<StripeSessionResponseDto> CreateStripeSessionUrlAsync(BookingResponseDto booking);
}