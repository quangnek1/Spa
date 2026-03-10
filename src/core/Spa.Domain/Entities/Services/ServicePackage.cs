using System.ComponentModel.DataAnnotations.Schema;
using Spa.Domain.Common;

namespace Spa.Domain.Entities.Services;

public class ServicePackage : EntityBase<int>
{
    public int ServiceId { get; set; }
    public int DurationMinutes { get; set; }

    [Column(TypeName = "decimal(12,2)")] public decimal Price { get; set; }

    public Service? Service { get; set; }
}