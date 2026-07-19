namespace Lumora.Application.DTOs.Chat;

public class ChatMessageDto
{
    public int Id { get; set; }
    public string Text { get; set; } = string.Empty;
    public bool IsFromAdmin { get; set; }
    public DateTime SentAt { get; set; }
}

/// Row in the admin inbox. Student display name is resolved in the Web layer
/// (Identity lives outside Application).
public class ChatConversationDto
{
    public int Id { get; set; }
    public int StudentUserId { get; set; }
    public DateTime LastMessageAt { get; set; }
    public string LastMessagePreview { get; set; } = string.Empty;
    public int UnreadFromStudent { get; set; }
}
