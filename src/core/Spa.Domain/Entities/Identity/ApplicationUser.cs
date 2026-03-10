using Microsoft.AspNetCore.Identity;

namespace Spa.Domain.Entities.Identity;

public class ApplicationUser : IdentityUser<Guid>
{
    public string Name { get; set; }
    public DateTime? DateOfBirth { get; set; }
}