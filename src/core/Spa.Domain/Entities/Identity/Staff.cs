using System.ComponentModel.DataAnnotations.Schema;
using Spa.Domain.Common;
using Spa.Domain.Entities.Bookings;

namespace Spa.Domain.Entities.Identity;

public class Staff : EntityAuditBase<int>
{
    [Column(TypeName = "nvarchar(250)")] public string Name { get; set; } = default!;

    [Column(TypeName = "nvarchar(250)")] public string? Avatar { get; set; }

    [Column(TypeName = "nvarchar(500)")] public string? Bio { get; set; }

    public bool IsActive { get; set; }

    public ICollection<Booking>? Bookings { get; set; }
}