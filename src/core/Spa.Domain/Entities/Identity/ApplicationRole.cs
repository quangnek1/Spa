using Microsoft.AspNetCore.Identity;

namespace Spa.Domain.Entities.Identity;

public class ApplicationRole : IdentityRole<Guid>
{
    public string? Description { get; set; }
}