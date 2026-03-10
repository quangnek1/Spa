using Spa.Domain.Common;

namespace Spa.Domain.Entities.Bookings;

public class BookingSetting : EntityBase<int>
{
    public int MaxCustomerPerSlot { get; set; }
    public int AdvanceBookingDays { get; set; }
}