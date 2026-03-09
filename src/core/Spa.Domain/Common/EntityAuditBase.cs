using System.ComponentModel.DataAnnotations;
using Spa.Domain.Common.InterFaces;

namespace Spa.Domain.Common;

public class EntityAuditBase<T> : EntityBase<T>, IAuditable
{
	public DateTimeOffset CreatedDate { get; set; }
	public DateTimeOffset? LastModifiedDate { get; set; }
	[Required(AllowEmptyStrings = true)]
	public string CreatedBy { get; set; } = default!;
	public string? ModifiedBy { get; set; }
}