using System.ComponentModel.DataAnnotations.Schema;
using Spa.Domain.Common;

namespace Spa.Domain.Entities.SystemConfigs;

public class SystemSetting : EntityBase<int>
{
	[Column(TypeName = "nvarchar(100)")]
	public string Name { get; set; }
	[Column(TypeName = "nvarchar(100)")]
	public string? Type { get; set; }
	public string Value { get; set; }
	public bool Status { get; set; }
}