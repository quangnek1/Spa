using AutoMapper;
using Spa.Application.Mappings;
using Spa.Domain.Entities.Bookings;
using Spa.Domain.Enums;

namespace Spa.Application.DTOs.Bookings;

public class CreateBookingRequestDto : IMapFrom<Booking>
{
	public Guid? UserId { get; set; } // Null nếu là khách vãng lai
	public string? CustomerName { get; set; }
	public string? CustomerPhone { get; set; }
	public string? CustomerEmail { get; set; }

	public int ServicePackageId { get; set; }
	public int? StaffId { get; set; }
	public string? CouponCode { get; set; }
	
	public PaymentMethod PaymentMethod { get; set; } 

	public DateTime ScheduledStartTime { get; set; }
	public string? Notes { get; set; }
	
	public void Mapping(Profile profile)
	{
		profile.CreateMap<CreateBookingRequestDto, Booking>()
			// Các trường chữ nhật thì tự map, nhưng các trường nhạy cảm thì cấm AutoMapper tự điền
			.ForMember(dest => dest.TotalPrice, opt => opt.Ignore())
			.ForMember(dest => dest.DiscountAmount, opt => opt.Ignore())
			.ForMember(dest => dest.RemainingAmount, opt => opt.Ignore())
			.ForMember(dest => dest.DepositAmount, opt => opt.Ignore())
			.ForMember(dest => dest.ScheduledEndTime, opt => opt.Ignore())
			.ForMember(dest => dest.DurationMinutes, opt => opt.Ignore())
			.ForMember(dest => dest.Status, opt => opt.Ignore())
			.ForMember(dest => dest.CouponId, opt => opt.Ignore());
	}
}

