using AutoMapper;
using Spa.Application.Mappings;
using Spa.Domain.Entities.Identity;

namespace Spa.Application.DTOs;

public class LoginRequestDto
{
    public string Email { get; set; } = default!;
    public string Password { get; set; } = default!;
}

public class RegisterRequestDto : IMapFrom<ApplicationUser>
{
    public string FullName { get; set; } = default!;
    public string Email { get; set; } = default!;
    public string Password { get; set; } = default!;
    public string PhoneNumber { get; set; } = default!;
    public bool EmailConfirmed { get; set; } = true;

    public void Mapping(Profile profile)
    {
        profile.CreateMap<RegisterRequestDto, ApplicationUser>().ReverseMap();
    }
}

public class AuthResponseDto
{
    public string Token { get; set; } = default!;
    public string Email { get; set; } = default!;
    public string FullName { get; set; } = default!;
    public IList<string> Roles { get; set; } = new List<string>();
}