using Lumora.Application.DTOs.Chat;
using Lumora.Application.Interfaces.Services;
using Lumora.Domain.Entities;
using Lumora.Domain.Interfaces.Repositories;

namespace Lumora.Application.Services;

public class ChatService : IChatService
{
    private readonly IUnitOfWork _unitOfWork;

    public ChatService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    private IGenericRepository<ChatConversation> Conversations =>
        _unitOfWork.Repository<ChatConversation>();

    private IGenericRepository<ChatMessage> Messages =>
        _unitOfWork.Repository<ChatMessage>();

    public async Task<int> GetOrCreateConversationIdAsync(int studentUserId)
    {
        var existing = (await Conversations.FindAsync(c => c.StudentUserId == studentUserId))
            .FirstOrDefault();
        if (existing is not null)
        {
            return existing.Id;
        }

        var conversation = new ChatConversation { StudentUserId = studentUserId };
        await Conversations.AddAsync(conversation);
        await _unitOfWork.CommitAsync();
        return conversation.Id;
    }

    public async Task<int?> GetConversationStudentIdAsync(int conversationId)
    {
        var conversation = await Conversations.GetByIdAsync(conversationId);
        return conversation?.StudentUserId;
    }

    public async Task<IReadOnlyList<ChatMessageDto>> GetMessagesAsync(int conversationId)
    {
        var messages = await Messages.FindAsync(m => m.ConversationId == conversationId);
        return messages
            .OrderBy(m => m.CreatedAt)
            .Select(ToDto)
            .ToList();
    }

    public async Task<ChatMessageDto> SendAsync(
        int conversationId, int senderUserId, bool isFromAdmin, string text)
    {
        var message = new ChatMessage
        {
            ConversationId = conversationId,
            SenderUserId = senderUserId,
            IsFromAdmin = isFromAdmin,
            Text = text.Trim()
        };

        await Messages.AddAsync(message);

        var conversation = await Conversations.GetByIdAsync(conversationId);
        if (conversation is not null)
        {
            conversation.LastMessageAt = DateTime.UtcNow;
            Conversations.Update(conversation);
        }

        await _unitOfWork.CommitAsync();
        return ToDto(message);
    }

    public async Task<IReadOnlyList<ChatConversationDto>> GetConversationsForAdminAsync()
    {
        var conversations = await Conversations.GetAllAsync();
        var result = new List<ChatConversationDto>();

        foreach (var conversation in conversations.OrderByDescending(c => c.LastMessageAt))
        {
            var messages = await Messages.FindAsync(m => m.ConversationId == conversation.Id);
            var ordered = messages.OrderByDescending(m => m.CreatedAt).ToList();

            result.Add(new ChatConversationDto
            {
                Id = conversation.Id,
                StudentUserId = conversation.StudentUserId,
                LastMessageAt = conversation.LastMessageAt,
                LastMessagePreview = ordered.FirstOrDefault()?.Text ?? string.Empty,
                UnreadFromStudent = ordered.Count(m => !m.IsFromAdmin && !m.IsRead)
            });
        }

        return result;
    }

    public async Task MarkReadAsync(int conversationId, bool readerIsAdmin)
    {
        // The reader consumes the OTHER party's messages.
        var unread = await Messages.FindAsync(m =>
            m.ConversationId == conversationId && !m.IsRead && m.IsFromAdmin != readerIsAdmin);

        foreach (var message in unread)
        {
            message.IsRead = true;
            Messages.Update(message);
        }

        if (unread.Any())
        {
            await _unitOfWork.CommitAsync();
        }
    }

    private static ChatMessageDto ToDto(ChatMessage message) => new()
    {
        Id = message.Id,
        Text = message.Text,
        IsFromAdmin = message.IsFromAdmin,
        SentAt = message.CreatedAt
    };
}
