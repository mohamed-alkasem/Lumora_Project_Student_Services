using System.ComponentModel.DataAnnotations;
using Lumora.Application.DTOs.Base;

namespace Lumora.Application.DTOs.Cities;

public class CityDto : BaseDto
{
    public string Name { get; set; } = string.Empty;
}

public class CityTranslationInputDto : BaseTranslationDto
{
    [Required]
    [StringLength(150)]
    public string Name { get; set; } = string.Empty;
}

public class CityCreateDto
{
    [Required]
    [MinLength(1)]
    public List<CityTranslationInputDto> Translations { get; set; } = [];
}

public class CityUpdateDto : CityCreateDto
{
}
