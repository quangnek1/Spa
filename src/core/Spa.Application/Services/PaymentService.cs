using Spa.Application.DTOs.Bookings;
using Spa.Application.Interfaces;
using Spa.Domain.Entities.Payments;
using Spa.Domain.Enums;
using Spa.Domain.Repositories;

namespace Spa.Application.Services;

public class PaymentService : IPaymentService
{
    private readonly IUnitOfWork _unitOfWork;

    public PaymentService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> ProcessBookingPaymentAsync(ProcessPaymentRequestDto request)
    {
        var booking = await _unitOfWork.Bookings.GetByIdAsync(request.BookingId);
        if (booking == null) throw new Exception("Không tìm thấy đơn đặt lịch.");

        if (booking.Status == BookingStatus.Confirmed || booking.Status == BookingStatus.Completed)
            throw new Exception("Đơn này đã được thanh toán hoặc đã hoàn thành.");

        // Bật khiên bảo vệ Transaction
        await _unitOfWork.BeginTransactionAsync();

        try
        {
            // 1. Tạo biên lai thanh toán (Payment)
            var payment = new Payment
            {
                BookingId = booking.Id,
                Amount = request.AmountPaid,
                PaymentMethod = Enum.Parse<PaymentMethod>(request.PaymentMethod, true),
                TransactionId = request.TransactionId,
                PaymentDate = DateTime.UtcNow,
                Status = PaymentStatus.Paid,
                CreatedBy = booking.UserId?.ToString() ?? "Guest"
            };

            await _unitOfWork.Payments.AddAsync(payment);

            // 2. Cập nhật lại đơn Booking
            booking.DepositAmount += request.AmountPaid;
            booking.RemainingAmount = booking.TotalPrice - booking.DiscountAmount - booking.DepositAmount;

            if (booking.RemainingAmount <= 0) booking.RemainingAmount = 0;

            // Đổi trạng thái sang Confirmed (Đã chốt lịch)
            booking.Status = BookingStatus.Confirmed;
            _unitOfWork.Bookings.Update(booking);

            // 3. Lưu toàn bộ xuống DB
            await _unitOfWork.SaveChangesAsync();
            await _unitOfWork.CommitTransactionAsync();

            return true;
        }
        catch (Exception)
        {
            // Nếu có bất kỳ lỗi gì (ví dụ sập mạng), Rollback lại toàn bộ, coi như chưa có gì xảy ra
            await _unitOfWork.RollbackTransactionAsync();
            return false;
        }
    }
}