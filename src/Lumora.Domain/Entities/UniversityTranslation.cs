using Lumora.Domain.Entities.Base;

namespace Lumora.Domain.Entities;

public class UniversityTranslation : BaseTranslation
{
    public int UniversityId { get; set; }
    public University University { get; set; } = null!;

    public string Name { get; set; } = string.Empty;
    public string ShortDescription { get; set; } = string.Empty;
}
