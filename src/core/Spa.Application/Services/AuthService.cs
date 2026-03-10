using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Spa.Application.DTOs;
using Spa.Application.Interfaces;
using Spa.Domain.Entities.Identity;

namespace Spa.Application.Services;

public class AuthService : IAuthService
{
	private readonly UserManager<ApplicationUser> _userManager;
	private readonly IConfiguration _configuration;

	public AuthService(UserManager<ApplicationUser> userManager, IConfiguration configuration)
	{
		_userManager = userManager;
		_configuration = configuration;
	}

	public async Task<AuthResponseDto?> LoginAsync(LoginRequestDto request)
	{
		var user = await _userManager.FindByEmailAsync(request.Email);
		if (user == null || !await _userManager.CheckPasswordAsync(user, request.Password))
		{
			return null; // Sai email hoặc password
		}

		var roles = await _userManager.GetRolesAsync(user);
		var token = GenerateJwtToken(user, roles);

		return new AuthResponseDto
		{
			Token = token,
			Email = user.Email!,
			FullName = user.Name!,
			Roles = roles
		};
	}

	public async Task<bool> RegisterCustomerAsync(RegisterRequestDto request)
	{
		var existingUser = await _userManager.FindByEmailAsync(request.Email);
		if (existingUser != null) return false;

		var user = new ApplicationUser
		{
			UserName = request.Email,
			Email = request.Email,
			Name = request.FullName,
			PhoneNumber = request.PhoneNumber,
			EmailConfirmed = true // Mặc định true cho lẹ, thực tế nên gửi email xác nhận
		};

		var result = await _userManager.CreateAsync(user, request.Password);
		if (result.Succeeded)
		{
			// Gán role mặc định cho khách hàng
			await _userManager.AddToRoleAsync(user, "Customer");
			return true;
		}
		return false;
	}

	// Hàm private để "nặn" ra cái Token
	private string GenerateJwtToken(ApplicationUser user, IList<string> roles)
	{
		var authClaims = new List<Claim>
	{
		new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
		new Claim(ClaimTypes.Email, user.Email!),
		new Claim(ClaimTypes.Name, user.Name!),
		new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()), // ID duy nhất của token
        };

		// Nhồi Roles vào Token để Frontend biết đường hiển thị menu Admin hay Customer
		foreach (var role in roles)
		{
			authClaims.Add(new Claim(ClaimTypes.Role, role));
		}

		var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Secret"]!));

		var token = new JwtSecurityToken(
			//issuer: _configuration["JWT:ValidIssuer"],
			//audience: _configuration["JWT:ValidAudience"],
			expires: DateTime.Now.AddDays(7), // Token sống 7 ngày
			claims: authClaims,
			signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
		);

		return new JwtSecurityTokenHandler().WriteToken(token);
	}
}
