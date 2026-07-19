using Lumora.Domain.Entities.Base;

namespace Lumora.Domain.Entities;

public class Testimonial : BaseTranslatableEntity<TestimonialTranslation>
{
    public string? PhotoUrl { get; set; }
    public int DisplayOrder { get; set; }
    public bool IsPublished { get; set; } = true;
}
