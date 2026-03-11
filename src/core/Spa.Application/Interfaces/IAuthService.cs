using Spa.Application.DTOs;
using Spa.Application.DTOs.Auth;

namespace Spa.Application.Interfaces;

public interface IAuthService
{
	Task<AuthResponseDto?> LoginAsync(LoginRequestDto request);
	Task LogoutAsync(string refreshToken);
	Task<AuthResponseDto?> RefreshTokenAsync(string refreshToken);
	Task<bool> RegisterCustomerAsync(RegisterRequestDto request);
}