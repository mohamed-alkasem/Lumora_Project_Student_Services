using Lumora.Domain.Entities.Base;

namespace Lumora.Domain.Entities;

public class AcademicProgramTranslation : BaseTranslation
{
    public int AcademicProgramId { get; set; }
    public AcademicProgram AcademicProgram { get; set; } = null!;

    public string Name { get; set; } = string.Empty;
}
