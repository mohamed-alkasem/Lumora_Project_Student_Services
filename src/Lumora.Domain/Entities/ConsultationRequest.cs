using Lumora.Domain.Entities.Base;
using Lumora.Domain.Enums;

namespace Lumora.Domain.Entities;

// StudentUserId points at ApplicationUser (Lumora.Infrastructure.Identity) — see Favorite.cs for why
// this stays a plain int instead of a navigation property.
public class ConsultationRequest : BaseEntity
{
    public int StudentUserId { get; set; }

    // Snapshot at submission time so the request stays readable even if the profile changes later.
    public string FullName { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string? Message { get; set; }

    // Optional — set when the request originates from a specific university's "Apply now" / consultation button.
    public int? UniversityId { get; set; }
    public University? University { get; set; }

    public ConsultationStatus Status { get; set; } = ConsultationStatus.New;
}
