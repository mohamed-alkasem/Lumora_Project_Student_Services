using Lumora.Domain.Entities.Base;

namespace Lumora.Domain.Entities;

// One conversation per student (unique StudentUserId). StudentUserId points at
// ApplicationUser — plain int, no navigation, same rule as Favorite/ConsultationRequest.
public class ChatConversation : BaseEntity
{
    public int StudentUserId { get; set; }

    /// Denormalized for sorting the admin inbox without joining messages.
    public DateTime LastMessageAt { get; set; } = DateTime.UtcNow;

    public ICollection<ChatMessage> Messages { get; set; } = new List<ChatMessage>();
}
