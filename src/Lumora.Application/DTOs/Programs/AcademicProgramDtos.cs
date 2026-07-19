using System.ComponentModel.DataAnnotations;
using Lumora.Application.DTOs.Base;

namespace Lumora.Application.DTOs.Programs;

public class AcademicProgramDto : BaseDto
{
    public string Name { get; set; } = string.Empty;
}

public class AcademicProgramTranslationInputDto : BaseTranslationDto
{
    [Required]
    [StringLength(150)]
    public string Name { get; set; } = string.Empty;
}

public class AcademicProgramCreateDto
{
    [Required]
    [MinLength(1)]
    public List<AcademicProgramTranslationInputDto> Translations { get; set; } = [];
}

public class AcademicProgramUpdateDto : AcademicProgramCreateDto
{
}
