using Lumora.Domain.Entities.Base;

namespace Lumora.Domain.Entities;

public class ScholarshipTranslation : BaseTranslation
{
    public int ScholarshipId { get; set; }
    public Scholarship Scholarship { get; set; } = null!;

    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
}
