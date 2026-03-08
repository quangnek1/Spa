using Spa.Domain.Common;

namespace Spa.Domain.Entities.Bookings;

public class Booking : EntityAuditBase<int>
{
    public int CustomerId { get; set; }
    public int ServicePackageId { get; set; }
    public int Discount { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
}