using Lumora.Domain.Entities.Base;

namespace Lumora.Domain.Entities;

/// Gallery photo for a housing listing (ImageUrl on Housing stays the cover).
public class HousingImage : BaseEntity
{
    public int HousingId { get; set; }
    public Housing Housing { get; set; } = null!;

    public string Url { get; set; } = string.Empty;
    public int DisplayOrder { get; set; }
}
