using Lumora.Domain.Entities.Base;

namespace Lumora.Domain.Entities;

/// Gallery photo for a university (the logo stays a scalar on University).
public class UniversityImage : BaseEntity
{
    public int UniversityId { get; set; }
    public University University { get; set; } = null!;

    public string Url { get; set; } = string.Empty;
    public int DisplayOrder { get; set; }
}
