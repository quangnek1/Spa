using System.ComponentModel.DataAnnotations.Schema;
using Spa.Domain.Common;
using Spa.Domain.Enums;

namespace Spa.Domain.Entities.SystemConfigs
{
	public class Banner : EntityAuditBase<int>
	{
		[Column(TypeName = "nvarchar(250)")]
		public string Title { get; set; } = default!;

		[Column(TypeName = "nvarchar(250)")]
		public string ImageUrl { get; set; } = default!; // Link ảnh

		[Column(TypeName = "nvarchar(250)")]
		public string? Link { get; set; } // Bấm vào ảnh thì dẫn đi đâu

		public BannerPosition Position { get; set; }
		public int SortOrder { get; set; }
		public bool Status { get; set; }
	}
}
