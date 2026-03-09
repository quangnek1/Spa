using System.ComponentModel.DataAnnotations.Schema;
using Spa.Domain.Common;
using Spa.Domain.Enums;

namespace Spa.Domain.Entities.VoucherCoupon
{
	public class Coupon : EntityAuditBase<int>
	{
		[Column(TypeName = "varchar(50)")]
		public string Code { get; set; } = default!;
		public DiscountType Type { get; set; } // Phầm trăm hay Tiền mặt
		[Column(TypeName = "decimal(12,2)")]
		public decimal DiscountValue { get; set; }
		[Column(TypeName = "decimal(12,2)")]
		public decimal? MaxDiscountAmount { get; set; } // Giảm tối đa bao nhiêu (nếu là %)
		public DateTime StartDate { get; set; }
		public DateTime EndDate { get; set; }
		public int MaxUsage { get; set; } // Giới hạn số lần dùng
		public int CurrentUsage { get; set; }
		public bool IsActive { get; set; }
	}
}
