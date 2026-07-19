using Lumora.Domain.Entities.Base;

namespace Lumora.Domain.Entities;

public class HousingTranslation : BaseTranslation
{
    public int HousingId { get; set; }
    public Housing Housing { get; set; } = null!;

    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
}
