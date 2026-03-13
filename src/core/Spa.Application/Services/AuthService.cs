using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Spa.Application.DTOs.Auth;
using Spa.Application.Interfaces;
using Spa.Application.Seedwork;
using Spa.Domain.Entities.Identity;

namespace Spa.Application.Services;

public class AuthService : IAuthService
{
	private readonly IConfiguration _configuration;
	private readonly IMapper _mapper;
	private readonly UserManager<ApplicationUser> _userManager;
	private readonly IUnitOfWork _unitOfWork;

	public AuthService(UserManager<ApplicationUser> userManager, IConfiguration configuration,
		IMapper mapper, IUnitOfWork unitOfWork)
	{
		_userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
		_configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
		_mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
		_unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
	}

	public async Task<AuthResponseDto?> LoginAsync(LoginRequestDto request)
	{
		var user = await _userManager.FindByEmailAsync(request.Email);
		if (user == null ||
			!await _userManager.CheckPasswordAsync(user, request.Password)) return null; // Sai email hoặc password

		var roles = await _userManager.GetRolesAsync(user);


		var accessToken = GenerateJwtToken(user, roles);
		var refreshToken = GenerateRefreshToken();
		await _unitOfWork.RefreshTokens.AddAsync(new RefreshToken
		{
			Token = refreshToken,
			UserId = user.Id,
			ExpiryDate = DateTime.UtcNow.AddDays(30) // Refresh Token sống 30 ngày
		});
		await _unitOfWork.SaveChangesAsync();

		return new AuthResponseDto
		{
			AccessToken = accessToken,
			RefreshToken = refreshToken,
			Email = user.Email!,
			FullName = user.Name!,
			Roles = roles
		};
	}

	public async Task LogoutAsync(string refreshToken)
	{
		var token = await _unitOfWork.RefreshTokens
			.GetFirstOrDefaultAsync(x => x.Token == refreshToken);

		if (token != null)
		{
			token.IsRevoked = true;
			await _unitOfWork.SaveChangesAsync();
		}
	}

	public async Task<AuthResponseDto?> RefreshTokenAsync(string refreshToken)
	{
		var tokenEntity = await _unitOfWork.RefreshTokens
			  .GetFirstOrDefaultAsync(x => x.Token == refreshToken, x => x.User);
		//var tokenEntity = await _unitOfWork.RefreshTokens.GetFirstOrDefaultAsync(x => x.Token == refreshToken);

		var user = await _userManager.FindByIdAsync(tokenEntity.UserId.ToString());

		if (tokenEntity == null || tokenEntity.IsRevoked || tokenEntity.ExpiryDate < DateTime.UtcNow)
			return null;

		// revoke token cũ
		tokenEntity.IsRevoked = true;

		var roles = await _userManager.GetRolesAsync(tokenEntity.User);

		var newAccessToken = GenerateJwtToken(tokenEntity.User, roles);
		var newRefreshToken = GenerateRefreshToken();

		await _unitOfWork.RefreshTokens.AddAsync(new RefreshToken
		{
			Token = newRefreshToken,
			UserId = tokenEntity.UserId,
			ExpiryDate = DateTime.UtcNow.AddDays(30)
		});

		await _unitOfWork.SaveChangesAsync();


		return new AuthResponseDto
		{
			AccessToken = newAccessToken,
			RefreshToken = newRefreshToken,
		};
	}

	public async Task<bool> RegisterCustomerAsync(RegisterRequestDto request)
	{
		var existingUser = await _userManager.FindByEmailAsync(request.Email);
		if (existingUser != null) return false;

		var user = _mapper.Map<ApplicationUser>(request);

		// var user = new ApplicationUser
		// {
		// 	UserName = request.Email,
		// 	Email = request.Email,
		// 	Name = request.FullName,
		// 	PhoneNumber = request.PhoneNumber,
		// 	EmailConfirmed = true // Mặc định true cho lẹ, thực tế nên gửi email xác nhận
		// };

		var result = await _userManager.CreateAsync(user, request.Password);
		if (result.Succeeded)
		{
			// Gán role mặc định cho khách hàng
			await _userManager.AddToRoleAsync(user, "Customer");
			return true;
		}

		return false;
	}

	// Hàm private để gen ra Token
	private string GenerateJwtToken(ApplicationUser user, IList<string> roles)
	{
		var authClaims = new List<Claim>
		{
			new("uid", user.Id.ToString()),
			new("userName", user.UserName!),
			new(ClaimTypes.NameIdentifier, user.Id.ToString()),
			new(ClaimTypes.Email, user.Email!),
			new(ClaimTypes.Name, user.Name!),
			new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()) // ID duy nhất của token
        };

		// Nhồi Roles vào Token để Frontend biết đường hiển thị menu Admin hay Customer
		foreach (var role in roles) authClaims.Add(new Claim(ClaimTypes.Role, role));

		var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Secret"]!));

		var token = new JwtSecurityToken(
			issuer: _configuration["JWT:ValidIssuer"],
			audience: _configuration["JWT:ValidAudience"],
			expires: DateTime.UtcNow.AddMinutes(5), // Access token sống 5 phút
			claims: authClaims,
			signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
		);

		return new JwtSecurityTokenHandler().WriteToken(token);
	}
	private string GenerateRefreshToken()
	{
		var randomNumber = new byte[32];

		using var rng = RandomNumberGenerator.Create();
		rng.GetBytes(randomNumber);

		return Convert.ToBase64String(randomNumber);
	}

}