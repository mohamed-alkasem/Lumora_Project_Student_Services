using Microsoft.AspNetCore.Identity;

namespace Lumora.Infrastructure.Identity;

public class ApplicationUser : IdentityUser<int>
{
    public string FullName { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Admin accounts are created by a SuperAdmin from inside the panel and can be disabled without deleting them.
    public bool IsActive { get; set; } = true;
}
