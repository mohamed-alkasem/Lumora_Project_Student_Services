using Lumora.Domain.Entities.Base;

namespace Lumora.Domain.Entities;

public class City : BaseTranslatableEntity<CityTranslation>
{
    public ICollection<University> Universities { get; set; } = new List<University>();
    public ICollection<Housing> Housings { get; set; } = new List<Housing>();
}
