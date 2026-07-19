using Lumora.Domain.Entities.Base;

namespace Lumora.Domain.Entities;

public class TestimonialTranslation : BaseTranslation
{
    public int TestimonialId { get; set; }
    public Testimonial Testimonial { get; set; } = null!;

    public string StudentName { get; set; } = string.Empty;

    // e.g. "Computer Engineering Student, Boğaziçi University" — free text, kept per-language
    // since it reads naturally translated rather than composed from separate fields.
    public string StudentTitle { get; set; } = string.Empty;

    public string QuoteText { get; set; } = string.Empty;
}
