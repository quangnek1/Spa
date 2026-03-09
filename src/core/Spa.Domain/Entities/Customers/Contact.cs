using System.ComponentModel.DataAnnotations.Schema;
using Spa.Domain.Common;
using Spa.Domain.Enums;

namespace Spa.Domain.Entities.Customers
{
	public class Contact : EntityAuditBase<int>
	{
		[Column(TypeName = "nvarchar(250)")]
		public string FullName { get; set; } = default!;

		[Column(TypeName = "varchar(50)")]
		public string Email { get; set; } = default!;

		[Column(TypeName = "varchar(20)")]
		public string Phone { get; set; } = default!;

		[Column(TypeName = "nvarchar(250)")]
		public string Subject { get; set; } = default!;

		[Column(TypeName = "nvarchar(max)")]
		public string Message { get; set; } = default!;

		public ContactStatus Status { get; set; } // Trạng thái xử lý
	}
}
