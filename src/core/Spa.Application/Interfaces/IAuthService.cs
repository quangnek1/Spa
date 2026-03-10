using Spa.Application.DTOs;

namespace Spa.Application.Interfaces;

public interface IAuthService
{
    Task<AuthResponseDto?> LoginAsync(LoginRequestDto request);
    Task<bool> RegisterCustomerAsync(RegisterRequestDto request);
}