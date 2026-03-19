using AutoMapper;
using Spa.Application.Mappings;
using Spa.Domain.Entities.Bookings;
using Spa.Domain.Enums;

namespace Spa.Application.DTOs.Bookings;

public class BookingResponseDto : IMapFrom<Booking>
{
	public int Id { get; set; }
	public string CustomerName { get; set; } = default!;
	public string ServiceName { get; set; } = default!;
	public int DurationMinutes { get; set; }
	public DateTime ScheduledStartTime { get; set; }
	public DateTime ScheduledEndTime { get; set; }
	public decimal TotalPrice { get; set; }
	public decimal DiscountAmount { get; set; }
	public decimal DepositAmount { get; set; } 
	public decimal FinalAmount { get; set; }
	public BookingStatus Status { get; set; }
	
	public void Mapping(Profile profile)
	{
		profile.CreateMap<Booking, BookingResponseDto>()
			// Map từ các quan hệ (Navigation Properties)
			.ForMember(dest => dest.ServiceName, opt => opt.MapFrom(src => src.ServicePackage!.Service!.Name))
			.ForMember(dest => dest.FinalAmount, opt => opt.MapFrom(src => src.RemainingAmount))
			.ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString())); // Chuyển Enum sang String
	}
}
