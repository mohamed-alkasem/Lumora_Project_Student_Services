using Lumora.Domain.Entities.Base;

namespace Lumora.Domain.Entities;

// StudentUserId points at ApplicationUser (Lumora.Infrastructure.Identity), kept as a plain int here
// because Domain must not depend on Infrastructure; the relationship is wired in Infrastructure's
// EF configuration instead.
public class Favorite : BaseEntity
{
    public int StudentUserId { get; set; }

    public int UniversityId { get; set; }
    public University University { get; set; } = null!;
}
