using Spa.Application.DTOs.Bookings;

namespace Spa.Application.Interfaces;

public interface IPaymentService
{
    Task<bool> ProcessBookingPaymentAsync(ProcessPaymentRequestDto request);
    Task<string> GenerateStripeSessionAsync(BookingResponseDto bookingDto);
}