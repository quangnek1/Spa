using System.ComponentModel.DataAnnotations.Schema;
using Spa.Domain.Common;
using Spa.Domain.Enums;

namespace Spa.Domain.Entities.SystemConfigs;

public class Menu : EntityBase<int>
{
    [Column(TypeName = "nvarchar(250)")] public string Name { get; set; } = default!;

    [Column(TypeName = "nvarchar(250)")]
    public string Url { get; set; } = default!; // Link đích (ví dụ: /ve-chung-toi, /dich-vu/massage-body)

    public int? ParentId { get; set; } // Hỗ trợ menu đa cấp (Dropdown)
    public int SortOrder { get; set; }
    public MenuPosition Position { get; set; }
    public bool Status { get; set; }
}