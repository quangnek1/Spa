using System.ComponentModel.DataAnnotations.Schema;
using Spa.Domain.Common;

namespace Spa.Domain.Entities.Post;

public class PostCategory : EntitySeoAuditBase<int>
{
	[Column(TypeName = "nvarchar(250)")]
	public string Name { get; set; } = default!;
	public int? ParentId { get; set; }
	public int SortOrder { get; set; }
	public bool Status { get; set; }

	public ICollection<Post>? Posts { get; set; }
}
