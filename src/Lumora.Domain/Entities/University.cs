using Lumora.Domain.Entities.Base;
using Lumora.Domain.Enums;

namespace Lumora.Domain.Entities;

public class University : BaseTranslatableEntity<UniversityTranslation>
{
    public UniversityType Type { get; set; }
    public decimal Rating { get; set; }
    public decimal AnnualTuitionUsd { get; set; }
    public int CampusCount { get; set; }
    public string? LogoUrl { get; set; }

    public int CityId { get; set; }
    public City City { get; set; } = null!;

    public ICollection<AcademicProgram> Programs { get; set; } = new List<AcademicProgram>();
    public ICollection<Favorite> Favorites { get; set; } = new List<Favorite>();
    public ICollection<UniversityImage> Images { get; set; } = new List<UniversityImage>();
}
