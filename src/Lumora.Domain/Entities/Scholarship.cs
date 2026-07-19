using Lumora.Domain.Entities.Base;

namespace Lumora.Domain.Entities;

/// University scholarship offer: original seat price crossed out, discounted price shown.
public class Scholarship : BaseTranslatableEntity<ScholarshipTranslation>
{
    public int UniversityId { get; set; }
    public University University { get; set; } = null!;

    public decimal OriginalPriceUsd { get; set; }
    public decimal DiscountedPriceUsd { get; set; }

    public string? ImageUrl { get; set; }
    public DateTime? Deadline { get; set; }
    public bool IsActive { get; set; } = true;
    public int DisplayOrder { get; set; }
}
