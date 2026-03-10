using System.ComponentModel.DataAnnotations.Schema;
using Spa.Domain.Common;

namespace Spa.Domain.Entities.Services;

public class ServiceImage : EntityBase<int>
{
    public int ServiceId { get; set; }

    [Column(TypeName = "nvarchar(250)")] public string ImageUrl { get; set; } = default!;

    [Column(TypeName = "nvarchar(250)")] public string? AltText { get; set; } // Cực kỳ quan trọng cho SEO

    public int SortOrder { get; set; }
    public bool IsDefault { get; set; }

    public Service? Service { get; set; }
}