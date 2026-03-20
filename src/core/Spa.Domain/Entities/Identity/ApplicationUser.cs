using Microsoft.AspNetCore.Identity;
using Spa.Domain.Entities.Bookings;

namespace Spa.Domain.Entities.Identity;

public class ApplicationUser : IdentityUser<Guid>
{
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public DateTime? DateOfBirth { get; set; }
    public virtual ICollection<Booking>? Bookings { get; set; }
}