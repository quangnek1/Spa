using Spa.Application.DTOs;
using Spa.Domain.Enums;

namespace Spa.Application.Interfaces;

public interface IBookingService
{
    // 1. Kiểm tra xem khung giờ đó còn chỗ không
    Task<bool> CheckAvailabilityAsync(DateTime startTime, int durationMinutes);

    // 2. Tạo đơn đặt lịch mới (Dành cho Frontend Next.js)
    Task<BookingResponseDto> CreateBookingAsync(CreateBookingRequestDto request);

    // 3. Lấy chi tiết đơn đặt lịch (Để hiển thị trang Cảm ơn hoặc cho Admin)
    Task<BookingResponseDto?> GetBookingByIdAsync(int bookingId);

    // 4. Lấy lịch sử đặt của 1 khách hàng đang đăng nhập
    Task<IEnumerable<BookingResponseDto>> GetCustomerBookingsAsync(Guid userId);

    // 5. Cập nhật trạng thái (Dành cho Webhook Stripe hoặc Admin đổi trạng thái)
    Task<bool> UpdateBookingStatusAsync(int bookingId, BookingStatus newStatus);
}