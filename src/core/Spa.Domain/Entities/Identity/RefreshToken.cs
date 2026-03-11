using Spa.Domain.Common;

namespace Spa.Domain.Entities.Identity
{
	public class RefreshToken : EntityBase<Guid>
	{
		public string Token { get; set; } = default!;
		public DateTime ExpiryDate { get; set; }
		public bool IsRevoked { get; set; }
		public Guid UserId { get; set; }
		public ApplicationUser User { get; set; } = default!;
	}
}
