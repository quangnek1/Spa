using AutoMapper;
using Spa.Application.Mappings;
using Spa.Domain.Entities.Identity;

namespace Spa.Application.DTOs.Auth;

public class RegisterRequestDto : IMapFrom<ApplicationUser>
{
	public string LastName { get; set; } = default!;
	public string Email { get; set; } = default!;
	public string Password { get; set; } = default!;
	public string? PhoneNumber { get; set; } = default!;
	public bool? EmailConfirmed { get; set; } = true;

	public void Mapping(Profile profile)
	{
		profile.CreateMap<RegisterRequestDto, ApplicationUser>().ReverseMap();
		profile.CreateMap<RegisterRequestDto, ApplicationUser>()
			.ForMember(dest => dest.UserName, 
				opt => opt.MapFrom(src => src.Email));
	}
}
