using System.ComponentModel.DataAnnotations.Schema;
using Spa.Domain.Common;
using Spa.Domain.Entities.Services;

namespace Spa.Domain.Entities.SystemConfigs;

public class FAQ : EntityBase<int>
{
    [Column(TypeName = "nvarchar(500)")] public string Question { get; set; } = default!;

    [Column(TypeName = "nvarchar(max)")] public string Answer { get; set; } = default!;

    public int? ServiceId { get; set; } // Có thể gắn FAQ vào từng dịch vụ cụ thể, hoặc null là FAQ chung
    public int SortOrder { get; set; }
    public bool Status { get; set; }

    public Service? Service { get; set; }
}