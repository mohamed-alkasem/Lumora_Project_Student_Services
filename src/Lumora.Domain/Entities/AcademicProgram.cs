using Lumora.Domain.Entities.Base;

namespace Lumora.Domain.Entities;

public class AcademicProgram : BaseTranslatableEntity<AcademicProgramTranslation>
{
    public ICollection<University> Universities { get; set; } = new List<University>();
}
