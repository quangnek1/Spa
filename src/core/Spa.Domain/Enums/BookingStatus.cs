using System.ComponentModel.DataAnnotations;

namespace Spa.Domain.Enums;

public enum BookingStatus
{
    [Display(Name = "Booking")] Pending = 1, // vừa tạo booking, chờ thanh toán
    [Display(Name = "Confirmed")] Confirmed = 2, // đã thanh toán deposit
    [Display(Name = "InProgress")] InProgress = 3, // đang làm dịch vụ
    [Display(Name = "Completed")] Completed = 4, // hoàn thành
    [Display(Name = "Cancelled")] Cancelled = 5, // huỷ
    [Display(Name = "NoShow")] NoShow = 6 // khách không tới
}