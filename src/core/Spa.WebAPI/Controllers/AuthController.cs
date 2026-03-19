using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Spa.Application.DTOs.Auth;
using Spa.Application.Interfaces;

namespace Spa.WebAPI.Controllers;

public class AuthController : BaseController
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
		if (result == null) return Unauthorized(new { message = "Email hoặc mật khẩu không đúng." });

		var cookieOptions = new CookieOptions
		{
			HttpOnly = true,
			Secure = true,
			SameSite = SameSiteMode.Strict,
			Expires = DateTime.UtcNow.AddDays(30)
		};
		Response.Cookies.Append("refreshToken", result.RefreshToken, cookieOptions);

		return Ok(new
		{
			token = result.AccessToken,
			User = new
			{
				email = result.Email,
				fullName = result.FullName,
				role = result.Roles[0]
			}
		});
	}

	[HttpPost("refresh")]
	public async Task<IActionResult> Refresh()
	{
		var refreshToken = Request.Cookies["refreshToken"];

		if (refreshToken == null)
			return Unauthorized();

		var newToken  = await _authService.RefreshTokenAsync(refreshToken);
		if (newToken == null)
			return Unauthorized();

		var cookieOptions = new CookieOptions
		{
			HttpOnly = true,
			Secure = true,
			SameSite = SameSiteMode.Strict,
			Expires = DateTime.UtcNow.AddDays(30)
		};
		Response.Cookies.Append("refreshToken", newToken.RefreshToken, cookieOptions);

		return Ok(new
		{
			accessToken = newToken.AccessToken
		});
	}
	[HttpPost("logout")]
	public async Task<IActionResult> Logout()
	{
		var refreshToken = Request.Cookies["refreshToken"];
		if (!string.IsNullOrEmpty(refreshToken))
		{
			await _authService.LogoutAsync(refreshToken);
		}
		Response.Cookies.Delete("refreshToken");

		return Ok();
	}
	[HttpPost("register")]
	public async Task<IActionResult> Register([FromBody] RegisterRequestDto request)
	{
		var result = await _authService.RegisterCustomerAsync(request);
		if (!result) return BadRequest(new { message = "Email này đã được sử dụng hoặc có lỗi xảy ra." });

		return Ok(new { message = "Đăng ký thành công!" });
	}
}