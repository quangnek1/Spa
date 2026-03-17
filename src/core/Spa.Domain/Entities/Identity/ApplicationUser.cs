using Microsoft.AspNetCore.Identity;

namespace Spa.Domain.Entities.Identity;

public class ApplicationUser : IdentityUser<Guid>
{
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public DateTime? DateOfBirth { get; set; }
}