using System.Security.Claims;
using Lumora.Application.Interfaces.Services;
using Lumora.Domain.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace Lumora.Web.Hubs;

/// Live chat between a student and the support/admin team.
/// Group name per conversation: conv-{id}. Access rules:
///  - a student may only ever join/send in their OWN conversation;
///  - any Admin/SuperAdmin may join/send in any conversation.
[Authorize]
public class ChatHub : Hub
{
    private readonly IChatService _chat;

    public ChatHub(IChatService chat)
    {
        _chat = chat;
    }

    private int CurrentUserId => int.Parse(Context.User!.FindFirstValue(ClaimTypes.NameIdentifier)!);

    private bool IsAdmin =>
        RoleConstants.AdminRoles.Any(r => Context.User!.IsInRole(r));

    private static string GroupName(int conversationId) => $"conv-{conversationId}";

    /// Student entry point: resolves (or creates) their conversation and joins its group.
    public async Task<int> JoinMyConversation()
    {
        if (!Context.User!.IsInRole(RoleConstants.Student))
        {
            throw new HubException("Students only.");
        }

        var conversationId = await _chat.GetOrCreateConversationIdAsync(CurrentUserId);
        await Groups.AddToGroupAsync(Context.ConnectionId, GroupName(conversationId));
        return conversationId;
    }

    /// Admin entry point: joins an existing conversation by id.
    public async Task JoinConversation(int conversationId)
    {
        if (!IsAdmin)
        {
            throw new HubException("Admins only.");
        }

        await Groups.AddToGroupAsync(Context.ConnectionId, GroupName(conversationId));
    }

    public async Task SendMessage(int conversationId, string text)
    {
        if (string.IsNullOrWhiteSpace(text))
        {
            return;
        }

        text = text.Trim();
        if (text.Length > 2000)
        {
            text = text[..2000];
        }

        var isAdmin = IsAdmin;
        if (!isAdmin)
        {
            // A student can only write into their own conversation.
            var ownerId = await _chat.GetConversationStudentIdAsync(conversationId);
            if (ownerId != CurrentUserId)
            {
                throw new HubException("Not your conversation.");
            }
        }

        var message = await _chat.SendAsync(conversationId, CurrentUserId, isAdmin, text);

        await Clients.Group(GroupName(conversationId)).SendAsync("message", new
        {
            message.Text,
            message.IsFromAdmin,
            SentAt = message.SentAt.ToString("HH:mm")
        });
    }
}
