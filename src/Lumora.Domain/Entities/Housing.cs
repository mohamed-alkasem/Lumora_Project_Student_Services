using Lumora.Domain.Entities.Base;

namespace Lumora.Domain.Entities;

public class Housing : BaseTranslatableEntity<HousingTranslation>
{
    public decimal PricePerMonthUsd { get; set; }
    public string? ImageUrl { get; set; }

    public ICollection<HousingImage> Images { get; set; } = new List<HousingImage>();

    public int CityId { get; set; }
    public City City { get; set; } = null!;
}
