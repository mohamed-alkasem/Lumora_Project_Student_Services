using Lumora.Domain.Entities.Base;

namespace Lumora.Domain.Entities;

public class CityTranslation : BaseTranslation
{
    public int CityId { get; set; }
    public City City { get; set; } = null!;

    public string Name { get; set; } = string.Empty;
}
