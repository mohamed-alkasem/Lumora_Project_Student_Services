using Lumora.Domain.Entities.Base;

namespace Lumora.Domain.Entities;

public class ChatMessage : BaseEntity
{
    public int ConversationId { get; set; }
    public ChatConversation Conversation { get; set; } = null!;

    public int SenderUserId { get; set; }

    /// True when any admin/support member sent it; false when the student did.
    public bool IsFromAdmin { get; set; }

    public string Text { get; set; } = string.Empty;

    /// Set when the opposite party opens the conversation.
    public bool IsRead { get; set; }
}
