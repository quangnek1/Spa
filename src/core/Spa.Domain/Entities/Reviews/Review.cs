using System.ComponentModel.DataAnnotations.Schema;
using Spa.Domain.Common;
using Spa.Domain.Entities.Identity;
using Spa.Domain.Entities.Services;

namespace Spa.Domain.Entities.Reviews;

public class Review : EntityAuditBase<int>
{
	public int ServiceId { get; set; }
	public Guid? UserId { get; set; } // Khách vãng lai cũng có thể review nếu có BookingId
	public int Rating { get; set; } // 1 - 5 sao
	[Column(TypeName = "nvarchar(max)")]
	public string? Comment { get; set; }
	public bool IsApproved { get; set; } // Admin duyệt mới được hiển thị

	public Service? Service { get; set; }
	public ApplicationUser? User { get; set; }
}
