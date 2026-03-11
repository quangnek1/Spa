using Spa.Domain.Common;

namespace Spa.Domain.Entities.Identity
{
	public class RefreshToken : EntityBase<int>
	{
		public string Token { get; set; }

		public DateTime ExpiryDate { get; set; }

		public bool IsRevoked { get; set; }

		public string UserId { get; set; }
		public ApplicationUser User { get; set; }
	}
}
