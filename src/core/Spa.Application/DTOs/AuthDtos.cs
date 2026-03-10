namespace Spa.Application.DTOs
{
	public class LoginRequestDto
	{
		public string Email { get; set; } = default!;
		public string Password { get; set; } = default!;
	}

	public class RegisterRequestDto
	{
		public string FullName { get; set; } = default!;
		public string Email { get; set; } = default!;
		public string Password { get; set; } = default!;
		public string PhoneNumber { get; set; } = default!;
	}

	public class AuthResponseDto
	{
		public string Token { get; set; } = default!;
		public string Email { get; set; } = default!;
		public string FullName { get; set; } = default!;
		public IList<string> Roles { get; set; } = new List<string>();
	}
}
