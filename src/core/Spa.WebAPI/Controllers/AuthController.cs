using Microsoft.AspNetCore.Mvc;
using Spa.Application.DTOs;
using Spa.Application.Interfaces;

namespace Spa.WebAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
	private readonly IAuthService _authService;

	public AuthController(IAuthService authService)
	{
		_authService = authService;
	}
	[HttpPost("login")]
	public async Task<IActionResult> Login([FromBody] LoginRequestDto request)
	{
		var result = await _authService.LoginAsync(request);
		string token = result?.Token ?? string.Empty; // Lấy token nếu có, nếu không thì trả về chuỗi rỗng
		if (result == null)
		{
			return Unauthorized(new { message = "Email hoặc mật khẩu không đúng." });
		}
		return Ok(result);
	}

	[HttpPost("register")]
	public async Task<IActionResult> Register([FromBody] RegisterRequestDto request)
	{
		var result = await _authService.RegisterCustomerAsync(request);
		if (!result)
		{
			return BadRequest(new { message = "Email này đã được sử dụng hoặc có lỗi xảy ra." });
		}
		return Ok(new { message = "Đăng ký thành công!" });
	}
}
