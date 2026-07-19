using Lumora.Domain.Entities.Base;

namespace Lumora.Domain.Entities;

public class Faq : BaseTranslatableEntity<FaqTranslation>
{
    public int DisplayOrder { get; set; }
}
